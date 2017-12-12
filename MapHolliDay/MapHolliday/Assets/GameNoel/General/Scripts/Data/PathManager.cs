using UnityEngine;
using System.Collections;
using System.IO;

public static class PathManager
{
    private static string _dlc;

    public static string DLC
    {
        get
        {
            _dlc = Application.persistentDataPath + "/DLC/";

            if (!Directory.Exists(_dlc))
            {
                Directory.CreateDirectory(_dlc);
            }
            return _dlc;
        }
    }
    public static string ListQuest
    {

        get { return "Jsonfile/ListQuest"; }
    }
    public static string ListQuestScratch
    {

        get { return "Jsonfile/ListQuestScratch"; }
    }

    public static string ListDragAndDrop
    {

        get { return "Jsonfile/ListDragAndDrop"; }
    }
}

public class GameTags
{
    public const string SettingDataKey = "SettingDataKey";
    public const string UserinfoDataKey = "UserinfoDataKey";
    public const string UserStatusDataKey = "UserStatusDataKey";
}

public class SceneName
{
    public const string Splash = "Splash";
    public const string Login = "Login";
    public const string Home = "Home";
    public const string Empty = "Empty";
}