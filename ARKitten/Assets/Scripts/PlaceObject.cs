﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// AR Foundationを使用する際は次の2つのusingを追加する
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceObject : MonoBehaviour
{
    // public変数はUnityのエディタのインスペクターで設定項目として表示される
    public GameObject placedPrefab; // 配置用モデルのプレハブ
    public bool useAR = true; // AR使用フラグ（プレイモードで実行する際はfalseにする）
    public GameObject floorPlane; // プレイモード用の床面
    public float rotateDuration = 3.0f; // 回転所要時間
    public float delayTime = 3.0f; // 回転を始めるまでの時間
    public float itchingDuration = 20.0f; // シャカシャカ掻く間隔の時間（秒）

    GameObject spawnedObject; // 配置モデルのプレハブから生成されたオブジェクト
    // ARRaycastManagerは画面をタッチした先に伸ばしたレイと平面の衝突を検知する
    ARRaycastManager raycastManager;
    ARSessionOrigin arSession;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    Quaternion rotateFrom; // 回転開始値
    Quaternion rotateTo; // 回転終了値
    float rotateDelta; // 回転アニメーション残り時間
    Animator animator; // 子猫のアニメーター
    Rigidbody rb; // 子猫のリジッドボディ
    bool isMoving = false; // 子猫の移動中を示すフラグ
    float arrivalTime; // 子猫が目的の位置まで移動するのにかかる時間
    float speed; // 子猫の移動スピード
    float nextItchingTime; // 次にシャカシャカ掻くまでの時間（秒）

    // オブジェクト配置時に呼び出すコールバック
    public static event Action onPlacedObject;

    // 起動時に1度呼び出される
    void Start()
    {
#if UNITY_EDITOR
        // エディタで実行する際はAR使用フラグをOFFにする
        useAR = false;
#else
        // 端末で実行する際はAR使用フラグをONにする
        useAR = true;
#endif
        // オブジェクトに追加されているARRaycastManagerコンポーネントを取得
        raycastManager = GetComponent<ARRaycastManager>();

        // プレイモード用床面が設定されていて、AR使用フラグがOFFの場合は床面を表示
        if (floorPlane != null)
        {
            floorPlane.SetActive(!useAR);
        }
        // 関連付けられたCameraオブジェクトを使用するためARSessionOriginを取得する
        arSession = GetComponent<ARSessionOrigin>();
        // 次にシャカシャカ掻くまでの時間をセット
        nextItchingTime = Time.time + itchingDuration;
    }

    // フレーム毎に呼び出される
    void Update()
    {
        // 移動中フラグが立っている場合は回転しない
        if (spawnedObject != null && isMoving == false)
        {
            // 回転アニメーション残り時間が0より大きい場合は回転させる
            if (rotateDelta <= 0.0f)
            {
                // 回転アニメーションをリセットする
                ResetRotateAnim();
                // 配置オブジェクトの向きとカメラへの方向をチェックして回転に必要な値を求める
                CheckObjDirection();
            }
            else
            {
                // 配置オブジェクトをカメラの方に回転させる
                RotateToCamera();
                // 回転アニメーション残り時間を求める
                rotateDelta -= Time.deltaTime;
            }
        }
    }

    // 画面がタッチされた際にUIManagerから呼び出される
    public void OnTouch(Vector2 touchPosition)
    {
        if (HitTest(touchPosition, out Pose hitPose))
        { // タッチした先に平面がある場合
            // モデル（子猫）を配置する位置からカメラへの方向のベクトルを求めて
            // モデルをどのくらい回転させるか求める
            Quaternion rotation = Quaternion.LookRotation(GetLookVector(hitPose.position));
            if (spawnedObject == null)
            { // 配置用モデルが未生成の場合
                // プレハブから配置用モデルを生成し、レイが平面に衝突した位置に配置する
                spawnedObject = Instantiate(placedPrefab, hitPose.position, rotation);
                // 子猫のアニメーターを取得
                // （歩く、アイドルのアニメーションを制御するため）
                animator = spawnedObject.GetComponent<Animator>();
                // 子猫のリジッドボディを取得
                // （位置や回転を制御するため）
                rb = spawnedObject.GetComponent<Rigidbody>();
                // 
                if (onPlacedObject != null)
                {
                    // オブジェクトが配置されたことを知らせるコールバックを呼び出す
                    onPlacedObject();
                }
            }
            else
            { // 配置するモデルが生成済みの場合
                // 配置用モデルの位置をレイが平面に衝突した位置にする
                rb.position = hitPose.position;
                // 配置用モデルを回転させてカメラの方に向ける
                rb.rotation = rotation;
            }
        }
    }

    // 配置モデル（子猫）からカメラへの方向のベクトルを求める
    Vector3 GetLookVector(Vector3 position)
    {
        // 2点間の位置の差分をとって方向ベクトルを求める
        Vector3 lookVector = arSession.camera.transform.position - position;
        // 床面の上（XZ平面）のみを回転の対象とするため、上下方向（Y軸）の差分は無視する
        lookVector.y = 0.0f;
        lookVector.Normalize();
        return lookVector;
    }

    // 配置オブジェクトの向きとカメラへの方向をチェックして回転に必要な値を求める
    void CheckObjDirection()
    {
        if (!CheckTemper())
        {
            // 子猫の機嫌が良くない場合は向きを変えるアニメーションをしない
            return;
        }
        // 配置オブジェクトの向きのベクトルを得る
        Vector3 catDirVector = rb.transform.forward;
        // 配置オブジェクトからカメラへの方向のベクトルを得る
        Vector3 lookVector = GetLookVector(rb.position);
        // 配置オブジェクトの向きとカメラへの方向のベクトルの内積を得る
        float dot = Vector3.Dot(catDirVector, lookVector);
        // ２つのベクトルのなす角が60°以上なら回転が必要とする
        if (dot <= 0.5f)
        {
            // 配置オブジェクトの現在の向きを回転開始値とする
            rotateFrom = rb.rotation;
            // 配置オブジェクトからカメラへの方向を回転終了値とする
            rotateTo = Quaternion.LookRotation(lookVector);
            // 回転アニメーション残り時間に回転所要時間に回転を始めるまでの時間を加えてセットする　
            // （回転を始めるまでの時間分遅れて回転を始めるため）
            rotateDelta = rotateDuration + delayTime;
        }
    }

    bool CheckTemper()
    {
        // 子猫の機嫌が良く、かつ動いていない場合に処理を行う
        if (CatPreferences.IsGoodTemper())
        {
            // 機嫌が良いので次にシャカシャカ掻くまでの時間を延長する
            nextItchingTime = Time.time + itchingDuration;
            // メインカメラの位置（端末を持っているプレイヤーの位置）
            Vector3 cameraPos = Camera.main.transform.position;
            // 取得した位置の高さを子猫の位置の高さに合わせる
            cameraPos.y = rb.transform.position.y;
            // 子猫とプレイヤーの距離を求める
            float dist = (cameraPos - rb.transform.position).magnitude;
            // 距離が1mより大きければ行う処理
            if (dist > 1.0f)
            {
                // 少し遅らせたタイミングでプレイヤーの手前まで移動させる
                Invoke("MoveToCameraPosition", 3.0f);
                return false;
            }
            return true;
        }
        else
        {
            // 経過時間が次にシャカシャカ掻くまでの時間よりも大きい時に処理を行う
            if (Time.time > nextItchingTime)
            {
                // 機嫌が良くない時のアニメーション（シャカシャカする）に遷移する
                animator.SetTrigger("Itching");
                // 次にシャカシャカ掻くまでの時間をセット
                nextItchingTime = Time.time + itchingDuration;
            }
            return false;
        }
    }

    // プレイヤーの手前の位置に移動させる
    void MoveToCameraPosition()
    {
        // メインカメラの位置（端末を持っているプレイヤーの位置）
        Vector3 cameraPos = Camera.main.transform.position;
        // 取得した位置の高さを子猫の位置の高さに合わせる
        cameraPos.y = rb.transform.position.y;
        // プレイヤーの手前（距離が-0.5の位置）まで移動させる
        MoveTo(cameraPos, -0.5f);
    }

    // アニメーションをリセットする
    void ResetRotateAnim()
    {
        // 回転アニメーション残り時間を0にすると回転は行わない
        rotateDelta = 0.0f;
        // アイドル状態のアニメーションに戻す
        animator.SetFloat("MoveSpeed", 0.0f);
    }

    // カメラの方向に回転する
    void RotateToCamera()
    {
        // 回転アニメーション残り時間が回転所要時間の値以下の場合は回転する
        if (rotateDelta <= rotateDuration)
        {
            animator.SetFloat("MoveSpeed", 0.2f);
            // 回転アニメーション残り時間を経過時間(0.0〜1.0)に正規化する
            float t = 1.0f - (rotateDelta / rotateDuration);
            // 経過時間(0.0〜1.0）の回転開始値から回転終了値の間の補間値を配置モデルにセットする
            rb.rotation = Quaternion.Slerp(rotateFrom, rotateTo, t);
        }
    }

    // 一定時間ごとに呼び出される
    private void FixedUpdate()
    {
        // 子猫が目的の位置まで移動するのにかかる時間が0になるまで動かす
        if (arrivalTime > 0.0f)
        {
            // 子猫が目的の位置まで移動するのにかかる時間を経過時間分減らす
            arrivalTime -= Time.deltaTime;
            if (arrivalTime < Mathf.Epsilon)
            {
                // 子猫が目的の位置まで移動するのにかかる時間が0になったら移動をやめる
                ResetRotateAnim();
                isMoving = false;
            }
        }
    }

    // 指定位置に子猫を移動させる
    public void MoveTo(Vector3 pos, float offset = 0.0f)
    {
        Vector3 planePos = pos;
        // 水平方向は現在位置のままにする
        planePos.y = rb.transform.position.y;
        // 子猫の向きを移動先の方に向ける
        rb.transform.LookAt(planePos);
        // 現在位置から移動先までのベクトルを求める
        Vector3 distanceVec = planePos - rb.transform.position;
        // 現在位置から移動先までの距離を求める
        float distance = distanceVec.magnitude + offset;
        // 移動中フラグを立てる
        isMoving = true;
        // 移動する距離が1mを超える場合は走るようにする
        if (distance > 1.0f)
        {
            // 移動スピード（走る）を設定
            speed = 0.7f;
        }
        else
        {
            // 移動スピード（歩く）を設定
            speed = 0.2f;
        }
        animator.SetFloat("MoveSpeed", speed);
        // 指定位置まで移動するのにかかる時間を求める
        arrivalTime = distance / speed;
    }

    // タッチされた先に平面があるか判定する
    // touchPosition ... タッチされた画面上の2D座標
    // hitPose ... 画面をタッチした先に伸ばしたレイと平面が衝突した位置と姿勢
    bool HitTest(Vector2 touchPosition, out Pose hitPose)
    {
        if (useAR)
        { // ARを使用する場合
            // 画面をタッチした先に伸ばしたレイと平面の衝突判定
            if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
            { // 衝突する平面があった場合
                // 1つ目に衝突した平面と交差する位置と姿勢の情報を取得
                hitPose = hits[0].pose;
                return true;
            }
        }
        else
        { // ARを使用しない場合（エディターのプレイモード用）
            // タッチ位置から伸びるレイを生成
            Ray ray = arSession.camera.ScreenPointToRay(touchPosition);
            RaycastHit hit;

            // レイが衝突するオブジェクトを検出する
            if (Physics.Raycast(ray, out hit))
            {
                // 衝突した位置と姿勢を
                // ARRaycastManagerが返す形式に合わせる
                hitPose = new Pose();
                hitPose.position = hit.point;
                hitPose.rotation = hit.transform.rotation;
                Debug.DrawRay(ray.origin, ray.direction, Color.red, 3);
                return true;
            }
        }
        hitPose = default;
        return false;
    }
}
