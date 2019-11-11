using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// AR Foundationを使用する際は次の2つのusingを追加する
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceObject : MonoBehaviour
{
    // public変数はUnityのエディタのインスペクターで設定項目として表示される
    public GameObject placedPrefab; // 配置用モデルのプレハブ
    public bool useAR = true; // AR使用フラグ（プレイモードで実行する際はfalseにする）
    public GameObject floorPlane; // プレイモード用の床面
    public float rotateDuration = 3.0f; // 回転所要時間
    public float delayTime = 3.0f; // 回転を始めるまでの時間
    
    GameObject spawnedObject; // 配置モデルのプレハブから生成されたオブジェクト
    // ARRaycastManagerは画面をタッチした先に伸ばしたレイと平面の衝突を検知する
    ARRaycastManager raycastManager;
    ARSessionOrigin arSession;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    Quaternion rotateFrom; // 回転開始値
    Quaternion rotateTo; // 回転終了値
    float rotateDelta; // 回転アニメーション残り時間
    Animator animator;
    Rigidbody rb;
    bool isMoving = false;
    float arrivalTime;
    float speed;

    // 起動時に1度呼び出される
    void Start()
    {
        // オブジェクトに追加されているARRaycastManagerコンポーネントを取得
        raycastManager = GetComponent<ARRaycastManager>();

        // プレイモード用床面が設定されていて、AR使用フラグがOFFの場合は床面を表示
        if (floorPlane != null)
        {
            floorPlane.SetActive(!useAR);
        }
        // 関連付けられたCameraオブジェクトを使用するためARSessionOriginを取得する
        arSession = GetComponent<ARSessionOrigin>();
    }

    // フレーム毎に呼び出される
    void Update()
    {
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

        // タッチされていない場合は処理をぬける
        if (!TryGetTouchPosition(out Vector2 touchPosition))
        {
            return;
        }

        if (HitTest(touchPosition, out Pose hitPose))
        { // タッチした先に平面がある場合
            // モデル（子猫）を配置する位置からカメラへの方向のベクトルを求めて
            // モデルをどのくらい回転させるか求める
            Quaternion rotation = Quaternion.LookRotation(GetLookVector(hitPose.position));
            if (spawnedObject == null)
            { // 配置用モデルが未生成の場合
                // プレハブから配置用モデルを生成し、レイが平面に衝突した位置に配置する
                spawnedObject = Instantiate(placedPrefab, hitPose.position, rotation);
                animator = spawnedObject.GetComponent<Animator>();
                rb = spawnedObject.GetComponent<Rigidbody>();
                rb.position = hitPose.position;
            }
            else
            { // 配置するモデルが生成済みの場合
                // 配置用モデルの位置をレイが平面に衝突した位置にする
                rb.position = hitPose.position;
                // 配置用モデルを回転させてカメラの方に向ける
                rb.rotation = rotation;
            }
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
        }
    }

    // 回転アニメーションをリセットする
    void ResetRotateAnim()
    {
        // 回転アニメーション残り時間を0にすると回転は行わない
        rotateDelta = 0.0f;
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

    // タッチ位置を取得する
    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
#if UNITY_EDITOR
        // Unityエディターで実行される場合
        if (Input.GetMouseButtonDown(1))
        {
            // マウスボタンが押された位置を取得する
            var mousePosition = Input.mousePosition;
            touchPosition = new Vector2(mousePosition.x, mousePosition.y);
            return true;
        }
#else
        // スマートフォンで実行される場合
        if (Input.touchCount == 2)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) {
                // 画面がタッチされた位置を取得する
                touchPosition = touch.position;
                return true;
            }
        }
#endif
        touchPosition = default;
        return false;
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

    private void FixedUpdate()
    {
        if (arrivalTime > 0.0f)
        {
            arrivalTime -= Time.deltaTime;
            if (arrivalTime < Mathf.Epsilon)
            {
                ResetRotateAnim();
                isMoving = false;
            }
        }
    }

    public void MoveTo(Vector3 pos)
    {
        Vector3 planePos = pos;
        planePos.y = rb.transform.position.y;
        rb.transform.LookAt(planePos);
        Vector3 distanceVec = planePos - rb.transform.position;
        float distance = distanceVec.magnitude;
        isMoving = true;
        speed = 0.2f;
        animator.SetFloat("MoveSpeed", speed);
        arrivalTime = distance / speed;
    }
}
