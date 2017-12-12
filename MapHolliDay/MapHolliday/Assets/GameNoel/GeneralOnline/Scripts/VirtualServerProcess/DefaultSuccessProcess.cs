using System;
using UnityEngine;
using System.Collections;
using LitJson;

public class DefaultSuccessProcess : IVirtualServerProcess
{
    public string ProcessData(string url, object inputData)
    {
        ServerMessage serverMessage = new ServerMessage();
        serverMessage.APIVer = "1.0.0";
        serverMessage.status = BaseOnline.Success;
        serverMessage.message = "";
        return JsonMapper.ToJson(serverMessage);
    }
}