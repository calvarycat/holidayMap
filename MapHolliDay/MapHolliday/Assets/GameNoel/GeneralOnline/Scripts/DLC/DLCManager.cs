using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using LitJson;

public class DLCManager : MonoSingleton<DLCManager>
{
    public const string DlcVersion = "CurrentDLCVersionOnDevice";

    public static bool Tracking;

    private string _domainName;
    private string _authString;
    private string _password;
    private string _dlcPath;
    private Action<float> _onDownloading;
    private Action<bool> _onCheckAndDownloadFinish;

    private bool _updating;
    private bool _updatingOneVersion;
    private IEnumerator _checkVersionMessageRef;
    private IEnumerator _downloadAndUnzipRef;
    private IEnumerator _checkVersionMessageRef2;
    private IEnumerator _getDlcListMessageRef;
    private IEnumerator _downloadAndUpdateVersionByVersionRef;

    /// <summary>
    ///     Check dlc version then update to latest version
    /// </summary>
    /// <param name="domainName"></param>
    /// <param name="authString"></param>
    /// <param name="password"></param>
    /// <param name="dlcPath"></param>
    /// <param name="onDownloading">return percent of downloading</param>
    /// <param name="onCheckAndDownloadFinish">success or not</param>
    public void CheckAndDownload(string domainName,
        string authString,
        string password,
        string dlcPath,
        Action<float> onDownloading = null,
        Action<bool> onCheckAndDownloadFinish = null)
    {
#if UNITY_EDITOR
        if (Tracking)
        {
            PrintTrack(MethodBase.GetCurrentMethod().Name);
            PrintTrack("domainName: " + domainName);
            PrintTrack("authString: " + authString);
            PrintTrack("password: " + password);
            PrintTrack("dlcPath: " + dlcPath);
        }
#endif

        if (_updating)
        {
            Debug.LogError("Many update DLC request, call ForceStopUpdate in case you want");
            return;
        }

        _updating = true;

        _domainName = domainName;
        _authString = authString;
        _password = password;
        _dlcPath = dlcPath;
        _onDownloading = onDownloading;
        _onCheckAndDownloadFinish = onCheckAndDownloadFinish;

        CheckVersionMessage message = new CheckVersionMessage(_domainName, _authString);
        _checkVersionMessageRef = BaseOnline.Instance.WWWCoroutine(message.Url,
            OnCheckVersionMessageResponse);
        StartCoroutine(_checkVersionMessageRef);
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
            FinishUpdate(false);
            return;
        }

        CheckVersionResponseMessage responseMessage = CheckVersionResponseMessage.FromJson(response);
        if (responseMessage == null)
        {
            FinishUpdate(false);
            return;
        }

        if (!Directory.Exists(_dlcPath))
        {
            UpdateFull(responseMessage.UrlDlcFull);
            return;
        }

        VersionNumber currentDLCVersionOnDevice = GetCurrentDLCVersionOnDevice();
        CompareVersionNumber compareResult =
            (CompareVersionNumber)currentDLCVersionOnDevice.CompareTo(responseMessage.DlcVersion);

        // If dlc version on client greater than dlc version on server,
        // it's fatal error, must update full
        // If major version change, update full
        // else update version by version
        switch (compareResult)
        {
            case CompareVersionNumber.MajorGreater:
            case CompareVersionNumber.MinorGreater:
            case CompareVersionNumber.PacthGreater:
            case CompareVersionNumber.MajorLess:
                UpdateFull(responseMessage.UrlDlcFull);
                break;

            case CompareVersionNumber.MinorLess:
            case CompareVersionNumber.PatchLess:
                UpdateVersionByVersion();
                break;

            case CompareVersionNumber.Error:
                FinishUpdate(false);
                break;

            case CompareVersionNumber.Equal:
                FinishUpdate(true);
                break;
        }
    }

    private void UpdateFull(string urlDlcFull)
    {
#if UNITY_EDITOR
        if (Tracking)
        {
            PrintTrack(MethodBase.GetCurrentMethod().Name);
        }
#endif

        Utility.DeleteFolder(_dlcPath);

        _downloadAndUnzipRef = ZipManager.Instance.DownloadAndUnzipCoroutine(urlDlcFull,
            _password,
            _dlcPath,
            _onDownloading,
            OnUpdateFullFinish);
        StartCoroutine(_downloadAndUnzipRef);
    }

    private void OnUpdateFullFinish(bool success)
    {
#if UNITY_EDITOR
        if (Tracking)
        {
            PrintTrack(MethodBase.GetCurrentMethod().Name);
            PrintTrack("success: " + success);
        }
#endif

        _downloadAndUnzipRef = null;

        if (success)
        {
            CheckVersionMessage message = new CheckVersionMessage(_domainName, _authString);
            _checkVersionMessageRef2 = BaseOnline.Instance.WWWCoroutine(message.Url,
                OnCheckVersionMessageResponseAfterUpdateFull);
            StartCoroutine(_checkVersionMessageRef2);
        }
        else
        {
            FinishUpdate(false);
        }
    }

    private void OnCheckVersionMessageResponseAfterUpdateFull(string response)
    {
#if UNITY_EDITOR
        if (Tracking)
        {
            PrintTrack(MethodBase.GetCurrentMethod().Name);
            PrintTrack("response: " + response);
        }
#endif

        _checkVersionMessageRef2 = null;

        if (string.IsNullOrEmpty(response))
        {
            FinishUpdate(false);
            return;
        }

        CheckVersionResponseMessage responseMessage = CheckVersionResponseMessage.FromJson(response);
        if (responseMessage == null)
        {
            FinishUpdate(false);
            return;
        }

        SaveDLCVersionToDevice(responseMessage.DlcVersion);
        FinishUpdate(true);
    }

    private void UpdateVersionByVersion()
    {
#if UNITY_EDITOR
        if (Tracking)
        {
            PrintTrack(MethodBase.GetCurrentMethod().Name);
        }
#endif

        GetDlcListMessage message = new GetDlcListMessage(_domainName, _authString);
        _getDlcListMessageRef = BaseOnline.Instance.WWWCoroutine(message.Url,
            OnUpdateVersionByVersionResponse);
        StartCoroutine(_getDlcListMessageRef);
    }

    private void OnUpdateVersionByVersionResponse(string response)
    {
#if UNITY_EDITOR
        if (Tracking)
        {
            PrintTrack(MethodBase.GetCurrentMethod().Name);
            PrintTrack("response: " + response);
        }
#endif

        _getDlcListMessageRef = null;

        if (string.IsNullOrEmpty(response))
        {
            FinishUpdate(false);
            return;
        }

        GetDlcListResponseMessage responseMessage = GetDlcListResponseMessage.FromJson(response);
        if (responseMessage == null)
        {
            FinishUpdate(false);
            return;
        }

        VersionNumber currentDLCVersionOnDevice = GetCurrentDLCVersionOnDevice();

        int currentIndex = responseMessage.DlcVersionInfoList.FindIndex((DlcVersionInfo dlcVersionInfo) =>
        {
            if (dlcVersionInfo.Version.CompareTo(currentDLCVersionOnDevice) == 0)
                return true;
            return false;
        });

        _downloadAndUpdateVersionByVersionRef = DownloadAndUpdateVersionByVersion(currentIndex,
            responseMessage.DlcVersionInfoList);
        StartCoroutine(_downloadAndUpdateVersionByVersionRef);
    }

    private IEnumerator DownloadAndUpdateVersionByVersion(int currentIndex, List<DlcVersionInfo> dlcVersionInfoList)
    {
#if UNITY_EDITOR
        if (Tracking)
        {
            PrintTrack(MethodBase.GetCurrentMethod().Name);
            PrintTrack("currentIndex: " + currentIndex);
            PrintTrack("dlcVersionInfoList: " +
                       (dlcVersionInfoList != null ? JsonMapper.ToJson(dlcVersionInfoList) : null));
        }
#endif

        for (int i = currentIndex + 1; i < dlcVersionInfoList.Count; i++)
        {
            _updatingOneVersion = true;

            yield return ZipManager.Instance.DownloadAndUnzipCoroutine(dlcVersionInfoList[i].Url,
                _password,
                _dlcPath,
                _onDownloading,
                OnDownloadAndUpdateoOneVersionFinish);

            while (_updatingOneVersion)
            {
                yield return new WaitForSeconds(1);
            }
        }

        SaveDLCVersionToDevice(dlcVersionInfoList[dlcVersionInfoList.Count - 1].Version);
        _downloadAndUpdateVersionByVersionRef = null;
        _updatingOneVersion = false;
        FinishUpdate(true);
    }

    private void OnDownloadAndUpdateoOneVersionFinish(bool success)
    {
#if UNITY_EDITOR
        if (Tracking)
        {
            PrintTrack(MethodBase.GetCurrentMethod().Name);
            PrintTrack("success: " + success);
        }
#endif

        if (success)
        {
            _updatingOneVersion = false;
        }
        else
        {
            StopCoroutine(_downloadAndUpdateVersionByVersionRef);
            _downloadAndUpdateVersionByVersionRef = null;
            _updatingOneVersion = false;
            FinishUpdate(false);
        }
    }

    private VersionNumber GetCurrentDLCVersionOnDevice()
    {
#if UNITY_EDITOR
        if (Tracking)
        {
            PrintTrack(MethodBase.GetCurrentMethod().Name);
        }
#endif

        return VersionNumber.Parse(PlayerPrefs.GetString(DlcVersion, "0.0.0"));
    }

    private void SaveDLCVersionToDevice(VersionNumber versionNumber)
    {
#if UNITY_EDITOR
        if (Tracking)
        {
            PrintTrack(MethodBase.GetCurrentMethod().Name);
            PrintTrack("versionNumber: " + versionNumber);
        }
#endif

        PlayerPrefs.SetString(DlcVersion, versionNumber.ToString());
    }

    private void FinishUpdate(bool success)
    {
#if UNITY_EDITOR
        if (Tracking)
        {
            PrintTrack(MethodBase.GetCurrentMethod().Name);
            PrintTrack("success: " + success);
        }
#endif

        _updating = false;

        if (_onCheckAndDownloadFinish != null)
            _onCheckAndDownloadFinish(success);
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

        // Order of clearing object is importance
        if (_checkVersionMessageRef != null)
        {
            StopCoroutine(_checkVersionMessageRef);
            _checkVersionMessageRef = null;
        }

        if (_downloadAndUnzipRef != null)
        {
            StopCoroutine(_downloadAndUnzipRef);
            _downloadAndUnzipRef = null;
        }

        if (_checkVersionMessageRef2 != null)
        {
            StopCoroutine(_checkVersionMessageRef2);
            _checkVersionMessageRef2 = null;
        }

        if (_getDlcListMessageRef != null)
        {
            StopCoroutine(_getDlcListMessageRef);
            _getDlcListMessageRef = null;
        }

        if (_downloadAndUpdateVersionByVersionRef != null)
        {
            StopCoroutine(_downloadAndUpdateVersionByVersionRef);
            _downloadAndUpdateVersionByVersionRef = null;
        }

        _onDownloading = null;
        _onCheckAndDownloadFinish = null;

        _domainName = "";
        _authString = "";
        _password = "";
        _dlcPath = "";

        _updatingOneVersion = false;
        _updating = false;
    }

    private void PrintTrack(string message)
    {
        Debug.Log("DLCManager: " + message);
    }
}