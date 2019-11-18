using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallOperation : MonoBehaviour
{
    public PlaceObject placeObject;
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
        if (collisionCount > 1)
        {
            placeObject.MoveTo(transform.position);
            collisionCount = 0;
        }
    }
}
