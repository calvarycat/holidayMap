using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using System.Reflection;
using LitJson;
using UnityEngine.Networking;

public class BaseOnline : MonoSingleton<BaseOnline>
{
    public const string Success = "success";
    public const string Error = "error";
    public const string RetryOnLostConnectionKey = "C_RetryOnLostConnection";

    public static readonly Dictionary<string, string> JsonHeader = new Dictionary<string, string>
    {
        {"Content-Type", "application/json"}
    };

    public static readonly Dictionary<string, string> MultipartHeader = new Dictionary<string, string>
    {
        {"Content-Type", "multipart/form-data"}
    };

    public static bool Tracking;
    public static float DefaultTimeOut = 30;

    #region WWW

    public void WWW(string url,
        Action<string> onFinish = null,
        Action<float> onUploadDownloading = null)
    {
        StartCoroutine(WWWCoroutine(url, null, DefaultTimeOut, onFinish, onUploadDownloading));
    }

    public void WWW(string url,
        int timeOut,
        Action<string> onFinish = null,
        Action<float> onUploadDownloading = null)
    {
        StartCoroutine(WWWCoroutine(url, null, timeOut, onFinish, onUploadDownloading));
    }

    public void WWW(string url,
        object data,
        Action<string> onFinish = null,
        Action<float> onUploadDownloading = null)
    {
        StartCoroutine(WWWCoroutine(url, data, DefaultTimeOut, onFinish, onUploadDownloading));
    }

    public void WWW(string url,
        object data,
        float timeOut,
        Action<string> onFinish = null,
        Action<float> onUploadDownloading = null)
    {
        StartCoroutine(WWWCoroutine(url, data, timeOut, onFinish, onUploadDownloading));
    }

    public IEnumerator WWWCoroutine(string url,
        Action<string> onFinish = null,
        Action<float> onUploadDownloading = null)
    {
        yield return WWWCoroutine(url, null, DefaultTimeOut, onFinish, onUploadDownloading);
    }

    public IEnumerator WWWCoroutine(string url,
        float timeOut,
        Action<string> onFinish = null,
        Action<float> onUploadDownloading = null)
    {
        yield return WWWCoroutine(url, null, timeOut, onFinish, onUploadDownloading);
    }

    public IEnumerator WWWCoroutine(string url,
        object data,
        Action<string> onFinish = null,
        Action<float> onUploadDownloading = null)
    {
        yield return WWWCoroutine(url, data, DefaultTimeOut, onFinish, onUploadDownloading);
    }

    public IEnumerator WWWCoroutine(string url,
        object data,
        float timeOut,
        Action<string> onFinish = null,
        Action<float> onUploadDownloading = null)
    {
#if UNITY_EDITOR
        if (Tracking)
        {
            PrintTrack(MethodBase.GetCurrentMethod().Name);
            PrintTrack("url: " + url);
            PrintTrack("data: " + (data != null ? JsonMapper.ToJson(data) : null));
        }
#endif

#if VIRTUAL_SERVER
        if (VirtualServer.Instance.IsContainUrl(url))
        {
            if (onFinish != null)
            {
                onFinish(VirtualServer.Instance.Send(url, data));
            }
            yield break;
        }
#endif
        if (data != null)
        {
            WWWForm wwwForm = data as WWWForm;
            if (wwwForm != null)
            {
                // Header "multipart/form-data" added by default
                WWW www = new WWW(url, wwwForm);
                yield return WWWCoroutineCore(www, timeOut, onFinish, onUploadDownloading);
            }
            else
            {
                string dataSendString = JsonMapper.ToJson(data);
                byte[] dataSend = Encoding.UTF8.GetBytes(dataSendString);
                WWW www = new WWW(url, dataSend, JsonHeader);
                yield return WWWCoroutineCore(www, timeOut, onFinish, onUploadDownloading);
            }
        }
        else
        {
            WWW www = new WWW(url, null, JsonHeader);
            yield return WWWCoroutineCore(www, timeOut, onFinish, onUploadDownloading);
        }
    }

    private IEnumerator WWWCoroutineCore(WWW www,
        float timeOut,
        Action<string> onFinish,
        Action<float> onUploadDownloading)
    {
        float startTime = Time.realtimeSinceStartup;

        if (timeOut > 0)
        {
            while (Time.realtimeSinceStartup - startTime <= timeOut)
            {
                if (www.isDone)
                    break;

                if (onUploadDownloading != null)
                    onUploadDownloading((www.uploadProgress + www.progress) / 2f);

                yield return null;
            }
        }
        else
        {
            while (!www.isDone)
            {
                if (onUploadDownloading != null)
                    onUploadDownloading((www.uploadProgress + www.progress) / 2f);

                yield return null;
            }
        }

        if (onUploadDownloading != null)
            onUploadDownloading((www.uploadProgress + www.progress) / 2f);

#if UNITY_EDITOR
        if (Tracking)
        {
            PrintTrack("www.isDone: " + www.isDone);
            if (www.isDone)
                PrintTrack("www.text: <color=yellow>" + www.text + "</color>");
        }
#endif

        if (www.isDone)
        {
            if (onFinish != null)
                onFinish(www.text);
        }
        else
        {
            if (onFinish != null)
                onFinish("");
        }

        www.Dispose();
    }

    #endregion

    #region UnityWebRequest

    public void Get(string url,
        Action<string> onFinish = null,
        Action<float> onUploadDownloading = null)
    {
        StartCoroutine(GetCoroutine(url, DefaultTimeOut, onFinish, onUploadDownloading));
    }

    public void Get(string url,
        float timeOut,
        Action<string> onFinish = null,
        Action<float> onUploadDownloading = null)
    {
        StartCoroutine(GetCoroutine(url, timeOut, onFinish, onUploadDownloading));
    }

    public IEnumerator GetCoroutine(string url,
        Action<string> onFinish = null,
        Action<float> onUploadDownloading = null)
    {
        yield return GetCoroutine(url, DefaultTimeOut, onFinish, onUploadDownloading);
    }

    public IEnumerator GetCoroutine(string url,
        float timeOut,
        Action<string> onFinish = null,
        Action<float> onUploadDownloading = null)
    {
#if UNITY_EDITOR
        if (Tracking)
        {
            PrintTrack(MethodBase.GetCurrentMethod().Name);
            PrintTrack("url: " + url);
        }
#endif

#if VIRTUAL_SERVER
        if (VirtualServer.Instance.IsContainUrl(url))
        {
            if (onFinish != null)
            {
                onFinish(VirtualServer.Instance.Send(url));
            }
            yield break;
        }
#endif

        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return WebRequestCoroutineCore(www, timeOut, onFinish, onUploadDownloading);
    }

    public void Post(string url,
        object data,
        Action<string> onFinish = null,
        Action<float> onUploadDownloading = null)
    {
        StartCoroutine(PostCoroutine(url, data, DefaultTimeOut, onFinish, onUploadDownloading));
    }

    public void Post(string url,
        object data,
        float timeOut,
        Action<string> onFinish = null,
        Action<float> onUploadDownloading = null)
    {
        StartCoroutine(PostCoroutine(url, data, timeOut, onFinish, onUploadDownloading));
    }

    public IEnumerator PostCoroutine(string url,
        object data,
        Action<string> onFinish = null,
        Action<float> onUploadDownloading = null)
    {
        yield return PostCoroutine(url, data, DefaultTimeOut, onFinish, onUploadDownloading);
    }

    public IEnumerator PostCoroutine(string url,
        object data,
        float timeOut,
        Action<string> onFinish = null,
        Action<float> onUploadDownloading = null)
    {
#if UNITY_EDITOR
        if (Tracking)
        {
            PrintTrack(MethodBase.GetCurrentMethod().Name);
            PrintTrack("url: " + url);
            PrintTrack("data: " + JsonMapper.ToJson(data));
        }
#endif

#if VIRTUAL_SERVER
        if (VirtualServer.Instance.IsContainUrl(url))
        {
            if (onFinish != null)
            {
                onFinish(VirtualServer.Instance.Send(url, data));
            }
            yield break;
        }
#endif

        WWWForm wwwForm = data as WWWForm;
        if (wwwForm != null)
        {
            UnityWebRequest www = UnityWebRequest.Post(url, wwwForm);
            www.SetRequestHeader("Content-Type", "multipart/form-data");
            yield return WebRequestCoroutineCore(www, timeOut, onFinish, onUploadDownloading);
        }
        else
        {
            string dataSendString = JsonMapper.ToJson(data);
            byte[] dataSendRaw = Encoding.UTF8.GetBytes(dataSendString);
            UnityWebRequest www = UnityWebRequest.Post(url, "POST");
            www.uploadHandler = new UploadHandlerRaw(dataSendRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            yield return WebRequestCoroutineCore(www, timeOut, onFinish, onUploadDownloading);
        }
    }

    public void Put(string url,
        byte[] data,
        Action<string> onFinish = null,
        Action<float> onUploadDownloading = null)
    {
        StartCoroutine(PutCoroutine(url, data, DefaultTimeOut, onFinish, onUploadDownloading));
    }

    public void Put(string url,
        byte[] data,
        float timeOut,
        Action<string> onFinish = null,
        Action<float> onUploadDownloading = null)
    {
        StartCoroutine(PutCoroutine(url, data, timeOut, onFinish, onUploadDownloading));
    }

    public IEnumerator PutCoroutine(string url,
        byte[] data,
        Action<string> onFinish = null,
        Action<float> onUploadDownloading = null)
    {
        yield return PutCoroutine(url, data, DefaultTimeOut, onFinish, onUploadDownloading);
    }

    public IEnumerator PutCoroutine(string url,
        byte[] data,
        float timeOut,
        Action<string> onFinish = null,
        Action<float> onUploadDownloading = null)
    {
#if UNITY_EDITOR
        if (Tracking)
        {
            PrintTrack(MethodBase.GetCurrentMethod().Name);
            PrintTrack("url: " + url);
        }
#endif

#if VIRTUAL_SERVER
        if (VirtualServer.Instance.IsContainUrl(url))
        {
            if (onFinish != null)
            {
                onFinish(VirtualServer.Instance.Send(url, data));
            }
            yield break;
        }
#endif

        UnityWebRequest www = UnityWebRequest.Put(url, data);
        yield return WebRequestCoroutineCore(www, timeOut, onFinish, onUploadDownloading);
    }

    public void Delete(string url,
        Action<string> onFinish = null,
        Action<float> onUploadDownloading = null)
    {
        StartCoroutine(DeleteCoroutine(url, DefaultTimeOut, onFinish, onUploadDownloading));
    }

    public void Delete(string url,
        float timeOut,
        Action<string> onFinish = null,
        Action<float> onUploadDownloading = null)
    {
        StartCoroutine(DeleteCoroutine(url, timeOut, onFinish, onUploadDownloading));
    }

    public IEnumerator DeleteCoroutine(string url,
        Action<string> onFinish = null,
        Action<float> onUploadDownloading = null)
    {
        yield return DeleteCoroutine(url, DefaultTimeOut, onFinish, onUploadDownloading);
    }

    public IEnumerator DeleteCoroutine(string url,
        float timeOut,
        Action<string> onFinish = null,
        Action<float> onUploadDownloading = null)
    {
#if UNITY_EDITOR
        if (Tracking)
        {
            PrintTrack(MethodBase.GetCurrentMethod().Name);
            PrintTrack("url: " + url);
        }
#endif

#if VIRTUAL_SERVER
        if (VirtualServer.Instance.IsContainUrl(url))
        {
            if (onFinish != null)
            {
                onFinish(VirtualServer.Instance.Send(url));
            }
            yield break;
        }
#endif

        UnityWebRequest www = UnityWebRequest.Delete(url);
        yield return WebRequestCoroutineCore(www, timeOut, onFinish, onUploadDownloading);
    }

    public void Head(string url,
        Action<string> onFinish = null,
        Action<float> onUploadDownloading = null)
    {
        StartCoroutine(HeadCoroutine(url, DefaultTimeOut, onFinish, onUploadDownloading));
    }

    public void Head(string url,
        float timeOut,
        Action<string> onFinish = null,
        Action<float> onUploadDownloading = null)
    {
        StartCoroutine(HeadCoroutine(url, timeOut, onFinish, onUploadDownloading));
    }

    public IEnumerator HeadCoroutine(string url,
        Action<string> onFinish = null,
        Action<float> onUploadDownloading = null)
    {
        yield return HeadCoroutine(url, DefaultTimeOut, onFinish, onUploadDownloading);
    }

    public IEnumerator HeadCoroutine(string url,
        float timeOut,
        Action<string> onFinish = null,
        Action<float> onUploadDownloading = null)
    {
#if UNITY_EDITOR
        if (Tracking)
        {
            PrintTrack(MethodBase.GetCurrentMethod().Name);
            PrintTrack("url: " + url);
        }
#endif

#if VIRTUAL_SERVER
        if (VirtualServer.Instance.IsContainUrl(url))
        {
            if (onFinish != null)
            {
                onFinish(VirtualServer.Instance.Send(url));
            }
            yield break;
        }
#endif

        UnityWebRequest www = UnityWebRequest.Head(url);
        yield return WebRequestCoroutineCore(www, timeOut, onFinish, onUploadDownloading);
    }

    private IEnumerator WebRequestCoroutineCore(UnityWebRequest www,
        float timeOut,
        Action<string> onFinish,
        Action<float> onUploadDownloading)
    {
        float startTime = Time.realtimeSinceStartup;
        www.Send();

        if (timeOut > 0)
        {
            while (Time.realtimeSinceStartup - startTime <= timeOut)
            {
                if (www.isDone)
                    break;

                if (onUploadDownloading != null)
                    onUploadDownloading((www.uploadProgress + www.downloadProgress) / 2f);

                yield return null;
            }
        }
        else
        {
            while (!www.isDone)
            {
                if (onUploadDownloading != null)
                    onUploadDownloading((www.uploadProgress + www.downloadProgress) / 2f);

                yield return null;
            }
        }

        if (onUploadDownloading != null)
            onUploadDownloading((www.uploadProgress + www.downloadProgress) / 2f);

#if UNITY_EDITOR
        if (Tracking)
        {
            PrintTrack("www.isDone: " + www.isDone);
            if (www.isDone)
                PrintTrack("www.downloadHandler.text: <color=yellow>" + www.downloadHandler.text + "</color>");
        }
#endif

        if (www.isDone)
        {
            if (onFinish != null)
                onFinish(www.downloadHandler.text);
        }
        else
        {
            if (onFinish != null)
                onFinish("");
        }

        www.Dispose();
    }

    #endregion

    private void PrintTrack(string message)
    {
        Debug.Log("BaseOnline: " + message);
    }

    public static bool IsSuccess(string jsonString)
    {
        JsonData jsonData = null;

        try
        {
            jsonData = JsonMapper.ToObject(jsonString);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }

        return IsSuccess(jsonData);
    }

    public static bool IsSuccess(JsonData jsonData)
    {
        try
        {
            if (jsonData["status"].ToString() == Success)
                return true;
            return false;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
    }
}