using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// AR Foundationを使用する際は次の2つのusingを追加する
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class FoodControl : MonoBehaviour
{
    public bool useAR = true; // AR使用フラグ（プレイモードで実行する際はfalseにする）
    public float yAxisOffset = 0.042f; // 配置時のY軸（高さ）方向オフセット（ごはんのオブジェクトが配置面にめり込まないようにする）
    public GameObject foodPrefab; // 配置用モデルのプレハブ
    public PlaceObject placeObject; // 子猫の配置用オブジェクト

    GameObject spawnedObject; // 配置モデルのプレハブから生成されたオブジェクト
    // ARRaycastManagerは画面をタッチした先に伸ばしたレイと平面の衝突を検知する
    ARRaycastManager raycastManager;
    ARSessionOrigin arSession;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    // 画面がタッチされた際にUIManagerから呼び出される
    public void OnTouch(Vector2 touchPosition)
    {
        if (HitTest(touchPosition, out Pose hitPose))
        { // タッチした先に平面がある場合
            // オブジェクトが配置面にめり込まないようにする
            hitPose.position.y += yAxisOffset;
            if (spawnedObject == null)
            { // 配置用モデルが未生成の場合
                // プレハブから配置用モデルを生成し、レイが平面に衝突した位置に配置する
                spawnedObject = Instantiate(foodPrefab, hitPose.position, Quaternion.identity);
            }
            else
            {
                spawnedObject.transform.parent = null;
                spawnedObject.transform.position = hitPose.position;
            }
            // 子猫をボールの衝突位置まで動かす
            placeObject.MoveTo(spawnedObject.transform.position);
            placeObject.WriteFoodInfo(spawnedObject);
        }
    }

    public void placeWithAnchor(Transform parent, Vector3 localPos)
    {
        if (spawnedObject == null)
        { // 配置用モデルが未生成の場合
          // プレハブから配置用モデルを生成し、レイが平面に衝突した位置に配置する
            spawnedObject = Instantiate(foodPrefab, Vector3.zero, Quaternion.identity);
        }
        spawnedObject.transform.parent = null; // 前に設定したTransformの親を外す
        spawnedObject.transform.localPosition = localPos;
        spawnedObject.transform.SetParent(parent, false);
        //localPos.y += yAxisOffset;
        //spawnedObject.transform.position = localPos;
        // 子猫をボールの衝突位置まで動かす
        placeObject.MoveTo(spawnedObject.transform.position);
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

    // 最初に1回だけ呼び出される（最初のUpdateの前）
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
        GameObject arSessionObject = GameObject.Find("AR Session Origin");
        raycastManager = arSessionObject.GetComponent<ARRaycastManager>();
        // 関連付けられたCameraオブジェクトを使用するためARSessionOriginを取得する
        arSession = arSessionObject.GetComponent<ARSessionOrigin>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
