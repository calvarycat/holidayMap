using System.Collections.Generic;
using LitJson;
using UnityEngine;

public static class Preferences
{
    public static void LoadPreferences()
    {
        LoadSetting();
        LoadUserinfo();
        LoadUserStatus();
    }

    /// <summary>
    ///     Setting
    /// </summary>
    public static Setting CurrentSetting;

    private static Setting LoadSetting()
    {
        CurrentSetting = SaveGameManager.LoadData<Setting>(GameTags.SettingDataKey);
        if (CurrentSetting == null)
        {
            CurrentSetting = new Setting();
            SaveSetting();
        }
        return CurrentSetting;
    }

    public static void SaveSetting()
    {
        SaveGameManager.SaveData(GameTags.SettingDataKey, CurrentSetting);
    }

    /// <summary>
    ///     User info
    /// </summary>
    public static UserInfo CurrentUserinfo;

    private static UserInfo LoadUserinfo()
    {
        CurrentUserinfo = SaveGameManager.LoadData<UserInfo>(GameTags.UserinfoDataKey);
        if (CurrentUserinfo == null)
        {
            CurrentUserinfo = new UserInfo();
            SaveUserInfo();
        }

        return CurrentUserinfo;
    }

    public static void SaveUserInfo()
    {
        SaveGameManager.SaveData(GameTags.UserinfoDataKey, CurrentUserinfo);
    }

    /// <summary>
    ///     User status
    /// </summary>
    public static UserStatus CurrentUserStatus;

    private static UserStatus LoadUserStatus()
    {
        CurrentUserStatus = SaveGameManager.LoadData<UserStatus>(GameTags.UserStatusDataKey);
        if (CurrentUserStatus == null)
        {
            CurrentUserStatus = new UserStatus();
            SaveUserStatus();
        }
        return CurrentUserStatus;
    }

    public static void SaveUserStatus()
    {
        SaveGameManager.SaveData(GameTags.UserStatusDataKey, CurrentUserStatus);
    }
}

public class Setting
{
    public bool EnableSound;

    public Setting()
    {
        EnableSound = true;
    }
}

public class UserInfo
{
    public int Id;
    public string Name;

    public UserInfo()
    {
        Id = 0;
        Name = "";
    }
}

public class UserStatus
{
    [JsonIgnore]
    public const string Secret = "J82jHksn-2jsHjd823-=82HHks8nmckJd83jJ";

    public bool IsLogged;
    public string Token;
    public int Apiclientid;

    [JsonIgnore]
    public string Sign
    {
        get { return Utility.GetMd5Hash(Token + Apiclientid + Secret); }
    }

    public UserStatus()
    {
        Token = "";
        Apiclientid = 0;
    }
}