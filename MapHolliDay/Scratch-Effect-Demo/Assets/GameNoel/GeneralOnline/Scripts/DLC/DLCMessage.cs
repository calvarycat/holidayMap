using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class CheckVersionMessage
{
    private readonly string _domainName;
    private readonly string _authString;

    public string Url
    {
        get { return _domainName + "api/index/check-version" + _authString; }
    }

    public CheckVersionMessage(string domainName, string authString)
    {
        _domainName = domainName;
        _authString = authString;
    }
}

public class CheckVersionResponseMessage
{
    public string Status { get; private set; }
    public VersionNumber ApiVersion { get; private set; }
    public VersionNumber DlcVersion { get; private set; }
    public string UrlDlcFull { get; private set; }

    public static CheckVersionResponseMessage FromJson(string jsonString)
    {
        CheckVersionResponseMessage result = new CheckVersionResponseMessage();

        try
        {
            JsonData jsonData = JsonMapper.ToObject(jsonString);
            result.Status = jsonData["status"].ToString();
            result.ApiVersion = VersionNumber.Parse(jsonData["APIVer"].ToString());
            result.DlcVersion = VersionNumber.Parse(jsonData["dlc"].ToString());
            result.UrlDlcFull = jsonData["dlcFull"].ToString();
            return result;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }
}

public class GetDlcListMessage
{
    private readonly string _domainName;
    private readonly string _authString;

    public string Url
    {
        get { return _domainName + "api/index/get-dlc-list" + _authString; }
    }

    public GetDlcListMessage(string domainName, string authString)
    {
        _domainName = domainName;
        _authString = authString;
    }
}

public class GetDlcListResponseMessage
{
    public List<DlcVersionInfo> DlcVersionInfoList { get; private set; }

    public static GetDlcListResponseMessage FromJson(string jsonString)
    {
        GetDlcListResponseMessage result = new GetDlcListResponseMessage();

        try
        {
            JsonData jsonData = JsonMapper.ToObject(jsonString);
            result.DlcVersionInfoList = new List<DlcVersionInfo>();

            for (int i = 0; i < jsonData["message"].Count; i++)
            {
                result.DlcVersionInfoList.Add(DlcVersionInfo.FromJson(jsonData["message"][i].ToJson()));
            }

            result.DlcVersionInfoList.Sort(DlcVersionInfo.SortCore);

            return result;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }
}

public class DlcVersionInfo
{
    public VersionNumber Version { get; private set; }
    public string Url { get; private set; }

    public static DlcVersionInfo FromJson(string jsonString)
    {
        DlcVersionInfo result = new DlcVersionInfo();

        try
        {
            JsonData jsonData = JsonMapper.ToObject(jsonString);
            result.Version = VersionNumber.Parse(jsonData["version"].ToString());
            result.Url = jsonData["url"].ToString();
            return result;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }

    public static int SortCore(DlcVersionInfo dlcVersionInfo1, DlcVersionInfo dlcVersionInfo2)
    {
        return dlcVersionInfo1.Version.CompareTo(dlcVersionInfo2.Version);
    }
}