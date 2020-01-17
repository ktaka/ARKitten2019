// based on a script from https://github.com/Unity-Technologies/arfoundation-samples/tree/2.1

using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARPlane))]
[RequireComponent(typeof(Animator))]
public class FadePlaneOnBoundaryChange : MonoBehaviour
{
    // 検出平面をフェードオフして消す制御用のパラメータ名
    const string k_FadeOffAnim = "FadeOff";
    // 検出平面をフェードオンして表示する制御用のパラメータ名
    const string k_FadeOnAnim = "FadeOn";
    // 検出平面を表示している時間
    const float k_TimeOut = 2.0f;
    
    Animator m_Animator;
    ARPlane m_Plane;

    float m_ShowTime = 0;  // 検出平面表示を消すまでの残り時間
    bool m_UpdatingPlane = false;

    // オブジェクトが有効になった時に呼び出される
    void OnEnable()
    {
        m_Plane = GetComponent<ARPlane>();
        m_Animator = GetComponent<Animator>();

        // 平面の境界が変化した際に呼び出されるようコールバックを登録する
        m_Plane.boundaryChanged += PlaneOnBoundaryChanged;
    }

    // オブジェクトが無効になった時に呼び出される
    void OnDisable()
    {
        // 平面の境界が変化した際に呼び出されるよう登録したコールバックを解除する
        m_Plane.boundaryChanged -= PlaneOnBoundaryChanged;
    }

    // 毎フレーム呼び出される
    void Update()
    {
        if (m_UpdatingPlane)
        {
            // 検出平面表示を消すまでの残り時間から、経過した時間を減らす
            m_ShowTime -= Time.deltaTime;

            if (m_ShowTime <= 0)
            {
                // 消すまでの残り時間が0になったら
                // フェードオフのアニメーションを行い検出平面を消す
                m_UpdatingPlane = false;
                m_Animator.SetBool(k_FadeOffAnim, true);
                m_Animator.SetBool(k_FadeOnAnim, false);
            }
        }
    }

    // 平面の境界が変化した際に呼び出されるよう登録するコールバック
    void PlaneOnBoundaryChanged(ARPlaneBoundaryChangedEventArgs obj)
    {
        // フェードオンのアニメーションを行い検出平面を表示する
        m_Animator.SetBool(k_FadeOffAnim, false);
        m_Animator.SetBool(k_FadeOnAnim, true);
        m_UpdatingPlane = true;
        // 検出平面表示を消すまでの残り時間をセットする
        m_ShowTime = k_TimeOut;
    }
}