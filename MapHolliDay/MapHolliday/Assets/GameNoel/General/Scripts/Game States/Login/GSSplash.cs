using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GSSplash : MonoBehaviour
{
    private int _oldApiclientid;
    private string _oldToken;

    private void Start()
    {
        Application.targetFrameRate = 60;
        Random.InitState(DateTime.UtcNow.Millisecond + DateTime.UtcNow.Second + DateTime.UtcNow.Minute);

        BaseOnline.Tracking = true;
        Preferences.LoadPreferences();

        LocalizationData.LoadLocalizationLocal();

        //Wait to display pathak icon
        Invoke("InitializeData", 0.2f);
    }

    private void InitializeData()
    {
        _oldApiclientid = Preferences.CurrentUserStatus.Apiclientid;
        _oldToken = Preferences.CurrentUserStatus.Token;

        ApiManager.Instance.Init(ClientMessage.domainName,
            "PathakApp",
            "",
            Preferences.CurrentUserStatus.Apiclientid,
            Preferences.CurrentUserStatus.Token,
            UserStatus.Secret,
            OnNewApiVersion,
            OnInitApiFinish);
    }

    private void OnInitApiFinish(int apiClientId, string token)
    {
        if (apiClientId != -1)
        {
            if (_oldApiclientid != apiClientId ||
                _oldToken != token)
            {
                Preferences.CurrentUserStatus.IsLogged = false;
            }

            Preferences.CurrentUserStatus.Apiclientid = apiClientId;
            Preferences.CurrentUserStatus.Token = token;
            Preferences.SaveUserStatus();

            Finish();
        }
        else
        {
            if (PopupManager.Instance != null)
            {
                string message = Localization.Get("C_RetryOnError");
                PopupManager.Instance.InitYesNoPopUp(message, InitializeData, ExitApp);
            }
        }
    }

    private void Finish()
    {
        LocalizationData.LoadLocalizationDLC();

        if (!Preferences.CurrentUserStatus.IsLogged)
        {
            SceneManager.LoadScene(SceneName.Login);
        }
        else
        {
            SceneManager.LoadScene(SceneName.Home);
        }
    }

    private void ExitApp()
    {
        Application.Quit();
    }

    private void OnNewApiVersion(VersionNumber currentVersion, VersionNumber newVersion)
    {
        if (PopupManager.Instance != null)
        {
            string message = Localization.Get("C_UpdateVersion");
            PopupManager.Instance.InitInfoPopUp(message, ExitApp);
        }
    }
}