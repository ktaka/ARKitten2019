using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CatControl : MonoBehaviour, IDragHandler
{
    public int strokingThreshold = 50;
    int strokingNum;

    public void OnDrag(PointerEventData eventData)
    {
        strokingNum++;
        if (strokingNum > strokingThreshold)
        {
            GetComponent<Animator>().SetTrigger("Sit");
            strokingNum = 0;
        }
    }

    void OnTriggerEnter(Collider co)
    {
        GetComponent<Animator>().SetFloat("MoveSpeed", 0.0f);
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.layer = 8;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
