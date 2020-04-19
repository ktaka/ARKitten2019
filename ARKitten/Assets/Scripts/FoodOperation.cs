using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodOperation : MonoBehaviour
{
    public float duration = 2.0f; // オブジェクトを消すまでの時間（秒）

    // オブジェクトが接触した時に呼び出される
    void OnTriggerEnter(Collider co)
    {
        // ごはんをあげた時間を記録する
        CatPreferences.SaveLastFeedTime();
        // 指定した時間後に自分自身のオブジェクトを削除する
        Destroy(gameObject, duration);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
