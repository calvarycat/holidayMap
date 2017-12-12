using UnityEngine;
using System.Collections;
using System;
using LitJson;

public class GetServerTimeMessage
{
    private readonly string _domainName;
    private readonly string _authString;

    public string Url
    {
        get { return _domainName + "api/index/get-server-time" + _authString; }
    }

    public GetServerTimeMessage(string domainName, string authString)
    {
        _domainName = domainName;
        _authString = authString;
    }
}

public class GetServerTimeResponseMessage
{
    public double Time { get; private set; }

    public static GetServerTimeResponseMessage FromJson(string jsonString)
    {
        GetServerTimeResponseMessage result = new GetServerTimeResponseMessage();

        try
        {
            JsonData jsonData = JsonMapper.ToObject(jsonString);
            result.Time = double.Parse(jsonData["time"].ToString());
            return result;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }
}