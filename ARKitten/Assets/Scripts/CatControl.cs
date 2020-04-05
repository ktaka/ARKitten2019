using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // イベントを扱うため追加

// ドラッグイベントを受け取れるようにするためにIDragHandlerインターフェースを追加
public class CatControl : MonoBehaviour, IDragHandler
{
    public int strokingThreshold = 50; // なでたと判定するドラッグ回数の閾値
    int strokingNum; // ドラッグした回数のカウント用

    // ドラッグイベントが発生した時に呼び出される
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
        // ドラッグ回数を数える
        strokingNum++;
        // ドラッグ回数が閾値を超えた際はなでたと判定する
        if (strokingNum > strokingThreshold)
        {
            // 座るアニメーションに遷移する
            GetComponent<Animator>().SetTrigger("Sit");
            // ドラッグ回数は初期化する
            strokingNum = 0;
            // なでた回数を増やして記録する
            CatPreferences.addStrokingNum();
        }
    }

    // オブジェクトが接触した時に呼び出される
    void OnTriggerEnter(Collider co)
    {
        // 走る or 歩くアニメーションと移動を止める
        GetComponent<Animator>().SetFloat("MoveSpeed", 0.0f);
    }

    // 最初に1回だけ呼び出される（最初のUpdateの前）
    void Start()
    {
        // 生成されたオブジェクトのレイヤーを8（AR Object）にする
        gameObject.layer = 8;
    }

    // 1フレームに1回呼び出される
    void Update()
    {
        
    }
}
