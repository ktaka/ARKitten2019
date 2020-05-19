using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShareCloudAnchor : MonoBehaviour
{
    public string mailAddress;
    public string cloudAnchorID;

    public void OnClick()
    {
        Debug.Log("ShareAnchorID: OnClick");
        ShareIdViaEmail(mailAddress, cloudAnchorID);
    }

    void ShareIdViaEmail(string mailAddress, string id)
    {
        string subject = UnityWebRequest.EscapeURL("ARKittenCloudAnchorID");
        string body = UnityWebRequest.EscapeURL(id);
        string url = "mailto:" + mailAddress + "?subject=" + subject + "&body=" + body;
        Application.OpenURL(url);
    }
}
