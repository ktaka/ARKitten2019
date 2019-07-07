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

    GameObject spawnedObject; // 配置モデルのプレハブから生成されたオブジェクト
    // ARRaycastManagerは画面をタッチした先に伸ばしたレイと平面の衝突を検知する
    ARRaycastManager raycastManager;
    ARSessionOrigin arSession;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

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
        // タッチされていない場合は処理をぬける
        if (!TryGetTouchPosition(out Vector2 touchPosition))
        {
            return;
        }

        if (HitTest(touchPosition, out Pose hitPose))
        { // タッチした先に平面がある場合
            if (spawnedObject == null)
            { // 配置用モデルが未生成の場合
                // プレハブから配置用モデルを生成し、レイが平面に衝突した位置に配置する
                spawnedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation);
            }
            else
            { // 配置するモデルが生成済みの場合
                // 配置用モデルの位置をレイが平面に衝突した位置にする
                spawnedObject.transform.position = hitPose.position;
            }
        }
    }

    // タッチ位置を取得する
    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
#if UNITY_EDITOR
        // Unityエディターで実行される場合
        if (Input.GetMouseButton(0))
        {
            // マウスボタンが押された位置を取得する
            var mousePosition = Input.mousePosition;
            touchPosition = new Vector2(mousePosition.x, mousePosition.y);
            return true;
        }
#else
        // スマートフォンで実行される場合
        if (Input.touchCount > 0)
        {
            // 画面がタッチされた位置を取得する
            touchPosition = Input.GetTouch(0).position;
            return true;
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
}
