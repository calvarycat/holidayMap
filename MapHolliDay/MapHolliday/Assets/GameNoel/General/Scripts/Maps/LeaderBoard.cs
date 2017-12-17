using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Facebook.Unity;
using LitJson;

public class LeaderBoard : MonoBehaviour
{
    public GameObject Root;
    public GameObject btnLoggin;
    public Texture2D texx;
    public RawImage rawi;
    public Image imgFa;
    public GameObject leaderPrefabs;
    public Transform parentLeader;
    public LeaderInit leaderUserDefault;
    void Start()
    {

        GetDataRank();
        GetUserRank();
    }
    public void OnShow(bool isShow)
    {
        Root.gameObject.SetActive(true);        
        GetDataRank();
        //  
      //  leaderUserDefault.LoadLeaderItem(0, rootresult.result[0]);
    }
    public void GetUserRank()
    {
        SentGetUserID st = new SentGetUserID();
        int userid = PlayerPrefs.GetInt(PlayerPrefsContance.UserID, -1);
     
        if (userid != -1)
        {
            st.userID = userid;
            BaseOnline.Instance.Post("https://wallstreetenglish.edu.vn/api/index/get-rank-user", st, ResPonseUserRank);
        }
    }
    private void ResPonseUserRank(string res)
    {
        int a = 5;      
        try
        {
           
            RootUserRank rootresult = JsonMapper.ToObject<RootUserRank>(res);
            //Debug.Log(rootresult.result[0].rank);

           leaderUserDefault.LoadLeaderItem(rootresult.result[0].rank, rootresult.result[0]);
           

        }
        catch (Exception ex)
        {
            Debug.Log("respon User" + ex.ToString());
        }
    }
    public void GetDataRank()
    {

        SentGetUserID st = new SentGetUserID();
        int userid = PlayerPrefs.GetInt(PlayerPrefsContance.UserID, -1);
        if (userid != -1)
        {
            st.userID = userid;
            BaseOnline.Instance.Post("https://wallstreetenglish.edu.vn/api/index/get-rank", st, ResponseFromServer);
        }


    }
    public void ResponseFromServer(string res)
    {
      
        try
        {
            int rank = 1;
            RootObjectLeaderBoard rootresult = JsonMapper.ToObject<RootObjectLeaderBoard>(res);
            foreach (Transform tran in parentLeader.transform)
            {
                LeaderInit li = tran.GetComponent<LeaderInit>();
                li.LoadLeaderItem(rank, rootresult.result[rank-1]);
                rank++;
            }

        }
        catch (Exception ex)
        {
            Debug.Log("respon Leader" + ex.ToString());
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
            PanelPopUp.intance.OnInitInforPopUp("", "faile");
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
            PanelPopUp.intance.OnInitInforPopUp("", "cos score " + CurrentUser.User.id + "  " + CurrentUser.User.name);
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
        Facebook.Unity.FB.API("https" + "://graph.facebook.com/" + "1633985750233291" + "/picture?type=large", HttpMethod.GET, delegate (IGraphResult result)
       {
           imgFaceBookAvata.overrideSprite = Sprite.Create(result.Texture, new Rect(0, 0, 125, 125), new Vector2(0.5f, 0.5f), 100);
       });

    }

}

public class SentGetUserID
{
    public int userID;

}

public class Result
{
    public int userID { get; set; }
    public string name { get; set; }
    public double? timeplay { get; set; }
    public string icon { get; set; }
    public int rank { get; set; }
}

public class RootObjectLeaderBoard
{
    public List<Result> result { get; set; }
}


public class RootUserRank
{
    public List<Result> result { get; set; }
}