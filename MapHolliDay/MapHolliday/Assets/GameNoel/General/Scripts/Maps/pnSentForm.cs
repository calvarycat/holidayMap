using LitJson;
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
    public Texture2D tx2d;
    string userFacebookID;
    public Text txtName;
    public Text txtEmail;
    public Text txtPhone;
    public Text txtage;
    byte[] icon;
   public void OnShow(bool isShow)
    {
        root.SetActive(isShow);

    }
    public void ButtonSentClick()
    {
        GetFormSendInfomation();
    }
    public void GetFormSendInfomation()
    {
        if(string.IsNullOrEmpty(txtName.text)|| string.IsNullOrEmpty(txtage.text)|| string.IsNullOrEmpty(txtPhone.text)
            || string.IsNullOrEmpty(txtEmail.text))
        {
            PanelPopUp.intance.OnInitInforPopUp("Opps", "Check your input");
            return;
        }

        frmSent = new FormSent();
        ReadTexture();
        frmSent.name = txtName.text;
        frmSent.age = int.Parse(txtage.text);
        frmSent.phone = txtPhone.text;
        // StartCoroutine(SentForm());
        leaderboard.OnShow(true);
    }
    IEnumerator SentForm()
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("icon", icon, "iconUser.png", "image/png");
        string datarp = JsonMapper.ToJson(frmSent);
        form.AddField("data", datarp);
        WWW httpResponse = new WWW("http://localhost:61604/", form);
        float timer = 0;
        while (!httpResponse.isDone)
        {
            if (timer > 120)
            {
                Debug.Log("sent too long");
                break;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        yield return httpResponse;
        if (!string.IsNullOrEmpty(httpResponse.error))
        {
            Debug.Log("sent error");
        }
        yield return null;
    }
    void ReadTexture()
    {
        RenderTexture tmp = RenderTexture.GetTemporary(
                   raw.texture.width,
                   raw.texture.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        // Blit the pixels on texture to the RenderTexture
        Graphics.Blit(raw.texture, tmp);
        // Backup the currently set RenderTexture
        RenderTexture previous = RenderTexture.active;
        // Set the current RenderTexture to the temporary one we created
        RenderTexture.active = tmp;
        // Create a new readable Texture2D to copy the pixels to it
        Texture2D myTexture2D = new Texture2D(raw.texture.width, raw.texture.height);
        // Copy the pixels from the RenderTexture to the new Texture
        myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
        myTexture2D.Apply();
        // Reset the active RenderTexture
        RenderTexture.active = previous;
        // Release the temporary RenderTexture
        RenderTexture.ReleaseTemporary(tmp);
       icon = myTexture2D.EncodeToPNG();
        // "myTexture2D" now has the same pixels from "texture" and it's readable.
    }
}
public class FormSent
{
    public string name;
    public string email;
    public string phone;
    public string message;
    public int age;
    public int score; 
    public string userFacebookID;
    public string facebookName;
}