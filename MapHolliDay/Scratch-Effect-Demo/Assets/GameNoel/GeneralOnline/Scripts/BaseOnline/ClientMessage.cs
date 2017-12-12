using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using LitJson;

#region BaseMessage

public class ClientMessage
{
    public static readonly string domainName = "http://stage-pathak.ngaydautien.vn/";
    //public static readonly string domainName = "http://dev-pathak.ngaydautien.vn/";

    public static string authString
    {
        get
        {
            return "?apiclientid=" + Preferences.CurrentUserStatus.Apiclientid
                   + "&token=" + Preferences.CurrentUserStatus.Token
                   + "&sign=" + Preferences.CurrentUserStatus.Sign;
        }
    }
}

public class ServerMessage
{
    public string APIVer;
    public string status;
    public object message;
}

#endregion