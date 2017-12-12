using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using LitJson;

public static class SaveGameManager
{
    public static T LoadData<T>(string paramKey) where T : class
    {
        string jsonData = LoadStringData(paramKey);
        if (string.IsNullOrEmpty(jsonData))
            return null;
        try
        {
            return JsonMapper.ToObject<T>(jsonData);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return null;
        }
    }

    public static bool SaveData<T>(string paramKey, T paramData) where T : class
    {
        string jsonData = "";
        if (paramData != null)
            jsonData = JsonMapper.ToJson(paramData);
        return SaveData(paramKey, jsonData);
    }

    private static bool SaveData(string paramKey, string paramData)
    {
        try
        {
            PlayerPrefs.SetString(paramKey, paramData);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return false;
        }
    }

    public static bool SaveData(string paramKey, int paramData)
    {
        try
        {
            PlayerPrefs.SetInt(paramKey, paramData);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return false;
        }
    }

    private static bool SaveData(string paramKey, float paramData)
    {
        try
        {
            PlayerPrefs.SetFloat(paramKey, paramData);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return false;
        }
    }

    public static int LoadIntData(string paramKey)
    {
        try
        {
            return PlayerPrefs.GetInt(paramKey, -1);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return -1;
        }
    }

    private static float LoadFloatData(string paramKey)
    {
        try
        {
            return PlayerPrefs.GetFloat(paramKey, -1);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return -1;
        }
    }

    private static string LoadStringData(string paramKey)
    {
        try
        {
            return PlayerPrefs.GetString(paramKey, null);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return null;
        }
    }
}