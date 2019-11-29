using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallOperation : MonoBehaviour
{
    // 子猫の配置用オブジェクト
    public PlaceObject placeObject;
    // 衝突回数のカウンター
    int collisionCount;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // オブジェクトの衝突があった際に呼び出される
    private void OnCollisionEnter(Collision collision)
    {
        collisionCount++;
        // 2回以上衝突した場合
        if (collisionCount > 1)
        {
            // 子猫をボールの衝突位置まで動かす
            placeObject.MoveTo(transform.position);
            collisionCount = 0;
        }
    }
}
