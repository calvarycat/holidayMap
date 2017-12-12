using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ICSharpCode.SharpZipLib.Zip;
using LitJson;

public class ApiManager : MonoSingleton<ApiManager>
{
    public static bool Tracking;

    private string _domainName;
    private string _appName;
    private string _pushNotificationToken;
    private int _currentApiClientId;
    private string _currentToken;
    private string _secret;
    private Action<VersionNumber, VersionNumber> _onNewApiVersion;
    private Action<int, string> _onInitFinish;

    private bool _updating;
    private IEnumerator _initializeAppMessageRef;
    private IEnumerator _checkVersionMessageRef;

    /// <summary>
    ///     Check api version, then return api client id and token
    /// </summary>
    /// <param name="domainName"></param>
    /// <param name="appName"></param>
    /// <param name="pushNotificationToken"></param>
    /// <param name="currentApiClientId"></param>
    /// <param name="currentToken"></param>
    /// <param name="secret"></param>
    /// <param name="onNewApiVersion">if new version, return (current version, new version)</param>
    /// <param name="onInitFinish">if error, return (-1, "") else return (apiClientId, token)</param>
    public void Init(string domainName,
        string appName,
        string pushNotificationToken,
        int currentApiClientId,
        string currentToken,
        string secret,
        Action<VersionNumber, VersionNumber> onNewApiVersion,
        Action<int, string> onInitFinish)
    {
#if UNITY_EDITOR
        if (Tracking)
        {
            PrintTrack(MethodBase.GetCurrentMethod().Name);
            PrintTrack("domainName: " + domainName);
            PrintTrack("appName: " + appName);
            PrintTrack("pushNotificationToken: " + pushNotificationToken);
            PrintTrack("currentApiClientId: " + currentApiClientId);
            PrintTrack("currentToken: " + currentToken);
            PrintTrack("secret: " + secret);
        }
#endif

        if (_updating)
        {
            Debug.LogError("Many init Api request");
            return;
        }

        _updating = true;

        _domainName = domainName;
        _appName = appName;
        _pushNotificationToken = pushNotificationToken;
        _currentApiClientId = currentApiClientId;
        _currentToken = currentToken;
        _secret = secret;
        _onNewApiVersion = onNewApiVersion;
        _onInitFinish = onInitFinish;

        if (_currentApiClientId <= 0
            || string.IsNullOrEmpty(_currentToken)
            || string.IsNullOrEmpty(_secret))
        {
            Initialize();
        }
        else
        {
            CheckVersion();
        }
    }

    private void Initialize()
    {
#if UNITY_EDITOR
        if (Tracking)
        {
            PrintTrack(MethodBase.GetCurrentMethod().Name);
        }
#endif

        InitializeAppMessage message = new InitializeAppMessage(_domainName,
            _appName,
            GetCurrentApiVersionOnDevice(),
            _pushNotificationToken);
        _initializeAppMessageRef = BaseOnline.Instance.WWWCoroutine(message.url,
            message,
            OnInitMessageResponse);
        StartCoroutine(_initializeAppMessageRef);
    }

    private void CheckVersion()
    {
#if UNITY_EDITOR
        if (Tracking)
        {
            PrintTrack(MethodBase.GetCurrentMethod().Name);
        }
#endif

        string sign = Utility.GetMd5Hash(_currentToken + _currentApiClientId + _secret);
        string authString = "?apiclientid=" + _currentApiClientId + "&token=" + _currentToken + "&sign=" + sign;
        CheckVersionMessage message = new CheckVersionMessage(_domainName, authString);
        _checkVersionMessageRef = BaseOnline.Instance.WWWCoroutine(message.Url,
            OnCheckVersionMessageResponse);
        StartCoroutine(_checkVersionMessageRef);
    }

    private void OnInitMessageResponse(string response)
    {
#if UNITY_EDITOR
        if (Tracking)
        {
            PrintTrack(MethodBase.GetCurrentMethod().Name);
            PrintTrack("response: " + response);
        }
#endif

        _initializeAppMessageRef = null;

        if (string.IsNullOrEmpty(response))
        {
            FinishInit();
            return;
        }

        InitializeAppResponseMessage responseMessage = InitializeAppResponseMessage.FromJson(response);
        if (responseMessage == null)
        {
            FinishInit();
            return;
        }

        VersionNumber currentApiVersionOnDevice = GetCurrentApiVersionOnDevice();
        CompareVersionNumber compareResult =
            (CompareVersionNumber)currentApiVersionOnDevice.CompareTo(responseMessage.ApiVersion);

        if (compareResult == CompareVersionNumber.Equal)
        {
            _currentApiClientId = responseMessage.ApiclientId;
            _currentToken = responseMessage.Token;
            FinishInit(responseMessage.ApiclientId, responseMessage.Token);
        }
        else
        {
            _updating = false;
            if (_onNewApiVersion != null)
                _onNewApiVersion(currentApiVersionOnDevice, responseMessage.ApiVersion);
        }
    }

    private void OnCheckVersionMessageResponse(string response)
    {
#if UNITY_EDITOR
        if (Tracking)
        {
            PrintTrack(MethodBase.GetCurrentMethod().Name);
            PrintTrack("response: " + response);
        }
#endif

        _checkVersionMessageRef = null;

        if (string.IsNullOrEmpty(response))
        {
            Initialize();
            return;
        }

        CheckVersionResponseMessage responseMessage = CheckVersionResponseMessage.FromJson(response);
        if (responseMessage == null)
        {
            Initialize();
            return;
        }

        if (responseMessage.Status != BaseOnline.Success)
        {
            Initialize();
            return;
        }

        VersionNumber currentApiVersionOnDevice = GetCurrentApiVersionOnDevice();
        CompareVersionNumber compareResult =
            (CompareVersionNumber)currentApiVersionOnDevice.CompareTo(responseMessage.ApiVersion);

        switch (compareResult)
        {
            case CompareVersionNumber.MajorGreater:
            case CompareVersionNumber.MinorGreater:
            case CompareVersionNumber.PacthGreater:
            case CompareVersionNumber.Error:
                Initialize();
                break;

            case CompareVersionNumber.MajorLess:
            case CompareVersionNumber.MinorLess:
            case CompareVersionNumber.PatchLess:
                _updating = false;
                if (_onNewApiVersion != null)
                    _onNewApiVersion(currentApiVersionOnDevice, responseMessage.ApiVersion);
                break;

            case CompareVersionNumber.Equal:
                FinishInit(_currentApiClientId, _currentToken);
                break;
        }
    }

    private void FinishInit(int apiClientId = -1, string token = "")
    {
#if UNITY_EDITOR
        if (Tracking)
        {
            PrintTrack(MethodBase.GetCurrentMethod().Name);
            PrintTrack("apiClientId: " + apiClientId);
            PrintTrack("token: " + token);
        }
#endif

        _updating = false;

        if (_onInitFinish != null)
            _onInitFinish(apiClientId, token);
    }

    /// <summary>
    ///     Cause unknown error
    /// </summary>
    public void ForceStopUpdate()
    {
#if UNITY_EDITOR
        if (Tracking)
        {
            PrintTrack(MethodBase.GetCurrentMethod().Name);
        }
#endif

        if (_initializeAppMessageRef != null)
        {
            StopCoroutine(_initializeAppMessageRef);
            _initializeAppMessageRef = null;
        }

        if (_checkVersionMessageRef != null)
        {
            StopCoroutine(_checkVersionMessageRef);
            _checkVersionMessageRef = null;
        }

        _onNewApiVersion = null;
        _onInitFinish = null;

        _domainName = "";
        _appName = "";
        _pushNotificationToken = "";

        _updating = false;
    }

    private void PrintTrack(string message)
    {
        Debug.Log("ApiManager: " + message);
    }

    public static VersionNumber GetCurrentApiVersionOnDevice()
    {
        VersionNumber result = VersionNumber.Parse(Application.version);
        if (result == null)
            result = new VersionNumber();

        return result;
    }

    public static bool IsNewApiVersion(string jsonString)
    {
        JsonData jsonData = null;

        try
        {
            jsonData = JsonMapper.ToObject(jsonString);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }

        return IsNewApiVersion(jsonData);
    }

    public static bool IsNewApiVersion(JsonData jsonData)
    {
        try
        {
            VersionNumber versionNumber = VersionNumber.Parse(jsonData["APIVer"].ToString());
            VersionNumber currentApiVersionOnDevice = GetCurrentApiVersionOnDevice();
            CompareVersionNumber compareResult =
                (CompareVersionNumber)currentApiVersionOnDevice.CompareTo(versionNumber);

            switch (compareResult)
            {
                case CompareVersionNumber.MajorGreater:
                case CompareVersionNumber.MinorGreater:
                case CompareVersionNumber.PacthGreater:
                case CompareVersionNumber.Error:
                    return true;

                case CompareVersionNumber.MajorLess:
                case CompareVersionNumber.MinorLess:
                case CompareVersionNumber.PatchLess:
                case CompareVersionNumber.Equal:
                    return false;

                default:
                    return true;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
    }
}