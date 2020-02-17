// based on a script from https://github.com/Unity-Technologies/arfoundation-samples/tree/2.1

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The ARCameraManager which will produce frame events.")]
    ARCameraManager m_CameraManager;

    /// <summary>
    /// Get or set the <c>ARCameraManager</c>.
    /// </summary>
    public ARCameraManager cameraManager
    {
        get { return m_CameraManager; }
        set
        {
            if (m_CameraManager == value)
                return;

            if (m_CameraManager != null)
                m_CameraManager.frameReceived -= FrameChanged;

            m_CameraManager = value;

            if (m_CameraManager != null & enabled)
                m_CameraManager.frameReceived += FrameChanged;
        }
    }

    // アニメーションをフェードオフして消すトリガーとなるパラメータ名
    const string k_FadeOffAnim = "FadeOff";
    // アニメーションをフェードオンして表示するトリガーとなるパラメータ名
    const string k_FadeOnAnim = "FadeOn";

    [SerializeField]
    ARPlaneManager m_PlaneManager;

    public ARPlaneManager planeManager
    {
        get { return m_PlaneManager; }
        set { m_PlaneManager = value; }
    }

    [SerializeField]
    Animator m_MoveDeviceAnimation;

    public Animator moveDeviceAnimation
    {
        get { return m_MoveDeviceAnimation; }
        set { m_MoveDeviceAnimation = value; }
    }

    [SerializeField]
    Animator m_TapToPlaceAnimation;

    public Animator tapToPlaceAnimation
    {
        get { return m_TapToPlaceAnimation; }
        set { m_TapToPlaceAnimation = value; }
    }

    public PlaceObject placeObject;  // 子猫を配置するスクリプトコンポーネント
    public BallControl ballControl;  // ボールを配置するスクリプトコンポーネント

    static List<ARPlane> s_Planes = new List<ARPlane>();

    // "Tap to Place"（タップして配置）ガイドの表示中を示すフラグ
    bool m_ShowingTapToPlace = false;

    // "Move Device Slowly"（端末をゆっくり動かす）ガイドの表示中を示すフラグ
    bool m_ShowingMoveDevice = true;

    int selectedIdx;  // Dropdownで選択されたインデックス

    // 起動時に1度呼び出される
    void Start()
    {
#if UNITY_EDITOR // Unityのエディタで実行する場合
        if (moveDeviceAnimation)
            moveDeviceAnimation.SetTrigger(k_FadeOffAnim);

        // "Tap to Place" ガイドのアニメーションを表示する
        if (tapToPlaceAnimation)
            tapToPlaceAnimation.SetTrigger(k_FadeOnAnim);

        m_ShowingTapToPlace = true;
        m_ShowingMoveDevice = false;
#endif
    }

    // オブジェクトが有効になった時に呼び出される
    void OnEnable()
    {
        // カメラから新しいフレームを取得した際に呼び出されるコールバックとして登録する
        if (m_CameraManager != null)
            m_CameraManager.frameReceived += FrameChanged;

        // 子猫を配置した際に呼び出されるコールバックとして登録する
        PlaceObject.onPlacedObject += PlacedObject;
    }

    // オブジェクトが無効になった時に呼び出される
    void OnDisable()
    {
        // カメラから新しいフレームを取得した際のコールバックを解除する
        if (m_CameraManager != null)
            m_CameraManager.frameReceived -= FrameChanged;

        // 子猫を配置した際に呼び出されるコールバックを解除する
        PlaceObject.onPlacedObject -= PlacedObject;
    }

    // カメラから新しいフレームを取得した際に呼び出されるコールバック用
    void FrameChanged(ARCameraFrameEventArgs args)
    {
        // 平面を検出済みで且つ "Move Device" ガイドが表示中ならば
        if (PlanesFound() && m_ShowingMoveDevice)
        {
            // "Move Device" ガイドのアニメーションを消す
            if (moveDeviceAnimation)
                moveDeviceAnimation.SetTrigger(k_FadeOffAnim);

            // "Tap to Place" ガイドのアニメーションを表示する
            if (tapToPlaceAnimation)
                tapToPlaceAnimation.SetTrigger(k_FadeOnAnim);

            m_ShowingTapToPlace = true;
            m_ShowingMoveDevice = false;
        }
    }

    // 平面を検出したか判断する
    bool PlanesFound()
    {
        if (planeManager == null)
            return false;

        // 検出した平面の数が0より大きければ検出したと判断する
        return planeManager.trackables.count > 0;
    }

    // 子猫を配置した際に呼び出されるコールバック用
    void PlacedObject()
    {
        // "Tap to Place" ガイドが表示中ならば
        if (m_ShowingTapToPlace)
        {
            // "Tap to Place" ガイドのアニメーションを消す
            if (tapToPlaceAnimation)
                tapToPlaceAnimation.SetTrigger(k_FadeOffAnim);

            m_ShowingTapToPlace = false;
        }
    }

    // タッチ位置を取得する
    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
#if UNITY_EDITOR
        // Unityエディターで実行される場合
        if (Input.GetMouseButtonDown(0))
        {
            // UI要素をタップした際はAR空間のオブジェクトへのタップ通知が届かないようにする
            if (EventSystem.current.IsPointerOverGameObject())
            {
                touchPosition = default;
                Debug.Log("IsPointerOverGameObject");
                return false;
            }
            // マウスボタンが押された位置を取得する
            var mousePosition = Input.mousePosition;
            touchPosition = new Vector2(mousePosition.x, mousePosition.y);
            return true;
        }
#else
        // スマートフォンで実行される場合
        if (Input.touchCount == 1)
        {
            // UI要素をタップした際はAR空間のオブジェクトへのタップ通知が届かないようにする
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) {
                touchPosition = default;
                Debug.Log("IsPointerOverGameObject");
                return false;
            }
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) {
                // 画面がタッチされた位置を取得する
                touchPosition = touch.position;
                return true;
            }
        }
#endif
        touchPosition = default;
        return false;
    }

    // 毎フレーム呼び出される
    void Update()
    {
        // タッチされていない場合は処理をぬける
        if (!TryGetTouchPosition(out Vector2 touchPosition))
        {
            return;
        }

        // "Move Device" ガイドが表示中ではない場合で
        if (!m_ShowingMoveDevice)
        {
            if (m_ShowingTapToPlace)
            {
                // "Tap to Place" ガイドが表示中ならば
                // 子猫のオブジェクトを配置する
                placeObject.OnTouch(touchPosition);
            } else
            {
                // "Tap to Place" ガイドが表示中ではないならば
                // Dropdownで選択中の要素に対応する機能が呼び出される
                SelectControl(selectedIdx, touchPosition);
            }
        }
    }

    // Dropdownの要素が選択された時に呼び出される
    // 引数のidxに選択された番号が渡される
    public void OnValueChanged(int idx)
    {
        selectedIdx = idx;
    }

    // Dropdownで選択中の要素に対応する機能が呼び出す
    void SelectControl(int idx, Vector2 touchPosition)
    {
        switch (idx)
        {
            case 0: // 子猫を配置する
                placeObject.OnTouch(touchPosition);
                break;
            case 1: // ご飯をあげる（未実装）
                break;
            case 2: // ボールを配置して投げる
                ballControl.OnTouch(touchPosition);
                break;
        }
    }
}
