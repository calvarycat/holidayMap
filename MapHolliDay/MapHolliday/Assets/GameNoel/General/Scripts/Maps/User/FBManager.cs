using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using Facebook.Unity;
using LitJson;

public class FBManager : MonoBehaviour
{
    public static FBManager Instance { get; private set; }
    public string DeepLink { get; private set; }

    private Action _onAutoLoginCB;

    private Action<bool, string> _onLoginCB;
    private Action<bool, string> _onGetFBProfileCB;
    private Action<bool, string> _onSendScoreCB;
    private Action<FBUserScore, string> _onGetUserScoreCB;
    private Action<List<FBUserScore>, string> _onGetUserAndFriendScoreCB;
    private Action<bool, string> _onShareCB;
    private Action<bool, string> _onGetDeepLinkCB;
    private Action<bool, string> _onInviteFriendCB;
    private Action<bool, string> _onAppRequestCB;
    private Action<List<FBUserInfo>, string> _onGetFriendListCB;
    private Action<Texture2D, string> _onGetPictureCB;

    private bool _isDownloadPictureOfEachUser;
    private Action<List<FBUserHighScore>, string> _onGetHighScoreListCB;
    private List<FBUserScore> _userScoreList;
    private List<FBUserHighScore> _userHighScoreList;
    private FBUserHighScore _currentUserHighScore;
    private IEnumerator _getPictureListRef;
    private bool _isGettingPicture;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Init();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyObject(gameObject);
        }
        GetLinkShare();
    }

    private void Init()
    {
    }

    public void AutoLogin(Action onAutoLoginCB)
    {
        _onAutoLoginCB = onAutoLoginCB;
        if (!FB.IsInitialized)
            FB.Init(OnAutoLoginCB, OnHideUnity);
        else
            OnAutoLoginCB();
    }

    private void OnAutoLoginCB()
    {
        if (FB.IsLoggedIn)
        {
            StartGetFBProfile(OnAutoLoginGetFBProfileCB);
        }
        else
        {
            SaveManager.Instance.LoadScoreHistory();
            if (_onAutoLoginCB != null)
                _onAutoLoginCB();
        }
    }

    private void OnAutoLoginGetFBProfileCB(bool isSuccess, string message)
    {
        if (isSuccess)
        {
            GetUserScore(OnAutoLoginGetUserScoreCB);
        }
        else
        {
            LogOut();
            if (_onAutoLoginCB != null)
                _onAutoLoginCB();
        }
    }

    private void OnAutoLoginGetUserScoreCB(FBUserScore userScore, string message)
    {
        if (userScore != null)
        {
            CurrentUser.ScoreOnFacebook = userScore.score;
            SaveManager.Instance.LoadScoreHistory();
        }
        else
        {
            LogOut();
        }
        if (_onAutoLoginCB != null)
            _onAutoLoginCB();
    }

    public bool IsLogged
    {
        get
        {
            if (CurrentUser.User == null)
                return false;
            return FB.IsLoggedIn;
        }
    }

    public void Login(Action<bool, string> onLoginCB)
    {
        _onLoginCB = onLoginCB;

        if (FB.IsInitialized)
        {
            OnInitFBComplete();
        }
        else
        {
            FB.Init(OnInitFBComplete, OnHideUnity);
        }
    }

    public void LogOut()
    {
        FB.LogOut();
        CurrentUser.User = null;
        CurrentUser.Picture = null;
        CurrentUser.ScoreOnFacebook = 0;
      //  SaveManager.Instance.LoadScoreHistory();
    }

    private void OnInitFBComplete()
    {
        if (!FB.IsLoggedIn)
        {
            FB.LogInWithReadPermissions(
                new List<string>
                {
                    "public_profile",
                    "email",
                    "user_friends"
                }, OnLoginWithReadPermissionFBCallback);
        }
        else
        {
            StartGetFBProfile(_onLoginCB);
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (isGameShown)
            Time.timeScale = 1;
        else
            Time.timeScale = 0;
    }

    private void OnLoginWithReadPermissionFBCallback(ILoginResult result)
    {
        bool isSuccess;
        string message;
        HandleResult(result, out isSuccess, out message);

        if (isSuccess)
        {
            StartCoroutine(LogInWithPublishPermissions());
        }
        else
        {
            LogOut();
            if (_onLoginCB != null)
                _onLoginCB(false, message);
        }
    }

    private IEnumerator LogInWithPublishPermissions()
    {
        yield return new WaitForSeconds(0.5f);
        FB.LogInWithPublishPermissions(
            new List<string>
            {
                "publish_actions"
            }, OnLoginWithPublishPermissionFBCallback);
    }

    private void OnLoginWithPublishPermissionFBCallback(ILoginResult result)
    {
        bool isSuccess;
        string message;
        HandleResult(result, out isSuccess, out message);

        if (isSuccess)
        {
            StartGetFBProfile(_onLoginCB);
        }
        else
        {
            LogOut();
            if (_onLoginCB != null)
                _onLoginCB(false, message);
        }
    }

    private void StartGetFBProfile(Action<bool, string> onGetFBProfileCB)
    {
       // Debug.Log("No lay profile");
        _onGetFBProfileCB = onGetFBProfileCB;
        FB.API("/me", HttpMethod.GET, OnGetFBProfileCB);
    }

    private void OnGetFBProfileCB(IGraphResult result)
    {
      //  Debug.Log("GetProfile success");
        bool isSuccess;
        string message;
        HandleResult(result, out isSuccess, out message);

        if (isSuccess)
        {
            bool isProcessDataSuccess = false;

            try
            {
                JsonData jsonData = JsonMapper.ToObject(result.RawResult);
                CurrentUser.User = new FBUserInfo();
                CurrentUser.User.id = jsonData["id"].ToString();
                CurrentUser.User.name = jsonData["name"].ToString();
                CurrentUser.Picture = result.Texture;
                isProcessDataSuccess = true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                CurrentUser.User = null;
                CurrentUser.Picture = null;
            }

            if (isProcessDataSuccess)
            {
               // SaveManager.Instance.LoadScoreHistory();
                if (_onGetFBProfileCB != null)
                    _onGetFBProfileCB(true, "Login success");
            }
            else
            {
                if (_onGetFBProfileCB != null)
                    _onGetFBProfileCB(false, "Process data failed\n" + result.RawResult);
            }
        }
        else
        {
            LogOut();
            if (_onGetFBProfileCB != null)
                _onGetFBProfileCB(false, message);
        }
    }

    public void SendScore(int score, Action<bool, string> onSendScoreCB)
    {
        _onSendScoreCB = onSendScoreCB;

        WWWForm formData = new WWWForm();
        formData.AddField("score", score);
        FB.API("/" + CurrentUser.User.id + "/scores", HttpMethod.POST, OnSendScoreCB, formData);
    }

    private void OnSendScoreCB(IGraphResult result)
    {
        bool isSuccess;
        string message;
        HandleResult(result, out isSuccess, out message);
        if (_onSendScoreCB != null)
            _onSendScoreCB(isSuccess, message);
    }

    public void GetUserScore(Action<FBUserScore, string> onGetUserScoreCB)
    {
        _onGetUserScoreCB = onGetUserScoreCB;
        FB.API("/" + CurrentUser.User.id + "/scores", HttpMethod.GET, OnGetUserScoreCB);
    }

    private void OnGetUserScoreCB(IGraphResult result)
    {
        bool isSuccess;
        string message;
        HandleResult(result, out isSuccess, out message);

        if (isSuccess)
        {
            bool isProcessDataSuccess = false;
            FBUserScore userScore = null;
            bool isFirstTimeLogin = false;

            try
            {
                JsonData jsonData = JsonMapper.ToObject(result.RawResult);
                JsonData data = jsonData["data"];
                if (data != null && data.Count > 0)
                {
                    List<FBUserScore> userScoreList = JsonMapper.ToObject<List<FBUserScore>>(data.ToJson());
                    userScore = userScoreList[0];
                    isProcessDataSuccess = true;
                }
                else
                {
                    // First time login
                    isFirstTimeLogin = true;
                    isProcessDataSuccess = true;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            _onGetUserScoreCB(userScore, "Get score success");
            //if (isProcessDataSuccess)
            //{
            //    if (!isFirstTimeLogin)
            //    {
            //        if (_onGetUserScoreCB != null)
            //            _onGetUserScoreCB(userScore, "Get score success");
            //    }
            //    //else
            //    //{
            //    //    SaveManager.Instance.SaveScoreHistoryFromGenericUserToCurrentUser(OnFirstTimeLoginCB);
            //    //}
            //}
            //else
            //{
            //    if (_onGetUserScoreCB != null)
            //        _onGetUserScoreCB(null, "Process data failed\n" + result.RawResult);
            //}
        }
        else
        {
            if (_onGetUserScoreCB != null)
                _onGetUserScoreCB(null, message);
        }
    }

    private void OnFirstTimeLoginCB(bool isSuccess, string message)
    {
        if (isSuccess)
        {
            FBUserScore userScore = new FBUserScore();
            userScore.score = CurrentUser.ScoreOnFacebook;
            userScore.user = new FBUserInfo();
            userScore.user.id = CurrentUser.User.id;
            userScore.user.name = CurrentUser.User.name;

            if (_onGetUserScoreCB != null)
                _onGetUserScoreCB(userScore, "Get score success");
        }
        else
        {
            if (_onGetUserScoreCB != null)
                _onGetUserScoreCB(null, message);
        }
    }

    public void GetUserAndFriendScore(Action<List<FBUserScore>, string> onGetUserAndFriendScoreCB)
    {
        _onGetUserAndFriendScoreCB = onGetUserAndFriendScoreCB;
        FB.API("/" + FB.AppId + "/scores", HttpMethod.GET, OnGetUserAndFriendScoreCB);
    }

    private void OnGetUserAndFriendScoreCB(IGraphResult result)
    {
        bool isSuccess;
        string message;
        HandleResult(result, out isSuccess, out message);

        if (isSuccess)
        {
            bool isProcessDataSuccess = false;
            List<FBUserScore> userScoreList = null;

            try
            {
                JsonData jsonData = JsonMapper.ToObject(result.RawResult);
                JsonData data = jsonData["data"];
                if (data != null && data.Count > 0)
                {
                    userScoreList = JsonMapper.ToObject<List<FBUserScore>>(data.ToJson());
                    isProcessDataSuccess = true;
                }
                else
                {
                    userScoreList = new List<FBUserScore>();
                    isProcessDataSuccess = true;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            if (isProcessDataSuccess)
            {
                if (_onGetUserAndFriendScoreCB != null)
                    _onGetUserAndFriendScoreCB(userScoreList, "Get ranking success");
            }
            else
            {
                if (_onGetUserAndFriendScoreCB != null)
                    _onGetUserAndFriendScoreCB(null, "Process data failed\n" + result.RawResult);
            }
        }
        else
        {
            if (_onGetUserAndFriendScoreCB != null)
                _onGetUserAndFriendScoreCB(null, message);
        }
    }

    public void GetLinkShare()
    {
#if UNITY_IPHONE
 //  ... iPhone code here...

             linkShare= "https://itunes.apple.com/us/app/Sticky-Monkey/id1197152307";
#endif
#if UNITY_ANDROID
        //  ... Android code here...
        linkShare = "https://play.google.com/store/apps/details?id=com.Starseed.StickyMonkey";
#endif
    }

    private string linkShare;

    public void Share(string contentTitle,
        string contentDescription,
        Action<bool, string> onShareCB)
    {
        _onShareCB = onShareCB;

        FB.ShareLink(new Uri(linkShare),
            contentTitle,
            contentDescription,
            new Uri("https://4.bp.blogspot.com/-N_slPsjnDmM/WFeXqJ77ieI/AAAAAAAAALQ/vchRl1YdfeUucMl1JnA8WkhO5FE4NOzgwCLcB/s320/IconSticky.png"),
            OnShareCB);
    }

    private void OnShareCB(IShareResult result)
    {
        bool isSuccess;
        string message;
        HandleResult(result, out isSuccess, out message);
        if (_onShareCB != null)
            _onShareCB(isSuccess, message);
    }

    public void GetDeepLink(Action<bool, string> onGetDeepLinkCB)
    {
        _onGetDeepLinkCB = onGetDeepLinkCB;
        FB.GetAppLink(DeepLinkCallback);
    }

    private void DeepLinkCallback(IAppLinkResult result)
    {
        bool isSuccess;
        string message;
        HandleResult(result, out isSuccess, out message);

        if (isSuccess)
        {
            if (!string.IsNullOrEmpty(result.Url))
            {
                DeepLink = result.Url;
                if (_onGetDeepLinkCB != null)
                    _onGetDeepLinkCB(true, "Get deep link success");
            }
            else
            {
                if (_onGetDeepLinkCB != null)
                    _onGetDeepLinkCB(false, "Deep link not found");
            }
        }
        else
        {
            if (_onGetDeepLinkCB != null)
                _onGetDeepLinkCB(false, message);
        }
    }

    public void InviteFriend(Action<bool, string> onInviteFriendCB)
    {
        _onInviteFriendCB = onInviteFriendCB;

        if (string.IsNullOrEmpty(DeepLink))
        {
            GetDeepLink(ReadyToInviteFriend);
        }
        else
        {
            ReadyToInviteFriend(true, "Deep link ready");
        }
    }

    private void ReadyToInviteFriend(bool isSuccess, string message)
    {
        if (!isSuccess)
        {
            if (_onInviteFriendCB != null)
            {
                _onInviteFriendCB(false, message);
                return;
            }
        }

        FB.Mobile.AppInvite(new Uri(DeepLink),
            new Uri("https://4.bp.blogspot.com/-N_slPsjnDmM/WFeXqJ77ieI/AAAAAAAAALQ/vchRl1YdfeUucMl1JnA8WkhO5FE4NOzgwCLcB/s320/IconSticky.png"),
            OnInviteFriendCB);
    }

    private void OnInviteFriendCB(IAppInviteResult result)
    {
        bool isSuccess;
        string message;
        HandleResult(result, out isSuccess, out message);
        if (_onInviteFriendCB != null)
            _onInviteFriendCB(isSuccess, message);
    }

    public void SendAppRequest(List<string> targetUserId, Action<bool, string> onAppRequestCB)
    {
        _onAppRequestCB = onAppRequestCB;

        FB.AppRequest(
            "Come play this great game!",
            targetUserId,
            null, null, null, null, null,
            OnAppRequestCB);
    }

    private void OnAppRequestCB(IAppRequestResult result)
    {
        bool isSuccess;
        string message;
        HandleResult(result, out isSuccess, out message);
        if (_onAppRequestCB != null)
            _onAppRequestCB(isSuccess, message);
    }

    public void GetFriendList(Action<List<FBUserInfo>, string> onGetFriendListCB)
    {
        _onGetFriendListCB = onGetFriendListCB;
        FB.API("/" + CurrentUser.User.id + "/friends", HttpMethod.GET, OnGetFriendListCB);
    }

    private void OnGetFriendListCB(IGraphResult result)
    {
        bool isSuccess;
        string message;
        HandleResult(result, out isSuccess, out message);

        if (isSuccess)
        {
            bool isProcessDataSuccess = false;
            List<FBUserInfo> userInfoList = null;

            try
            {
                JsonData jsonData = JsonMapper.ToObject(result.RawResult);
                JsonData data = jsonData["data"];

                if (data != null && data.Count > 0)
                {
                    userInfoList = JsonMapper.ToObject<List<FBUserInfo>>(data.ToJson());
                    isProcessDataSuccess = true;
                }
                else
                {
                    userInfoList = new List<FBUserInfo>();
                    isProcessDataSuccess = true;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            if (isProcessDataSuccess)
            {
                if (_onGetFriendListCB != null)
                    _onGetFriendListCB(userInfoList, message);
            }
            else
            {
                if (_onGetFriendListCB != null)
                    _onGetFriendListCB(null, "Process data failed\n" + result.RawResult);
            }
        }
        else
        {
            if (_onGetFriendListCB != null)
                _onGetFriendListCB(null, message);
        }
    }

    public void GetPicture(string facebookId, Action<Texture2D, string> onGetPictureCB)
    {
        _onGetPictureCB = onGetPictureCB;
        FB.API("/" + facebookId + "/picture", HttpMethod.GET, OnGetPictureCB);
    }

    private void OnGetPictureCB(IGraphResult result)
    {
        bool isSuccess;
        string message;
        HandleResult(result, out isSuccess, out message);

        if (isSuccess)
        {
            if (_onGetPictureCB != null)
                _onGetPictureCB(result.Texture, message);
        }
        else
        {
            if (_onGetPictureCB != null)
                _onGetPictureCB(null, message);
        }
    }

    public void GetHighScoreList(bool isDownloadPictureOfEachUser,
        Action<List<FBUserHighScore>, string> onGetHighScoreListCB)
    {
        _isDownloadPictureOfEachUser = isDownloadPictureOfEachUser;
        _onGetHighScoreListCB = onGetHighScoreListCB;
        GetUserAndFriendScore(OnGetUserAndFriendScoreOfHighScoreListCB);
    }

    private void OnGetUserAndFriendScoreOfHighScoreListCB(List<FBUserScore> userScoreList, string message)
    {
        if (userScoreList != null)
        {
            _userScoreList = userScoreList;
            GetFriendList(OnGetFriendListOfHighScoreListCB);
        }
        else
        {
            if (_onGetHighScoreListCB != null)
                _onGetHighScoreListCB(null, message);
        }
    }

    private void OnGetFriendListOfHighScoreListCB(List<FBUserInfo> userInfoList, string message)
    {
        if (userInfoList != null)
        {
            _userHighScoreList = new List<FBUserHighScore>();

            for (int i = 0; i < userInfoList.Count; i++)
            {
                FBUserHighScore userHighScore = new FBUserHighScore();
                userHighScore.Id = userInfoList[i].id;
                userHighScore.Name = userInfoList[i].name;

                for (int j = 0; j < _userScoreList.Count; j++)
                {
                    if (userHighScore.Id == _userScoreList[j].user.id)
                    {
                        userHighScore.Score = _userScoreList[j].score;
                        userHighScore.IsPlayedApp = true;
                        break;
                    }
                }

                _userHighScoreList.Add(userHighScore);
            }

            // friend list dont contain current user, so need to add manualy
            for (int i = 0; i < _userScoreList.Count; i++)
            {
                if (_userScoreList[i].user.id == CurrentUser.User.id)
                {
                    FBUserHighScore userHighScore = new FBUserHighScore();
                    userHighScore.Id = _userScoreList[i].user.id;
                    userHighScore.Name = _userScoreList[i].user.name;
                    userHighScore.IsPlayedApp = true;
                    userHighScore.Score = _userScoreList[i].score;
                    _userHighScoreList.Add(userHighScore);
                    break;
                }
            }

            _userHighScoreList.Sort((x1, x2) =>
            {
                if (x1.Score > x2.Score)
                    return -1;
                if (x1.Score < x2.Score)
                    return 1;
                return string.Compare(x1.Name, x2.Name);
            });

            if (_isDownloadPictureOfEachUser)
            {
                _getPictureListRef = GetPictureListGetHighScoreList();
                StartCoroutine(_getPictureListRef);
            }
            else
            {
                if (_onGetHighScoreListCB != null)
                    _onGetHighScoreListCB(_userHighScoreList, "Success");
            }
        }
        else
        {
            if (_onGetHighScoreListCB != null)
                _onGetHighScoreListCB(null, message);
        }
    }

    private IEnumerator GetPictureListGetHighScoreList()
    {
        for (int i = 0; i < _userHighScoreList.Count; i++)
        {
            _isGettingPicture = true;
            _currentUserHighScore = _userHighScoreList[i];
            GetPicture(_currentUserHighScore.Id, OnGetPictureOfGetHighScoreListCB);

            while (_isGettingPicture)
                yield return new WaitForEndOfFrame();
        }

        if (_onGetHighScoreListCB != null)
            _onGetHighScoreListCB(_userHighScoreList, "Success");
    }

    private void OnGetPictureOfGetHighScoreListCB(Texture2D picture, string message)
    {
        if (picture != null)
        {
            _currentUserHighScore.Picture = picture;
            _isGettingPicture = false;
        }
        else
        {
            StopCoroutine(_getPictureListRef);
            if (_onGetHighScoreListCB != null)
                _onGetHighScoreListCB(null, message);
        }
    }

    private void HandleResult(IResult result, out bool isSuccess, out string message)
    {
        if (result == null)
        {
            isSuccess = false;
            message = "Connection error";
        }
        else if (!string.IsNullOrEmpty(result.Error))
        {
            isSuccess = false;
            message = "Error\n" + result.Error;
        }
        else if (result.Cancelled)
        {

            isSuccess = false;
            message = "User cancelled";
        }
        else if (!string.IsNullOrEmpty(result.RawResult))
        {
            isSuccess = true;
            message = "Success";
        }
        else
        {
            isSuccess = false;
            message = "Connection error";
        }
    }
}