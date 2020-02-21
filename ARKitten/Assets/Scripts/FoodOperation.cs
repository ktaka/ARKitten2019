using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodOperation : MonoBehaviour
{
    public float duration = 2.0f;

    void OnTriggerEnter(Collider co)
    {
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
