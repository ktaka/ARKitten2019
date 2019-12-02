using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BallOperation : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // 子猫の配置用オブジェクト
    public PlaceObject placeObject;
    // 衝突回数のカウンター
    int collisionCount;

    private float yPosition; // 
    private Rigidbody rb;
    private Vector3 startPosition;
    private Vector3 screenPos;
    private float beginDragTime;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        yPosition = -Camera.main.farClipPlane;
    }

    // Update is called once per frame
    void Update()
    {
        if ((rb.position.y - yPosition) < 0)
        {
            Destroy(gameObject, 1);
        }
    }

    // オブジェクトをタップ
    public void OnPointerDown(PointerEventData eventData)
    {
        screenPos = Camera.main.WorldToScreenPoint(transform.position);
        startPosition = transform.position;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.AddForce(Vector3.zero);
        collisionCount = 0;
    }

    // オブジェクトをドラッグ(フリック)開始
    public void OnBeginDrag(PointerEventData data)
    {
        beginDragTime = Time.time;
    }

    // オブジェクトをドラッグしている間
    public void OnDrag(PointerEventData eventData)
    {
        Vector3 scrPos = eventData.position;
        scrPos.z = screenPos.z;
        Vector3 pos = Camera.main.ScreenToWorldPoint(scrPos);
        transform.position = pos;
    }

    // オブジェクトのドラッグ終了(指を離した)
    public void OnEndDrag(PointerEventData eventData)
    {
        Vector3 moved = transform.position - startPosition;
        float dragFactor = (Time.time - beginDragTime) * 10;
        rb.AddForce(100.0f / dragFactor * moved);
        rb.useGravity = true;
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
