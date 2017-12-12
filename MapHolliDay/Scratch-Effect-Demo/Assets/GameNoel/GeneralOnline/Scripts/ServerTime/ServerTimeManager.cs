using UnityEngine;
using System.Collections;
using System;
using LitJson;

public class ServerTimeManager : MonoSingleton<ServerTimeManager>
{
    public double BaseTime { get; private set; }

    public double CurrentTime
    {
        get { return BaseTime + Time.realtimeSinceStartup; }
    }

    public DateTime CurrentDateTime
    {
        get { return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(CurrentTime); }
    }

    private Action<bool> _onGetServerTimeFinish;

    public void Init(string domainName,
        string authString,
        Action<bool> onFinish = null)
    {
        _onGetServerTimeFinish = onFinish;
        GetServerTimeMessage message = new GetServerTimeMessage(domainName, authString);
        BaseOnline.Instance.WWW(message.Url, OnGetServerTimeResponse);
    }

    private void OnGetServerTimeResponse(string response)
    {
        if (!string.IsNullOrEmpty(response))
        {
            try
            {
                GetServerTimeResponseMessage result = GetServerTimeResponseMessage.FromJson(response);
                if (result != null)
                {
                    BaseTime = result.Time;
                    BaseTime -= Time.realtimeSinceStartup;
                    if (_onGetServerTimeFinish != null)
                        _onGetServerTimeFinish(true);
                }
                else
                {
                    if (_onGetServerTimeFinish != null)
                        _onGetServerTimeFinish(false);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                if (_onGetServerTimeFinish != null)
                    _onGetServerTimeFinish(false);
            }
        }
        else
        {
            if (_onGetServerTimeFinish != null)
                _onGetServerTimeFinish(false);
        }
    }
}