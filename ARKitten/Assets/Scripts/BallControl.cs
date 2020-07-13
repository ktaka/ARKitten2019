using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallControl : MonoBehaviour
{
    // ボールオブジェクトのプレハブ
    public GameObject ballObject;
    // 子猫の配置用オブジェクト
    public PlaceObject placeObject;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    // 画面がタッチされた際にUIManagerから呼び出される
    public void OnTouch(Vector2 touchPosition)
    {
        // 画面上のタッチ座標を取得
        Vector3 pos = touchPosition;
        // 画面上の2D座標（スクリーン座標）を画面の少し奥（カメラのニアクリップ面より先）に設定
        pos.z = Camera.main.nearClipPlane * 2.0f;
        // タップした位置から奥行き方向に伸びるレイを作成
        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit hit = new RaycastHit();
        // 伸ばしたレイにぶつかるオブジェクトがない場合
        if (Physics.Raycast(ray, out hit) == false || hit.rigidbody == null)
        {
            // スクリーン座標をワールド座標（３次元空間の中の位置）に変換
            var position = Camera.main.ScreenToWorldPoint(pos);
            // ボールのオブジェクトを生成
            GameObject obj = Instantiate(ballObject, position, Quaternion.identity);
            obj.layer = 8; // layer: AR Object
            // ボールのオブジェクトと子猫の配置オブジェクトを関連付ける
            // （ボールが平面に当たった時に子猫に反応する命令を出せるようにするため）
            obj.GetComponent<BallOperation>().placeObject = placeObject;
        }
    }

    public void placeWithAnchor(Transform parent, Vector3 localPos, Vector3 throwForce)
    {
        GameObject obj = Instantiate(ballObject, Vector3.zero, Quaternion.identity);
        obj.GetComponent<BallOperation>().placeObject = placeObject;
        obj.transform.localPosition = localPos;
        obj.transform.SetParent(parent, false);
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.AddForce(throwForce);
        rb.useGravity = true;
    }
}
