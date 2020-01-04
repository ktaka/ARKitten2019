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

    private float yPosition; // ボールを削除するY軸の閾値
    private Rigidbody rb; // ボールのリジッドボディ
    private Vector3 startPosition; // ドラッグ開始位置
    private Vector3 screenPos; // 開始時のスクリーン座標の保存用
    private float beginDragTime; // ドラッグ開始時間

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // 重力の影響を受けないようにする
        rb.useGravity = false;
        // ファークリップ面までの距離をボールを削除する高さ（Y軸の閾値）とする
        yPosition = -Camera.main.farClipPlane;
    }

    // Update is called once per frame
    void Update()
    {
        // ボールの高さがY軸の閾値より低くなったら削除する
        if ((rb.position.y - yPosition) < 0)
        {
            Destroy(gameObject, 1);
        }
    }

    // オブジェクトをタップ
    public void OnPointerDown(PointerEventData eventData)
    {
        // ボールのワールド座標（3D空間）での位置をスクリーン座標（画面上の位置）に変換する
        screenPos = Camera.main.WorldToScreenPoint(transform.position);
        // 現在のボールの位置をドラッグ開始位置として保存する
        startPosition = transform.position;
        // 重力の影響を受けないようにする
        rb.useGravity = false;
        // 速度とかかる力を０にする
        rb.velocity = Vector3.zero;
        rb.AddForce(Vector3.zero);
        collisionCount = 0;
    }

    // オブジェクトをドラッグ(フリック)開始
    public void OnBeginDrag(PointerEventData data)
    {
        // 現在の時間をドラッグ開始時間として保存する
        beginDragTime = Time.time;
    }

    // オブジェクトをドラッグしている間
    public void OnDrag(PointerEventData eventData)
    {
        // ドラッグ中の現在位置（スクリーン座標）を取得する
        Vector3 scrPos = eventData.position;
        // ドラッグ開始時の画面からの距離をセットする
        scrPos.z = screenPos.z;
        // 現在のドラッグ位置のスクリーン座標をワールド座標（3D空間の座標）に変換する
        Vector3 pos = Camera.main.ScreenToWorldPoint(scrPos);
        // 変換したワールド座標をボールの座標としてセットする
        transform.position = pos;
    }

    // オブジェクトのドラッグ終了(指を離した)
    public void OnEndDrag(PointerEventData eventData)
    {
        // ドラッグ開始時から動かしたベクトルを求める
        Vector3 moved = transform.position - startPosition;
        // 現在の時間とドラッグ開始時の時間から動かすのにかかった時間を求める
        // 10をかけて移動時間の効果を調整して係数とする
        float dragFactor = (Time.time - beginDragTime) * 10;
        // 移動ベクトルに係数をかけたものをボールに加える力とする
        rb.AddForce(100.0f / dragFactor * moved);
        // 重力の影響を受けるようにする
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
