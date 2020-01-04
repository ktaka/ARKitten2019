using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
// エディタで実行時のみ有効なコード
#if UNITY_EDITOR
        // 左シフトキーを押しているとき
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // マウスの縦と横の動きを回転に割り当てる
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");
            transform.Rotate(y, -x, 0);
        }
#endif           
    }
}
