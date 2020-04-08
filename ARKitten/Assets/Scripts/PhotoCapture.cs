using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

#if UNITY_IOS
using System.Runtime.InteropServices;
#endif


public class PhotoCapture : MonoBehaviour
{
    public GameObject[] UICanvas;
    string fileName;
    string filePath;

#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void SaveToAlbum(string path);
#endif


    public void OnClick()
    {
        SetUIActiveFalse();
#if UNITY_ANDROID
        ScreenCapture.CaptureScreenshot("../../../../DCIM/Camera/" + GetFilePath());
#else
        ScreenCapture.CaptureScreenshot(GetFilePath());
#endif
        //ScreenCapture.CaptureScreenshot(filePath + GetFilePath());
        Invoke("SetUIActiveTrue", 2.0f);
    }

    void SetUIActiveFalse()
    {
        foreach (GameObject c in UICanvas)
        {
            c.gameObject.SetActive(false);
        }
    }

    void SetUIActiveTrue()
    {
#if UNITY_ANDROID

        ScanMedia(filePath + fileName);
#endif

#if UNITY_IOS
        string path = Application.persistentDataPath + "/" + fileName;
        StartCoroutine(SaveToCameraroll(path));
#endif

        foreach (GameObject c in UICanvas)
        {
            c.gameObject.SetActive(true);
        }
    }

#if UNITY_IOS
    IEnumerator SaveToCameraroll(string path)
    {
        Debug.Log("SaveToCamerarool, " + path);
        // ファイルが生成されるまで待つ
        while (true)
        {
            if (File.Exists(path))
            {
                Debug.Log("found " + path);
                break;
            }
            yield return null;
        }

        SaveToAlbum(path);
    }
#endif

    string GetFilePath()
    {
        string file = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png";
#if UNITY_EDITOR
        fileName = file;
#else
        //fileName = Application.persistentDataPath + "/" + file;
        fileName = file;
#endif
        Debug.Log("arkitten_path save file: " + fileName);
        return fileName;
    }

    IEnumerator DelayMethod(float waitSec, Action action)
    {
        yield return new WaitForSeconds(waitSec);
        action();
    }

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_ANDROID
        if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Debug.Log("has external storage permission.");

        }
        else
        {
            Debug.Log("doesn't have external storage permission.");
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }

        using (AndroidJavaClass jcEnvironment = new AndroidJavaClass("android.os.Environment"))
        using (AndroidJavaObject joExDir = jcEnvironment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory"))
        {
            filePath = joExDir.Call<string>("toString") + "/DCIM/Camera/";
            Debug.Log("arkitten_path got_path : " + filePath);
        }
#endif
    }

    //
    static void ScanMedia(string path)
    {
        if (Application.platform != RuntimePlatform.Android)
            return;
#if UNITY_ANDROID
        using (AndroidJavaClass jcUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject joActivity = jcUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (AndroidJavaObject joContext = joActivity.Call<AndroidJavaObject>("getApplicationContext"))
        using (AndroidJavaClass jcMediaScannerConnection = new AndroidJavaClass("android.media.MediaScannerConnection"))
        using (AndroidJavaClass jcEnvironment = new AndroidJavaClass("android.os.Environment"))
        using (AndroidJavaObject joExDir = jcEnvironment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory"))
        {
            //string getpath = joExDir.Call<string>("toString") + "/DCIM/Camera/" + fileName;
            string getpath = joExDir.Call<string>("toString") + "/DCIM/Camera/";
            //string path = fileName;
            Debug.Log("arkitten_path get path : " + getpath);
            Debug.Log("arkitten_path search path : " + path);
            jcMediaScannerConnection.CallStatic("scanFile", joContext, new string[] { path }, new string[] { "image/png" }, null);
        }
#endif
    }

    //

    // Update is called once per frame
    void Update()
    {
        
    }
}
