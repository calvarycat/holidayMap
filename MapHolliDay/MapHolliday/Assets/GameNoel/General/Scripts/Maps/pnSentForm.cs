using Facebook.Unity;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pnSentForm : MonoBehaviour
{
    public LeaderBoard leaderboard;
    public FormSent frmSent;
    public GameObject root;
    public RawImage raw;
    public Text txtName;
    public Text txtEmail;
    public Text txtPhone;
    public Text txtage;
    byte[] icon;
    public ControlQuestion controlQuest;
    public Text txtResult;
    public Selfie selfie;
    public void OnShow(bool isShow)
    {
        root.SetActive(isShow);
        txtResult.text = Utils.SecondToString((int)controlQuest.timeRuning);
        UpdateCurrentUserScore();// get usser facebook image

    }
    public void ButtonSentClick()
    {
        GetFormSendInfomation();
    }
    public void GetFormSendInfomation()
    {
        if (string.IsNullOrEmpty(txtName.text) || string.IsNullOrEmpty(txtage.text) || string.IsNullOrEmpty(txtPhone.text)
            || string.IsNullOrEmpty(txtEmail.text))
        {
            PanelPopUp.intance.OnInitInforPopUp("Opps", "Check your input");
            return;
        }
        if(!IsValidEmail(txtEmail.text.Trim()))
        {
            {
                PanelPopUp.intance.OnInitInforPopUp("Opps", "Email không hợp lệ");
                return;
            }
        }

        frmSent = new FormSent();
        ReadTexture();
        frmSent.name = txtName.text;
        frmSent.email = txtEmail.text;
        frmSent.age = int.Parse(txtage.text);
        frmSent.phone = txtPhone.text;
        frmSent.timeplay = (int)controlQuest.timeRuning;
        Sent();

    }


    bool IsValidEmail(string email)
    {
        try
        {

            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    void Sent()
    {
        string datarp = JsonMapper.ToJson(frmSent);
        BaseOnline.Instance.Post("https://wallstreetenglish.edu.vn/api/index/send-form-challenge", frmSent, ResponseFromServer);

    }
    void ResponseFromServer(string res)
    {
        Debug.Log(res);
        try
        {
            RootSentFormResult rootresult = JsonMapper.ToObject<RootSentFormResult>(res);
            SentSuccess(); // save thong tin vaof trong app
            PlayerPrefs.SetInt(PlayerPrefsContance.UserID, rootresult.userID);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
            PanelPopUp.intance.OnInitInforPopUp("Opps!", "Try again " + res + ex.ToString(), "Ok" );
        }


    }
   
    public void SentSuccess()
    {
        leaderboard.OnShow(true);
    }
    void ReadTexture()
    {
        RenderTexture tmp = RenderTexture.GetTemporary(
                   raw.texture.width,
                   raw.texture.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);
        Graphics.Blit(raw.texture, tmp);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = tmp;
        Texture2D myTexture2D = new Texture2D(raw.texture.width, raw.texture.height);
        myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
        myTexture2D.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(tmp);
        // icon = myTexture2D.EncodeToPNG();  
        frmSent.icon = Utils.TextureToString(myTexture2D);
    }
    public GameObject pnPopupSuccess;
    public void BangXepHangClick()
    {
        leaderboard.OnShow(true);
        pnPopupSuccess.gameObject.SetActive(false);
    }
    // public Image imgFaceBookAvata;
    public void UpdateCurrentUserScore()
    {
        if (FB.IsLoggedIn)
        {
            Facebook.Unity.FB.API("https" + "://graph.facebook.com/" + "1633985750233291" + "/picture?type=large", HttpMethod.GET, delegate (IGraphResult result)
            {
                raw.texture = result.Texture;//(Texture)Sprite.Create(result.Texture, new Rect(0, 0, 125, 125), new Vector2(0.5f, 0.5f), 100);
            });
        }
    }
    public void OnBackClick()
    {
        selfie.gameObject.SetActive(true);
        OnShow(false);
    }
}
public class FormSent
{
    public string name;
    public string email;
    public string phone;
    public string icon;
    public int age;
    public float timeplay;

}
public class RootSentFormResult
{
    public string status { get; set; }
    public int userID { get; set; }
}