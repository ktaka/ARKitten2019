using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
#elif UNITY_ANDROID
using UnityEngine.Android;
#elif UNITY_IOS
using System.Runtime.InteropServices;
#endif

public class PhotoCapture : MonoBehaviour
{
    public GameObject[] UICanvas;
    string filePath;
    string savedFileName;

#if !UNITY_EDITOR && UNITY_IOS
    [DllImport("__Internal")]
    private static extern void AddToPhotoLibrary(string path);
#endif

    // ボタンがクリックされた時に呼び出される
    public void OnClick()
    {
        // ボタン等のUIを非表示にする
        SetUIActiveFalse();
        // ファイルの名称と保存パスをセットする
        string path = filePath + GetFilePath();

        // 1フレーム待ってから実行する
        StartCoroutine(DelayFrame(() =>
        {
            // スクリーンキャプチャ
            CaptureScreenshot(path);
            // フォトライブラリへの登録
            AddToPhotoRegistry(path);
        }));
    }

    // 1フレーム遅らせてから渡された処理を実行する
    IEnumerator DelayFrame(Action action)
    {
        yield return new WaitForEndOfFrame();
        action();
    }

    // スクリーンキャプチャをとる
    void CaptureScreenshot(string path)
    {
        // スクリーンキャプチャをテクスチャとして取得する
        var texture = ScreenCapture.CaptureScreenshotAsTexture();
        // テクスチャ画像をJPEGにエンコードする
        var bytes = texture.EncodeToJPG();
        // JPEGエンコード後はテクスチャは不要となるため削除する
        Destroy(texture);
        // JPEGエンコードされたバイト列を指定されたファイルに保存する
        File.WriteAllBytes(path, bytes);
        Debug.Log("CaptureScreenshot: save to " + path);
        savedFileName = path;
    }

    // ボタンなどのUI要素を非表示にする
    void SetUIActiveFalse()
    {
        foreach (GameObject c in UICanvas)
        {
            c.gameObject.SetActive(false);
        }
    }

    // 画像ファイルをフォトライブラリに登録する
    void AddToPhotoRegistry(string path)
    {
#if UNITY_EDITOR
        // エディタのプレイモードで実行する場合は登録不要
        // 非表示にしたUIを元に戻す
        SetUIActiveTrue();
#elif UNITY_ANDROID
        // Androidのフォトギャラリーに登録する
        AddToGallery(path);
        // 非表示にしたUIを元に戻す
        SetUIActiveTrue();
#elif UNITY_IOS
        // iOSのプラグインでフォトライブラリに登録する
        AddToPhotoLibrary(path);
#endif
    }

    // ボタンなどのUI要素を表示する
    void SetUIActiveTrue()
    {
        foreach (GameObject c in UICanvas)
        {
            c.gameObject.SetActive(true);
        }
    }

#if !UNITY_EDITOR && UNITY_IOS
    // iOSプラグインから呼び出されるコールバック
    // フォトライブラリに登録後に呼び出される
    void AddToPhotoLibraryCompleted (string message) {
        Debug.Log("AddToPhotoLibraryCompleted: " + message + " was added to photo library.");
        // フォトライブラリ登録後は画像ファイルが不要になるため削除する
        File.Delete(savedFileName);
        // 非表示にしたUIを元に戻す
        SetUIActiveTrue();
    }
#endif

    // ファイル名の生成
    string GetFilePath()
    {
        // 現在時刻をミリ秒まで取得して文字列化してファイル名とする
        string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg";
        Debug.Log("arkitten_path save file: " + fileName);
        return fileName;
    }

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
#elif UNITY_ANDROID
        // Androidの場合
        // 外部ストレージへの書き込みパーミッションがあるか確認する
        if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Debug.Log("has external storage permission.");
        }
        else
        {   // パーミッションがない場合
            Debug.Log("doesn't have external storage permission.");
            // パーミッションの取得を求める（ダイアログが表示される）
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }

        // Javaのクラスのメソッドを呼び出せるようにする
        using (AndroidJavaClass osEnvironment = new AndroidJavaClass("android.os.Environment"))
        using (AndroidJavaObject getExternalStorageDirectory = osEnvironment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory"))
        {
            // 画像保存用に外部ストレージのパスを取得する
            filePath = getExternalStorageDirectory.Call<string>("toString") + "/DCIM/Camera/";
            Debug.Log("arkitten_path got_path : " + filePath);
        }
#elif UNITY_IOS
        // iOSの場合
        // 一時保存先のフォルダのパスを取得する
        filePath = Application.persistentDataPath + "/";
#endif
    }

#if !UNITY_EDITOR && UNITY_ANDROID
    // Androidの場合に呼び出されるフォトライブラリ登録
    static void AddToGallery(string path)
    {
        // Javaのクラスのメソッドを呼び出せるようにする
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (AndroidJavaObject appContext = activity.Call<AndroidJavaObject>("getApplicationContext"))
        using (AndroidJavaClass mediaScannerConnection = new AndroidJavaClass("android.media.MediaScannerConnection"))
        {
            Debug.Log("arkitten_path search path : " + path);
            // 指定された画像ファイルをフォトライブラリに登録する
            mediaScannerConnection.CallStatic("scanFile", appContext, new string[] { path }, new string[] { "image/jpg" }, null);
        }
    }
#endif

    // Update is called once per frame
    void Update()
    {

    }
}
