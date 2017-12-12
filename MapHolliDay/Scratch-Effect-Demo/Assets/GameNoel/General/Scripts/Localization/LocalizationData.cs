using System;
using UnityEngine;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine.UI;
using LitJson;
using System.IO;

public class LocalizationData
{
    public static List<LocalizationConfig> config;

    public static void LoadLocalizationLocal()
    {
        LoadConfig();
        LoadFormFile(LocalizationPath.common);
        LoadFormFile(LocalizationPath.home);
        LoadFormFile(LocalizationPath.server);
    }

    public static void LoadLocalizationDLC()
    {
    }

    private static void LoadFormFile(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        if (textAsset != null)
            Localization.LoadCSV(textAsset, true);
    }

    private static void LoadFormFile(string path, string keyUnit)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        if (textAsset != null)
            Localization.LoadCSV(textAsset, keyUnit, true);
    }

    private static void LoadFormFileDLC(string path, string keyUnit)
    {
        Localization.LoadCSVDLC(Utils.LoadBytesFromFile(path), keyUnit, true);
    }

    private static void LoadConfig()
    {
        IEnumerable<LocalizationConfig> dataListFromCSV =
            CsvControll.LoadCSVDataFromFile<LocalizationConfig>(LocalizationPath.config);
        config = new List<LocalizationConfig>();
        int index = 1;
        foreach (LocalizationConfig datafromCSV in dataListFromCSV)
        {
            datafromCSV.index = index;
            config.Add(datafromCSV);
            index++;
        }
    }

    public static LocalizationConfig GetConfig(string id)
    {
        for (int i = 0; i < config.Count; i++)
        {
            if (config[i].id == id)
            {
                return config[i];
            }
        }
        return null;
    }
}

public class LocalizationConfig
{
    public string id;
    public string name;
    public int status;
    public int index;
}

public class LocalizationPath
{
    public const string common = "Localizations/Common";
    public const string home = "Localizations/Home";
    public const string config = "Localizations/Config";
    public const string server = "Localizations/Server";
}