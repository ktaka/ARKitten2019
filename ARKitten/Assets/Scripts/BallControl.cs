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
        // タッチされていない場合は処理をぬける
        if (!TryGetTouchPosition(out Vector2 touchPosition))
        {
            return;
        }
        // 画面上のタッチ座標を取得
        Vector3 pos = touchPosition;
        // 画面上の2D座標（スクリーン座標）を画面の少し奥（カメラのニアクリップ面より先）に設定
        pos.z = Camera.main.nearClipPlane * 2.0f;
        // スクリーン座標をワールド座標（３次元空間の中の位置）に変換
        var position = Camera.main.ScreenToWorldPoint(pos);
        // ボールのオブジェクトを生成
        GameObject obj = Instantiate(ballObject, position, Quaternion.identity);
        // ボールのオブジェクトと子猫の配置オブジェクトを関連付ける
        // （ボールが平面に当たった時に子猫に反応する命令を出せるようにするため）
        obj.GetComponent<BallOperation>().placeObject = placeObject;
    }

    // タッチ位置を取得する
    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
#if UNITY_EDITOR
        // Unityエディターで実行される場合
        if (Input.GetMouseButtonDown(0))
        {
            // マウスボタンが押された位置を取得する
            var mousePosition = Input.mousePosition;
            touchPosition = new Vector2(mousePosition.x, mousePosition.y);
            return true;
        }
#else
        // スマートフォンで実行される場合
        if (Input.touchCount == 1)
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
}
