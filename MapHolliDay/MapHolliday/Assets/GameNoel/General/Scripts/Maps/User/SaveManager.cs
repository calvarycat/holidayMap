using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

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
    }

    private void Init()
    {
    }

    private Action<bool, string> _callback;
    private int _highestScore;

    public void LoadScoreHistory()
    {
        string jsonString;

        if (FBManager.Instance.IsLogged)
            jsonString = PlayerPrefs.GetString(CurrentUser.User.id, "");
        else
            jsonString = PlayerPrefs.GetString(PlayerPrefsContance.GenericUser, "");

        if (!string.IsNullOrEmpty(jsonString))
        {
            try
            {
                CurrentUser.ScoreHistory = JsonMapper.ToObject<List<int>>(jsonString);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                CurrentUser.ScoreHistory = new List<int>();
            }
        }
        else
        {
            CurrentUser.ScoreHistory = new List<int>();
        }
    }

    public void SaveScoreHistory()
    {
        if (FBManager.Instance.IsLogged)
            PlayerPrefs.SetString(CurrentUser.User.id, JsonMapper.ToJson(CurrentUser.ScoreHistory));
        else
            PlayerPrefs.SetString(PlayerPrefsContance.GenericUser, JsonMapper.ToJson(CurrentUser.ScoreHistory));
    }

    public void SaveScoreHistoryFromGenericUserToCurrentUser(Action<bool, string> callback)
    {
        _callback = callback;

        if (!FBManager.Instance.IsLogged)
        {
            if (_callback != null)
                _callback(false, "User not login yet");
            return;
        }

        string jsonString = PlayerPrefs.GetString(PlayerPrefsContance.GenericUser, "");
        List<int> tempScoreHistory;

        if (!string.IsNullOrEmpty(jsonString))
        {
            try
            {
                tempScoreHistory = JsonMapper.ToObject<List<int>>(jsonString);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                tempScoreHistory = new List<int>();
            }
        }
        else
        {
            tempScoreHistory = new List<int>();
        }

        PlayerPrefs.SetString(CurrentUser.User.id, JsonMapper.ToJson(tempScoreHistory));
        PlayerPrefs.SetString(PlayerPrefsContance.GenericUser, "");

        _highestScore = 0;
        for (int i = 0; i < tempScoreHistory.Count; i++)
        {
            if (_highestScore < tempScoreHistory[i])
            {
                _highestScore = tempScoreHistory[i];
            }
        }

        FBManager.Instance.SendScore(_highestScore, OnSendScoreCB);
    }

    private void OnSendScoreCB(bool isSuccess, string message)
    {
        if (isSuccess)
        {
            CurrentUser.ScoreOnFacebook = _highestScore;
            if (_callback != null)
                _callback(true, "Transfer score success");
        }
        else
        {
            CurrentUser.ScoreOnFacebook = 0;
            if (_callback != null)
                _callback(false, "Transfer score failed");
        }
    }
}