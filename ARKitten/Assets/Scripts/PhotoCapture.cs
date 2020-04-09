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

    public void OnClick()
    {
        SetUIActiveFalse();
        string path = filePath + GetFilePath();

        StartCoroutine(DelayFrame(() =>
        {
            CaptureScreenshot(path);
            AddToPhotoRegistry(path);
        }));
    }

    IEnumerator DelayFrame(Action action)
    {
        yield return new WaitForEndOfFrame();
        action();
    }

    void CaptureScreenshot(string path)
    {
        var texture = ScreenCapture.CaptureScreenshotAsTexture();
        var bytes = texture.EncodeToJPG();
        Destroy(texture);
        File.WriteAllBytes(path, bytes);
        Debug.Log("CaptureScreenshot: save to " + path);
        savedFileName = path;
    }

    void SetUIActiveFalse()
    {
        foreach (GameObject c in UICanvas)
        {
            c.gameObject.SetActive(false);
        }
    }

    void AddToPhotoRegistry(string path)
    {
#if UNITY_EDITOR
        SetUIActiveTrue();
#elif UNITY_ANDROID
        AddToGallery(path);
        SetUIActiveTrue();
#elif UNITY_IOS
        AddToPhotoLibrary(path);
#endif
    }

    void SetUIActiveTrue()
    {
        foreach (GameObject c in UICanvas)
        {
            c.gameObject.SetActive(true);
        }
    }

#if !UNITY_EDITOR && UNITY_IOS
    void AddToPhotoLibraryCompleted (string message) {
        Debug.Log("AddToPhotoLibraryCompleted: " + message + " was added to photo library.");
        File.Delete(savedFileName);
        SetUIActiveTrue();
    }
#endif

    string GetFilePath()
    {
        string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg";
        Debug.Log("arkitten_path save file: " + fileName);
        return fileName;
    }

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
#elif UNITY_ANDROID
        if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Debug.Log("has external storage permission.");

        }
        else
        {
            Debug.Log("doesn't have external storage permission.");
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }

        using (AndroidJavaClass osEnvironment = new AndroidJavaClass("android.os.Environment"))
        using (AndroidJavaObject getExternalStorageDirectory = osEnvironment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory"))
        {
            filePath = getExternalStorageDirectory.Call<string>("toString") + "/DCIM/Camera/";
            Debug.Log("arkitten_path got_path : " + filePath);
        }
#elif UNITY_IOS
        filePath = Application.persistentDataPath + "/";
#endif
    }

#if !UNITY_EDITOR && UNITY_ANDROID
    static void AddToGallery(string path)
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (AndroidJavaObject appContext = activity.Call<AndroidJavaObject>("getApplicationContext"))
        using (AndroidJavaClass mediaScannerConnection = new AndroidJavaClass("android.media.MediaScannerConnection"))
        {
            Debug.Log("arkitten_path search path : " + path);
            mediaScannerConnection.CallStatic("scanFile", appContext, new string[] { path }, new string[] { "image/jpg" }, null);
        }
    }
#endif

    // Update is called once per frame
    void Update()
    {
        
    }
}
