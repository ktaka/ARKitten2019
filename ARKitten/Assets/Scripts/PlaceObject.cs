using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// AR Foundationを使用する際は次の2つのusingを追加する
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
// ARCore Extensionsを使用する際に追加する
using Google.XR.ARCoreExtensions;

// Firebase Realtime Databaseを使用する際に追加する
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class PlaceObject : MonoBehaviour
{
    // public変数はUnityのエディタのインスペクターで設定項目として表示される
    public GameObject placedPrefab; // 配置用モデルのプレハブ
    public bool useAR = true; // AR使用フラグ（プレイモードで実行する際はfalseにする）
    public GameObject floorPlane; // プレイモード用の床面
    public float rotateDuration = 3.0f; // 回転所要時間
    public float delayTime = 3.0f; // 回転を始めるまでの時間
    public float itchingDuration = 20.0f; // シャカシャカ掻く間隔の時間（秒）
    public GameObject shareCloudAnchorButton; // Cloud Anchorを共有するボタン
    public string RoomID = "1"; // テスト用にRoom IDは1とする
    public FoodControl foodControl;
    public BallControl ballControl;

    GameObject cloudAnchorInputField; // Cloud Anchor ID入力フィールド
    ARAnchorManager anchorManager;
    ARAnchor arAnchor;
    ARCloudAnchor cloudAnchor;
    string cloudAnchorId; // Cloud Anchor ID
    GameObject spawnedObject; // 配置モデルのプレハブから生成されたオブジェクト
    // ARRaycastManagerは画面をタッチした先に伸ばしたレイと平面の衝突を検知する
    ARRaycastManager raycastManager;
    ARSessionOrigin arSession;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    Quaternion rotateFrom; // 回転開始値
    Quaternion rotateTo; // 回転終了値
    float rotateDelta; // 回転アニメーション残り時間
    Animator animator; // 子猫のアニメーター
    Rigidbody rb; // 子猫のリジッドボディ
    bool isMoving = false; // 子猫の移動中を示すフラグ
    float arrivalTime; // 子猫が目的の位置まで移動するのにかかる時間
    float speed; // 子猫の移動スピード
    float nextItchingTime; // 次にシャカシャカ掻くまでの時間（秒）
    bool isCloudMaster = true; // 複数端末でAR空間を共有する際のマスター端末かどうかを示す
    string uuid; // 複数端末でAR空間を共有する際の各端末を識別するためのID
    Pose hitPoseForAnchor; // 直近のタップ位置に対応する平面上の位置を保持する

    // for Firebase
    public string databaseUrl; // データベースのURL
    FirebaseApp app;
    DatabaseReference dbRef; // データベースへのリファレンス

    // オブジェクト配置時に呼び出すコールバック
    public static event Action onPlacedObject;

    // 起動時に1度呼び出される
    void Start()
    {
#if UNITY_EDITOR
        // エディタで実行する際はAR使用フラグをOFFにする
        useAR = false;
#else
        // 端末で実行する際はAR使用フラグをONにする
        useAR = true;
#endif
        // オブジェクトに追加されているARRaycastManagerコンポーネントを取得
        raycastManager = GetComponent<ARRaycastManager>();
        // オブジェクトに追加されているARAnchorManagerコンポーネントを取得
        anchorManager = GetComponent<ARAnchorManager>();

        // プレイモード用床面が設定されていて、AR使用フラグがOFFの場合は床面を表示
        if (floorPlane != null)
        {
            floorPlane.SetActive(!useAR);
        }
        // 関連付けられたCameraオブジェクトを使用するためARSessionOriginを取得する
        arSession = GetComponent<ARSessionOrigin>();
        // 次にシャカシャカ掻くまでの時間をセット
        nextItchingTime = Time.time + itchingDuration;
        // Cloud Anchorを共有するボタンは非表示にしておく
        shareCloudAnchorButton.SetActive(false);
        // UUID生成（AR空間を共有する際の各端末を識別するため）
        Guid guid = Guid.NewGuid();
        uuid = guid.ToString();
    }

    // フレーム毎に呼び出される
    void Update()
    {
        // Cloud Anchor に関する処理
        CloudAnchorProcess();
        // 移動中フラグが立っている場合は回転しない
        if (spawnedObject != null && isMoving == false)
        {
            // 回転アニメーション残り時間が0より大きい場合は回転させる
            if (rotateDelta <= 0.0f)
            {
                // 回転アニメーションをリセットする
                ResetRotateAnim();
                // 配置オブジェクトの向きとカメラへの方向をチェックして回転に必要な値を求める
                CheckObjDirection();
            }
            else
            {
                // 配置オブジェクトをカメラの方に回転させる
                RotateToCamera();
                // 回転アニメーション残り時間を求める
                rotateDelta -= Time.deltaTime;
            }
        }
    }

    // Firebase Realtime Databaseの構造を扱うためのクラス
    // Room単位にCloud Anchor IDを持つためのクラス
    public class Room
    {
        public string anchorId; // Cloud Anchor ID
        public Room(string cloudAnchorId)
        {
            anchorId = cloudAnchorId;
        }
    }

    // Firebase Realtime Databaseの構造を扱うためのクラス
    // 配置するオブジェクトごとに位置と向き（回転）を持つためのクラス
    public class PlacedObject
    {
        public Vector3 position; // オブジェクトの位置
        public Quaternion rotation; // オブジェクトの向きを表す回転
        public Vector4 moveTo; // 移動先の座標（4つめの要素は移動先の手前で止めるためのオフセット）
        public RotateObject rotateObj; // 回転の開始と終了の値
        public string animation; // アニメーションの名前

        public PlacedObject(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }

    // Firebase Realtime Databaseの構造を扱うためのクラス
    // 回転の開始と終了の値を扱う
    public class RotateObject
    {
        public Quaternion from; // 回転の開始値
        public Quaternion to; // 回転の終了値
        public string start; // 開始時間

        public RotateObject(Quaternion from, Quaternion to, DateTime start)
        {
            this.from = from;
            this.to = to;
            // 開始時間は文字列化して保持する
            this.start = start.ToString();
        }
    }

    // Firebase Realtime Databaseの構造を扱うためのクラス
    // ボールの位置と向き（回転）、投げる力のベクトルを持つためのクラス
    public class BallObject
    {
        public Vector3 position; // オブジェクトの位置
        public Quaternion rotation; // オブジェクトの向きを表す回転
        public Vector3 force; // 投げる力のベクトル
        public string uuid; // AR空間を共有する際の各端末を識別するためのID

        public BallObject(Vector3 position, Quaternion rotation, Vector3 throwForce, string uuid)
        {
            this.position = position;
            this.rotation = rotation;
            this.force = throwForce;
            this.uuid = uuid;
        }
    }

    // Firebaseの前処理を行い、前処理が完了済みなら渡された処理を実行する
    void SetupFirebase(Action<string> action, string key)
    {
        // DBのリファレンスが取得済みなら前処理は完了済みとする
        if (dbRef != null)
        {
            // 渡された処理を実行する
            action(key);
            return;
        }

        // Google Play開発者サービスのバージョンがFirebase SDKが必要とするものかを確認
        // 必要な場合はそのバージョンに更新する
        // ContinueWithの先は非同期で実行される
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            // 非同期で実行される処理：確認結果が返ってきた時に実行される
            var dependencyStatus = task.Result;
            // Google Play開発者サービスの必要条件を満たせた時
            if (dependencyStatus == DependencyStatus.Available)
            {
                // FirebaseAppのインスタンスを取得する
                app = FirebaseApp.DefaultInstance;
                // データベースのURLをセットする
                app.SetEditorDatabaseUrl(databaseUrl);
                // データベースのルートへのリファレンスを取得する
                dbRef = FirebaseDatabase.DefaultInstance.RootReference;
                // 渡された処理を実行する
                action(key);
            }
            else
            {
                // Google Play開発者サービスの必要条件を満たせなかった時はエラーとして
                // Firebaseに関する処理は行わない
                Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }

    // SetupFirebaseに渡して実行される処理
    // Firebase Realtime DatabaseにRoom情報を出力する
    void WriteRoomInfo(string roomId)
    {
        // Cloud Anchor IDを含むRoom情報を作成する
        Room room = new Room(cloudAnchorId);
        // ひとつのRoomオブジェクトをまとめて登録するためにJSONの文字列に変換
        string json = JsonUtility.ToJson(room);
        // Room IDをキーとしてDBに出力する
        dbRef.Child("rooms").Child(roomId).SetRawJsonValueAsync(json);

        // アンカーのTransformを子猫のTransformの親にして、子猫の位置と向きがアンカーの影響を受けるようにする
        spawnedObject.transform.parent = null; // 前に設定したTransformの親を外す
        　　// trueを指定することで親を設定する前の位置と回転を保った状態で親子関係が生成される
        　　// ==> 親のTransformと子のlocalPositionとlocalRotationの組合せで再現可能になる
        spawnedObject.transform.SetParent(arAnchor.transform, true);

        // 位置と向きを表す回転を含む配置オブジェクト情報を作成する
        PlacedObject kittenObj = new PlacedObject(spawnedObject.transform.localPosition, spawnedObject.transform.localRotation);
        // ひとつの配置オブジェクトをまとめて登録するためにJSONの文字列に変換
        json = JsonUtility.ToJson(kittenObj);
        // 子猫の配置情報にRoom IDをキーとしてDBに出力する
        dbRef.Child("kitten").Child(roomId).SetRawJsonValueAsync(json);
    }

    // SetupFirebaseに渡して実行される処理
    // Firebase Realtime DatabaseからRoom情報を読み込む
    void ReadRoomInfo(string roomId)
    {
        // Room IDに対応するCloud Anchor IDを読み込む
        // ContinueWithの先は非同期で実行される
        dbRef.Child("rooms").Child(roomId).Child("anchorId").GetValueAsync().ContinueWith(readtask =>
        {
            // 非同期で実行される処理：読み込みが完了した時に実行される
            if (readtask.IsCompleted)
            {
                // 読み込んだ内容を文字列に変換してCloud Anchor IDとする
                DataSnapshot snapshot = readtask.Result;
                string id = snapshot.Value.ToString();
                // IDに対応するCloud Anchorを取得する
                ResolveAnchor(id);
                // Firebase Realtime Databaseの更新通知を受け取るための設定
                SetupDBEventListners(roomId);
            }
        });
    }

    // ごはんの配置情報を同期する
    public void WriteFoodInfo(GameObject obj)
    {
        // Firebase Realtime Databaseが設定されていない時は何もしない
        if (dbRef == null)
        {
            return;
        }
        // Cloud Anchorのトランスフォームを親にする（現在位置が変わらないようワールド座標は考慮される）
        obj.transform.SetParent(cloudAnchor.transform, true);
        // 位置と向きを表す回転を含む配置オブジェクト情報を作成する
        PlacedObject foodObj = new PlacedObject(obj.transform.localPosition, obj.transform.localRotation);
        // ひとつの配置オブジェクトをまとめて登録するためにJSONの文字列に変換
        string json = JsonUtility.ToJson(foodObj);
        // ごはんの配置情報にRoom IDをキーとしてDBに出力する
        dbRef.Child("food").Child(RoomID).SetRawJsonValueAsync(json);
    }

    // ボールを投げた情報を同期する
    public void WriteBallInfo(GameObject obj, Vector3 throwForce)
    {
        // Firebase Realtime Databaseが設定されていない時は何もしない
        if (dbRef == null)
        {
            return;
        }
        // Cloud Anchorのトランスフォームを親にする（現在位置が変わらないようワールド座標は考慮される）
        obj.transform.SetParent(cloudAnchor.transform, true);
        // 位置と向きと投げた力を表すオブジェクトを作成する
        BallObject ballObj = new BallObject(obj.transform.localPosition, obj.transform.localRotation, throwForce, uuid);
        // ひとつのオブジェクトをまとめて登録するためにJSONの文字列に変換
        string json = JsonUtility.ToJson(ballObj);
        // ボールを投げた情報をRoom IDをキーとしてDBに出力する
        dbRef.Child("ball").Child(RoomID).SetRawJsonValueAsync(json);
    }

    // 渡されたRoom IDに対応するFirebase Realtime Databaseの更新通知を受け取るための設定
    void SetupDBEventListners(string roomId)
    {
        // 子猫の移動
        dbRef.Child("kitten").Child(roomId).Child("moveTo").ValueChanged += MoveToValueChanged;
        // 子猫の回転
        dbRef.Child("kitten").Child(roomId).Child("rotateObj").ValueChanged += RotateObjValueChanged;
        // 子猫のアニメーション
        dbRef.Child("kitten").Child(roomId).Child("animation").ValueChanged += AnimationValueChanged;
        // ごはんの配置
        dbRef.Child("food").Child(roomId).ValueChanged += FoodObjectPlaced;
        // ボールを投げた情報
        dbRef.Child("ball").Child(roomId).ValueChanged += BallObjectThrown;
    }

    // ボールを投げた情報が同期された時の処理
    // （投げる操作をした端末とは別の端末でボールを生成し同じ方向に飛んでいく）
    void BallObjectThrown(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            // エラーがあった時は何もしない
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // 読み込んだ内容から端末を識別するIDを取り出す
        DataSnapshot snapshot = args.Snapshot;
        string ballUuid = snapshot.Child("uuid").Value.ToString();
        if (ballUuid == uuid)
        {
            // 同期された情報が自分自身で更新したものならば何もしない
            return;
        }
        // JSONの文字列として取得した位置の情報（x,y,z）を位置を示すオブジェクトに変換する
        Vector3 pos = JsonUtility.FromJson<Vector3>(snapshot.Child("position").GetRawJsonValue());
        // JSONの文字列として取得した投げる力のベクトルの情報（x,y,z）をベクトルを示すオブジェクトに変換する
        Vector3 throwForce = JsonUtility.FromJson<Vector3>(snapshot.Child("force").GetRawJsonValue());
        // Cloud Anchorを基準として位置と投げる力を与えてボールを生成する
        ballControl.placeWithAnchor(cloudAnchor.transform, pos, throwForce);
    }

    // ごはんを配置した情報が同期された時の処理
    // （操作をした端末とは別の端末でごはんを同じ位置に生成する）
    void FoodObjectPlaced(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            // エラーがあった時は何もしない
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // 読み込んだ内容から位置の情報を取り出す
        DataSnapshot snapshot = args.Snapshot;
        // JSONの文字列として取得した位置の情報（x,y,z）を位置を示すオブジェクトに変換する
        Vector3 pos = JsonUtility.FromJson<Vector3>(snapshot.Child("position").GetRawJsonValue());
        // Cloud Anchorを基準として位置を与えてごはんを生成する
        foodControl.placeWithAnchor(cloudAnchor.transform, pos);
    }

    // 子猫の移動情報が同期された時の処理
    // （プレイヤーに子猫が近づいてくる時の動きを同期する）
    void MoveToValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            // エラーがあった時は何もしない
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // 読み込んだ内容から移動のための情報を取り出す
        DataSnapshot snapshot = args.Snapshot;
        // JSONの文字列として取得した情報（w,x,y,z）を変換する
        //   wはオフセット, X,Y,Zは移動先の位置
        Vector4 posAndOffset = JsonUtility.FromJson<Vector4>(snapshot.GetRawJsonValue());
        Vector3 moveToPos = new Vector3(posAndOffset.x, posAndOffset.y, posAndOffset.z);
        // 子猫を移動するアニメーションを起動する
        MoveTo(moveToPos, posAndOffset.w);
    }

    // 子猫の回転情報が同期された時の処理
    // （プレイヤーの方に子猫が向きを変える時の動きを同期する）
    void RotateObjValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            // エラーがあった時は何もしない
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // 読み込んだ内容から向きを表す回転の情報を取り出す
        DataSnapshot snapshot = args.Snapshot;
        rotateFrom = JsonUtility.FromJson<Quaternion>(snapshot.Child("from").GetRawJsonValue());
        rotateTo = JsonUtility.FromJson<Quaternion>(snapshot.Child("to").GetRawJsonValue());
        //string start = snapshot.Child("delay").Value.ToString();
        //DateTime startTime = DateTime.Parse(start);
        //TimeSpan elapsed = startTime.Subtract(DateTime.UtcNow);
        //rotateDelta = rotateDuration + (float)elapsed.TotalSeconds;
        rotateDelta = rotateDuration + delayTime;
    }

    // 子猫のアニメーション情報が同期された時の処理
    // （座る、シャカシャカする等のアニメーションを同期する）
    void AnimationValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // 読み込んだ内容からアニメーション名の情報を取り出す
        DataSnapshot snapshot = args.Snapshot;
        string animation = snapshot.Value.ToString();
        if (animation.Length > 0)
        {
            // アニメーション名が有効（文字列長が0より大きい）時はトリガーにセットして
            // アニメーションを変更する
            animator.SetTrigger(animation);
        }
    }

    // 子猫のアニメーション名をFirebase Realtime Databaseにセットする
    public void SyncAnimation(string animation)
    {
        if (dbRef != null)
        {
            dbRef.Child("kitten").Child(RoomID).Child("animation").SetValueAsync(animation);
        }
    }

    // Cloud Anchorに関する処理モード
    enum CloudAnchorProcessMode
    {
        WaitingForHosttedCloudAnchor,  // 登録の完了を待つモード
        WaitingForResolvedCloudAnchor, // 取得の完了を待つモード
        WaitingForNoOne // Cloud Anchorの処理は必要ないモード
    }
    // Cloud Anchorの処理は必要ないモードで初期化しておく
    CloudAnchorProcessMode cloudAnchorMode = CloudAnchorProcessMode.WaitingForNoOne;

    // Cloud Anchorに関する処理
    void CloudAnchorProcess()
    {
        if (cloudAnchorMode == CloudAnchorProcessMode.WaitingForNoOne ||
            cloudAnchor == null)
        {
            // Cloud Anchorの処理は必要ないモードの時、もしくは
            // CloudAnchorオブジェクトがセットされていない時は何もしない
            return;
        }

        // Cloud Anchorオブジェクトの処理の状態を取得する
        CloudAnchorState cloudAnchorState = cloudAnchor.cloudAnchorState;
        // Cloud Anchorの登録の完了を待つモードの時
        if (cloudAnchorMode == CloudAnchorProcessMode.WaitingForHosttedCloudAnchor)
        {
            // 登録処理が正常終了となるまで毎フレーム状態を確認する
            if (cloudAnchorState == CloudAnchorState.Success)
            {
                // Cloud Anchor登録処理が正常終了の時
                // Cloud Anchor ID共有ボタンを表示状態にする
                shareCloudAnchorButton.SetActive(true);
                // Cloud Anchor IDを取得する
                cloudAnchorId = cloudAnchor.cloudAnchorId;
                // Firebase Realtime DatabaseにRoom情報を出力する
                SetupFirebase(WriteRoomInfo, RoomID);
                // Cloud Anchor ID共有ボタンのスクリプトコンポーネントにIDをセットする
                shareCloudAnchorButton.GetComponent<ShareCloudAnchor>().cloudAnchorID = cloudAnchorId;
                // Cloud Anchorの処理は必要ないモードにする
                cloudAnchorMode = CloudAnchorProcessMode.WaitingForNoOne;
                // Cloud Anchor登録側をマスターとする
                isCloudMaster = true;
            }
        }
        // Cloud Anchorの取得の完了を待つモードの時
        else if (cloudAnchorMode == CloudAnchorProcessMode.WaitingForResolvedCloudAnchor)
        {
            Debug.Log("WaitingForResolvedCloudAnchor " + cloudAnchorState);
            // 取得処理が正常終了となるまで毎フレーム状態を確認する
            if (cloudAnchorState == CloudAnchorState.Success)
            {
                // Cloud Anchor取得処理が正常終了の時
                Debug.Log("===>Success to Resolve Anchor ID");
                // Cloud Anchorオブジェクトのtransformに位置と回転の情報が入っているので、それを元に子猫を配置する
                PlaceWithAnchor(cloudAnchor);
                // Cloud Anchorの処理は必要ないモードにする
                cloudAnchorMode = CloudAnchorProcessMode.WaitingForNoOne;
                // Cloud Anchor取得側はマスターにはしない
                isCloudMaster = false;
            }
        }
    }

    // Cloud Anchorの位置と回転を用いて子猫を配置する
    void PlaceWithAnchor(ARCloudAnchor anchor)
    {
        if (spawnedObject == null)
        { // 配置用モデルが未生成の場合
            // プレハブから配置用モデルを生成する
            spawnedObject = Instantiate(placedPrefab, Vector3.zero, Quaternion.identity);
            // アンカーのTransformを子猫のTransformの親にして、子猫の位置と向きがアンカーの影響を受けるようにする
            spawnedObject.transform.SetParent(anchor.transform, false);
            // 子猫のアニメーターを取得
            // （歩く、アイドルのアニメーションを制御するため）
            animator = spawnedObject.GetComponent<Animator>();
            // 子猫のリジッドボディを取得
            // （位置や回転を制御するため）
            rb = spawnedObject.GetComponent<Rigidbody>();
            // 
            if (onPlacedObject != null)
            {
                // オブジェクトが配置されたことを知らせるコールバックを呼び出す
                onPlacedObject();
            }
        }
        else
        { // 配置するモデルが生成済みの場合
            // アンカーのTransformを子猫のTransformの親にして、子猫の位置と向きがアンカーの影響を受けるようにする
            spawnedObject.transform.parent = null; // 前に設定したTransformの親を外す
            spawnedObject.transform.SetParent(anchor.transform, false);
        }

        // Room IDに対応する子猫の配置情報を読み込む
        // ContinueWithの先は非同期で実行される
        dbRef.Child("kitten").Child(RoomID).GetValueAsync().ContinueWith(readtask =>
        {
            // 非同期で実行される処理：読み込みが完了した時に実行される
            if (readtask.IsCompleted)
            {
                // 読み込んだ内容から向きを表す回転の情報を取り出す
                DataSnapshot snapshot = readtask.Result;
                // JSONの文字列として取得した回転の情報（w,x,y,z）をクオータニオンのオブジェクトに変換する
                Quaternion rot = JsonUtility.FromJson<Quaternion>(snapshot.Child("rotation").GetRawJsonValue());
                Vector3 pos = JsonUtility.FromJson<Vector3>(snapshot.Child("position").GetRawJsonValue());
                // 子猫のローカル座標系の位置と回転としてセットする
                //   親のTransformと子のlocalPositionとlocalRotationの組合せで
                //   Cloud Anchor登録時の位置と向きが再現できる
                spawnedObject.transform.localPosition = pos;
                spawnedObject.transform.localRotation = rot;
            }
        });
    }

    // IDに対応するCloud Anchorを取得する
    //   uiObjectにはCloud Anchor ID入力フィールドが渡される
    public void ResolveAnchor(string key, GameObject uiObject)
    {
        // 正常終了時に非表示にするためRoom ID入力フィールドを保持する
        cloudAnchorInputField = uiObject;
        // Firebase Realtime DatabaseからRoom情報を読み取る
        SetupFirebase(ReadRoomInfo, key);
    }

    public void ResolveAnchor(string id)
    {
        Debug.Log("===>Resolve Anchor ID=" + id);
        // IDからCloud Anchorオブジェクトを取得する
        cloudAnchor = anchorManager.ResolveCloudAnchorId(id);
        if (cloudAnchor == null)
        {
            // 取得に失敗
            Debug.Log("Failed to resolve Cloud Anchor.");
        }
        else
        {
            // 取得に成功した場合
            // Cloud Anchorの取得の完了を待つモードにする
            cloudAnchorMode = CloudAnchorProcessMode.WaitingForResolvedCloudAnchor;
        }
    }

    // 子猫を配置した時のPose情報を元にRoom IDをキーとしてCloud Anchorを登録する
    public void AddCloudAnchor(string roomId)
    {
        RoomID = roomId;
        AddCloudAnchor(hitPoseForAnchor);
    }

    // 子猫を配置した時のPose情報を元にCloud Anchorを登録する
    public void AddCloudAnchor(Pose pose)
    {
        // Cloud Anchor ID共有ボタンを非表示にする
        shareCloudAnchorButton.SetActive(false);
        // Poseからアンカーを作成する
        arAnchor = anchorManager.AddAnchor(pose);
        // 作成したアンカーをCloud Anchorに登録する
        cloudAnchor = anchorManager.HostCloudAnchor(arAnchor);
        if (cloudAnchor == null)
        {
            // 登録に失敗
            Debug.Log("Failed to create Cloud Anchor.");
        }
        else
        {
            // 登録に成功した場合
            // Cloud Anchor登録処理の完了を待つモードにする
            cloudAnchorMode = CloudAnchorProcessMode.WaitingForHosttedCloudAnchor;
        }
        cloudAnchorId = "";
    }

    // 画面がタッチされた際にUIManagerから呼び出される
    public void OnTouch(Vector2 touchPosition, bool addCloudAnchor = false)
    {
        Debug.Log("OnTouch");

        if (HitTest(touchPosition, out Pose hitPose))
        { // タッチした先に平面がある場合
            // モデル（子猫）を配置する位置からカメラへの方向のベクトルを求めて
            // モデルをどのくらい回転させるか求める
            Quaternion rotation = Quaternion.LookRotation(GetLookVector(hitPose.position));
            if (spawnedObject == null)
            { // 配置用モデルが未生成の場合
                // プレハブから配置用モデルを生成し、レイが平面に衝突した位置に配置する
                spawnedObject = Instantiate(placedPrefab, hitPose.position, rotation);
                // 子猫のアニメーターを取得
                // （歩く、アイドルのアニメーションを制御するため）
                animator = spawnedObject.GetComponent<Animator>();
                // 子猫のリジッドボディを取得
                // （位置や回転を制御するため）
                rb = spawnedObject.GetComponent<Rigidbody>();
                // 
                if (onPlacedObject != null)
                {
                    // オブジェクトが配置されたことを知らせるコールバックを呼び出す
                    onPlacedObject();
                }
            }
            else
            { // 配置するモデルが生成済みの場合
                // 配置用モデルの位置をレイが平面に衝突した位置にする
                rb.position = hitPose.position;
                // 配置用モデルを回転させてカメラの方に向ける
                rb.rotation = rotation;
            }
            hitPoseForAnchor = hitPose;
        }
    }

    // 配置モデル（子猫）からカメラへの方向のベクトルを求める
    Vector3 GetLookVector(Vector3 position)
    {
        // 2点間の位置の差分をとって方向ベクトルを求める
        Vector3 lookVector = arSession.camera.transform.position - position;
        // 床面の上（XZ平面）のみを回転の対象とするため、上下方向（Y軸）の差分は無視する
        lookVector.y = 0.0f;
        lookVector.Normalize();
        return lookVector;
    }

    // 配置オブジェクトの向きとカメラへの方向をチェックして回転に必要な値を求める
    void CheckObjDirection()
    {
        if (!isCloudMaster)
        {
            // Cloud Anchor未使用、もしくは
            // Cloud Anchor登録側の時のみ向きを変えるアニメーションを発生させる
            return;
        }

        if (!CheckTemper())
        {
            // 子猫の機嫌が良くない場合は向きを変えるアニメーションをしない
            return;
        }
        // 配置オブジェクトの向きのベクトルを得る
        Vector3 catDirVector = rb.transform.forward;
        // 配置オブジェクトからカメラへの方向のベクトルを得る
        Vector3 lookVector = GetLookVector(rb.position);
        // 配置オブジェクトの向きとカメラへの方向のベクトルの内積を得る
        float dot = Vector3.Dot(catDirVector, lookVector);
        // ２つのベクトルのなす角が60°以上なら回転が必要とする
        if (dot <= 0.5f)
        {
            // 配置オブジェクトの現在の向きを回転開始値とする
            rotateFrom = rb.rotation;
            // 配置オブジェクトからカメラへの方向を回転終了値とする
            rotateTo = Quaternion.LookRotation(lookVector);
            // 回転アニメーション残り時間に回転所要時間に回転を始めるまでの時間を加えてセットする　
            // （回転を始めるまでの時間分遅れて回転を始めるため）
            rotateDelta = rotateDuration + delayTime;
            SyncRotateObjCommand(rotateFrom, rotateTo, DateTime.UtcNow.AddSeconds(delayTime));
        }
    }

    // 子猫の回転情報を同期する
    void SyncRotateObjCommand(Quaternion from, Quaternion to, DateTime startTime)
    {
        if (dbRef != null)
        {
            RotateObject rotObj = new RotateObject(from, to, startTime);
            dbRef.Child("kitten").Child(RoomID).Child("rotateObj").SetRawJsonValueAsync(JsonUtility.ToJson(rotObj));
        }
    }

    bool CheckTemper()
    {
        // 子猫の機嫌が良く、かつ動いていない場合に処理を行う
        if (CatPreferences.IsGoodTemper())
        {
            // 機嫌が良いので次にシャカシャカ掻くまでの時間を延長する
            nextItchingTime = Time.time + itchingDuration;
            // メインカメラの位置（端末を持っているプレイヤーの位置）
            Vector3 cameraPos = Camera.main.transform.position;
            // 取得した位置の高さを子猫の位置の高さに合わせる
            cameraPos.y = rb.transform.position.y;
            // 子猫とプレイヤーの距離を求める
            float dist = (cameraPos - rb.transform.position).magnitude;
            // 距離が1mより大きければ行う処理
            if (dist > 1.0f)
            {
                // 少し遅らせたタイミングでプレイヤーの手前まで移動させる
                Invoke("MoveToCameraPosition", 3.0f);
                return false;
            }
            return true;
        }
        else
        {
            // 経過時間が次にシャカシャカ掻くまでの時間よりも大きい時に処理を行う
            if (Time.time > nextItchingTime)
            {
                SyncAnimation("Itching");
                // 機嫌が良くない時のアニメーション（シャカシャカする）に遷移する
                animator.SetTrigger("Itching");
                // 次にシャカシャカ掻くまでの時間をセット
                nextItchingTime = Time.time + itchingDuration;
                // アニメーション名の更新をDBに発生させるために空の値を設定しておく
                SyncAnimation("");
            }
            return false;
        }
    }

    // プレイヤーの手前の位置に移動させる
    void MoveToCameraPosition()
    {
        // メインカメラの位置（端末を持っているプレイヤーの位置）
        Vector3 cameraPos = Camera.main.transform.position;
        // 取得した位置の高さを子猫の位置の高さに合わせる
        cameraPos.y = rb.transform.position.y;
        // プレイヤーの手前（距離が-0.5の位置）まで移動させる
        MoveTo(cameraPos, -0.5f);
    }

    // 子猫の移動情報を同期する
    void SyncMoveToCommand(Vector3 pos, float offset)
    {
        if (dbRef != null)
        {
            Vector3 childPos = pos - cloudAnchor.transform.position;
            // 4次元ベクトルのw成分にオフセットを設定する
            Vector4 posAndOffset = new Vector4(childPos.x, childPos.y, childPos.z, offset);
            dbRef.Child("kitten").Child(RoomID).Child("moveTo").SetRawJsonValueAsync(JsonUtility.ToJson(posAndOffset));
        }
    }

    // アニメーションをリセットする
    void ResetRotateAnim()
    {
        // 回転アニメーション残り時間を0にすると回転は行わない
        rotateDelta = 0.0f;
        // アイドル状態のアニメーションに戻す
        animator.SetFloat("MoveSpeed", 0.0f);
    }

    // カメラの方向に回転する
    void RotateToCamera()
    {
        // 回転アニメーション残り時間が回転所要時間の値以下の場合は回転する
        if (rotateDelta <= rotateDuration)
        {
            animator.SetFloat("MoveSpeed", 0.2f);
            // 回転アニメーション残り時間を経過時間(0.0〜1.0)に正規化する
            float t = 1.0f - (rotateDelta / rotateDuration);
            // 経過時間(0.0〜1.0）の回転開始値から回転終了値の間の補間値を配置モデルにセットする
            rb.rotation = Quaternion.Slerp(rotateFrom, rotateTo, t);
        }
    }

    // 一定時間ごとに呼び出される
    private void FixedUpdate()
    {
        // 子猫が目的の位置まで移動するのにかかる時間が0になるまで動かす
        if (arrivalTime > 0.0f)
        {
            // 子猫が目的の位置まで移動するのにかかる時間を経過時間分減らす
            arrivalTime -= Time.deltaTime;
            if (arrivalTime < Mathf.Epsilon)
            {
                // 子猫が目的の位置まで移動するのにかかる時間が0になったら移動をやめる
                ResetRotateAnim();
                isMoving = false;
            }
        }
    }

    // 指定位置に子猫を移動させる
    public void MoveTo(Vector3 pos, float offset = 0.0f)
    {
        Vector3 planePos = pos;
        // 水平方向は現在位置のままにする
        planePos.y = rb.transform.position.y;
        // 子猫の向きを移動先の方に向ける
        rb.transform.LookAt(planePos);
        // 現在位置から移動先までのベクトルを求める
        Vector3 distanceVec = planePos - rb.transform.position;
        // 現在位置から移動先までの距離を求める
        float distance = distanceVec.magnitude + offset;
        // 移動中フラグを立てる
        isMoving = true;
        // 移動する距離が1mを超える場合は走るようにする
        if (distance > 1.0f)
        {
            // 移動スピード（走る）を設定
            speed = 0.7f;
        }
        else
        {
            // 移動スピード（歩く）を設定
            speed = 0.2f;
        }
        animator.SetFloat("MoveSpeed", speed);
        // 指定位置まで移動するのにかかる時間を求める
        arrivalTime = distance / speed;
    }

    // タッチされた先に平面があるか判定する
    // touchPosition ... タッチされた画面上の2D座標
    // hitPose ... 画面をタッチした先に伸ばしたレイと平面が衝突した位置と姿勢
    bool HitTest(Vector2 touchPosition, out Pose hitPose)
    {
        if (useAR)
        { // ARを使用する場合
            // 画面をタッチした先に伸ばしたレイと平面の衝突判定
            if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
            { // 衝突する平面があった場合
                // 1つ目に衝突した平面と交差する位置と姿勢の情報を取得
                hitPose = hits[0].pose;
                return true;
            }
        }
        else
        { // ARを使用しない場合（エディターのプレイモード用）
            // タッチ位置から伸びるレイを生成
            Ray ray = arSession.camera.ScreenPointToRay(touchPosition);
            RaycastHit hit;

            // レイが衝突するオブジェクトを検出する
            if (Physics.Raycast(ray, out hit))
            {
                // 衝突した位置と姿勢を
                // ARRaycastManagerが返す形式に合わせる
                hitPose = new Pose();
                hitPose.position = hit.point;
                hitPose.rotation = hit.transform.rotation;
                Debug.DrawRay(ray.origin, ray.direction, Color.red, 3);
                return true;
            }
        }
        hitPose = default;
        return false;
    }
}
