using UnityEngine;
using UnityEngine.XR.ARFoundation;

// based on https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scripts/LightEstimation.cs

// このクラスを使用するGameObjectにはLightコンポーネントが追加されていることを必須とする
[RequireComponent(typeof(Light))]
public class LightEstimation : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The ARCameraManager which will produce frame events containing light estimation information.")]
    ARCameraManager m_CameraManager;

    // AR Foundationからカメラプレビューのフレームが更新される毎に通知を受け、光源推定値を得るために
    // ARCameraManagerコンポーネントを含むCameraオブジェクトをUnityエディタ上で設定する
    public ARCameraManager cameraManager
    {
        get { return m_CameraManager; }
        set
        {
            // 既に設定済みのものと同じ場合は何もしない
            if (m_CameraManager == value)
                return;

            // 既に設定済みの場合はCameraManagerを更新する前に
            // フレーム毎にFrameChangedメソッドが呼ばれるように登録していたものを解除する
            if (m_CameraManager != null)
                m_CameraManager.frameReceived -= FrameChanged;

            // CameraManagerを更新する
            m_CameraManager = value;

            // CameraManagerが有効な場合
            // フレーム毎にFrameChangedメソッドが呼ばれるよう登録する
            if (m_CameraManager != null & enabled)
                m_CameraManager.frameReceived += FrameChanged;
        }
    }

    Light m_Light;

    // 光源の明るさの推定値（有効な場合）
    public float? brightness { get; private set; }

    // 色温度の推定値（有効な場合）
    public float? colorTemperature { get; private set; }

    // 色補正値の推定値（有効な場合）
    public Color? colorCorrection { get; private set; }

    // 光源の色
    Color lightColor;

    // すべてのオブジェクトのインスタンス生成後に一度だけ呼び出される
    void Awake()
    {
        // ライトオブジェクトを取得し、光源の色を得る
        m_Light = GetComponent<Light>();
        lightColor = m_Light.color;
    }

    // オブジェクトがアクティブになった時に呼び出される
    void OnEnable()
    {
        // フレーム毎にFrameChangedメソッドが呼ばれるよう登録する
        if (m_CameraManager != null)
            m_CameraManager.frameReceived += FrameChanged;
    }

    // オブジェクトが非アクティブになった時に呼び出される
    void OnDisable()
    {
        // フレーム毎にFrameChangedメソッドが呼ばれるように登録していたものを解除する
        if (m_CameraManager != null)
            m_CameraManager.frameReceived -= FrameChanged;
    }

    // フレーム毎に呼び出され、AR Foundationからの各推定値をライトオブジェクトに反映する
    void FrameChanged(ARCameraFrameEventArgs args)
    {
        // 引数のargs.lightEstimationに光源推定機能の各推定値が格納されている
        // 各推定値ごとにHasValueがtrueならば有効な値、falseならば向こうな値（使用不可）

        // 光源の明るさのプロパティが有効な場合
        if (args.lightEstimation.averageBrightness.HasValue)
        {
            // プロパティの値をライトの強度に設定する
            brightness = args.lightEstimation.averageBrightness.Value;
            m_Light.intensity = brightness.Value;
        }

        // 色温度のプロパティが有効な場合
        if (args.lightEstimation.averageColorTemperature.HasValue)
        {
            // プロパティの値をライトの色温度に設定する
            colorTemperature = args.lightEstimation.averageColorTemperature.Value;
            m_Light.colorTemperature = colorTemperature.Value;
        }

        // 色補正値のプロパティが有効な場合
        if (args.lightEstimation.colorCorrection.HasValue)
        {
            // 光源の色にプロパティの値を掛けた値をライトの色に設定する
            colorCorrection = args.lightEstimation.colorCorrection.Value;
            m_Light.color = lightColor * colorCorrection.Value;
            // 元のサンプルではcolorCorrection.Valueを単純に代入していたが
            // Unityのエディタで設定した光源の色(lightColor)を考慮するよう変更
        }
    }
}
