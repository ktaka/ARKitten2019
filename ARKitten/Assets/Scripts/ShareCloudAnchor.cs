using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// UnityWebRequestを使用するために追加
using UnityEngine.Networking;

public class ShareCloudAnchor : MonoBehaviour
{
    public string mailAddress;  // 送付先メールアドレス
    public string cloudAnchorID;  // Cloud Anchor ID（PlaceObjectからセットされる）

    // ボタンが押されたときに呼び出される
    public void OnClick()
    {
        // Cloud Anchor IDをメールで共有する
        ShareIdViaEmail(mailAddress, cloudAnchorID);
    }

    // Cloud Anchor IDを共有するためにメーラーを起動する
    void ShareIdViaEmail(string mailAddress, string id)
    {
        // サブジェクトの文字列を入れる（URLに不適切な文字はエスケープする）
        string subject = UnityWebRequest.EscapeURL("ARKittenCloudAnchorID");
        // ボディ（本文）にCloud Anchor IDを入れる（URLに不適切な文字はエスケープする）
        string body = UnityWebRequest.EscapeURL(id);
        // mailtoスキームのURL文字列を作成する
        // （送付先メールアドレス、サブジェクト、ボディを付加）
        string url = "mailto:" + mailAddress + "?subject=" + subject + "&body=" + body;
        // 作成したURLをオープンすることで下記要素が入力済みの状態でメーラーを起動する
        //   送付先メールアドレス、サブジェクト、ボディ
        Application.OpenURL(url);
    }
}
