using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Facebook.Unity;

public class LeaderBoard : MonoBehaviour {
    public GameObject Root;
    public GameObject btnLoggin;
    public Texture2D texx;
    public RawImage rawi;
    public Image imgFa;
    public GameObject leaderPrefabs;
    public Transform parentLeader;

    public List<LeaderUser> listLeader;
    // Use this for initialization
    void Start () {
        Debug.Log(DateTime.Now);       
	}
	public void OnShow(bool isShow)
    {
        Root.gameObject.SetActive(true);
    }
    public void LoadLeaderBoard()
    {
        Utils.RemoveAllChildren(parentLeader);
        foreach(LeaderUser ld in listLeader)
        {
            GameObject obj = Instantiate(leaderPrefabs, parentLeader);
            LeaderInit li = obj.GetComponent<LeaderInit>();
            li.Init(ld);
            obj.transform.localScale = Vector3.one;
        }
    }

   public void Loggin()
    {
        OnLoginFacebookButtonClick();
    }
    private void OnLoginFacebookButtonClick()
    {      
       // PopupManager.Instance.ShowLoading();
       //if(FBManager.Instance.IsLogged)
       // {
            FBManager.Instance.Login(OnLoginFacebookCallback);
            btnLoggin.SetActive(FBManager.Instance.IsLogged);

        //}else
        //{
          
        //    PanelPopUp.intance.OnInitInforPopUp("", "faile " + CurrentUser.User.id + "  " + CurrentUser.User.name);
        //}
      
    }

    private void OnLoginFacebookCallback(bool isSuccess, string message)
    {
       
     
        if (isSuccess)
        {
           // PanelPopUp.intance.OnInitInforPopUp("","Login success");
            //PopupManager.Instance.ShowLoading();
             FBManager.Instance.GetUserScore(OnGetUserScoreCB);
            // btnInvite.SetActive(true);
            Debug.Log("Login Success");
          

        }
        else
        {
            PanelPopUp.intance.OnInitInforPopUp("","faile");
            //  btnInvite.SetActive(false);
            //   PopupManager.Instance.InitInfoPopup(message, null);
            Debug.Log("Login failed");
        }
    }
    private void OnGetUserScoreCB(FBUserScore userScore, string message)
    {
        Debug.Log("KHong lay score");
     //   PopupManager.Instance.HideLoading();
        if (userScore != null)
        {
            PanelPopUp.intance.OnInitInforPopUp("", "cos score "+CurrentUser.User.id +"  "+ CurrentUser.User.name);
            CurrentUser.ScoreOnFacebook = userScore.score;
           //  LoginFacebookButton.gameObject.SetActive(!FBManager.Instance.IsLogged);
            //    _isLoadUserAndFriendScore = true;
             //  InitUserScore();
            //     InitUserAndFriendScore();
            UpdateCurrentUserScore();
            Debug.Log("Get User Score");
          
        }
        else
        {
            PanelPopUp.intance.OnInitInforPopUp("", "faile");

           UpdateCurrentUserScore();
            Debug.Log("User null");
           // PopupManager.Instance.InitInfoPopup(message, null);
        }
      //  SaveManager.Instance.LoadScoreHistory();
    }
    public Image imgFaceBookAvata;
    public void UpdateCurrentUserScore()
    {
      //  1633985750233291
        Facebook.Unity.FB.API("https" + "://graph.facebook.com/" + "1633985750233291"  + "/picture?type=large", HttpMethod.GET, delegate (IGraphResult result)
        {
            imgFaceBookAvata.overrideSprite = Sprite.Create(result.Texture, new Rect(0, 0, 125, 125), new Vector2(0.5f, 0.5f), 100);
        });
       
    }
  
}
public class LeaderUser
{
    public int userID;
    public string UserName;
    public int rank;
    public float timePlay;

}