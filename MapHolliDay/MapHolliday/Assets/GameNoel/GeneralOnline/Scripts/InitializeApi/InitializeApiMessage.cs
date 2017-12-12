using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;

public class InitializeAppMessage
{
    public string name { get; private set; }
    public string api_version { get; private set; }
    public string push_token { get; private set; }

    public string url
    {
        get { return _domainName + "api/index/init"; }
    }

    public string platform_name
    {
        get { return SystemInfo.deviceModel; }
    }

    public string platform_version
    {
        get { return SystemInfo.operatingSystem; }
    }

    public string uid
    {
        get { return SystemInfo.deviceUniqueIdentifier; }
    }

    private readonly string _domainName;

    public InitializeAppMessage(string domainName,
        string appName,
        VersionNumber deviceApiVersion,
        string pushNotificationToken)
    {
        _domainName = domainName;
        name = appName;
        api_version = deviceApiVersion.ToString();
        push_token = pushNotificationToken;
    }
}

public class InitializeAppResponseMessage
{
    public string Status { get; private set; }
    public VersionNumber ApiVersion { get; private set; }
    public string Token { get; private set; }
    public int ApiclientId { get; private set; }

    public static InitializeAppResponseMessage FromJson(string jsonString)
    {
        InitializeAppResponseMessage result = new InitializeAppResponseMessage();

        try
        {
            JsonData jsonData = JsonMapper.ToObject(jsonString);
            result.Status = jsonData["status"].ToString();
            result.ApiVersion = VersionNumber.Parse(jsonData["apiversion"].ToString());
            result.Token = jsonData["token"].ToString();
            result.ApiclientId = int.Parse(jsonData["apiclientid"].ToString());
            return result;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }
}