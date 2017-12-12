using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using LitJson;

public class JsonInterface<T> where T : class
{
    public virtual string ToJson()
    {
        return JsonMapper.ToJson(this);
    }

    public override string ToString()
    {
        return JsonMapper.ToJson(this);
    }

    public static T FromJson(string paramJson)
    {
        try
        {
            return JsonMapper.ToObject<T>(paramJson);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }

    public static T FromJson(JsonData paramJson)
    {
        try
        {
            return FromJson(paramJson.ToJson());
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }

    public delegate T FromJsonDelegate(JsonData paramData);

    public static List<T> ArrayFromJson(JsonData paramJsonData, FromJsonDelegate paramFromJson)
    {
        try
        {
            if (paramJsonData == null)
                return null;

            List<T> data = new List<T>();

            for (int i = 0; i < paramJsonData.Count; i++)
            {
                T tmpData = paramFromJson(paramJsonData[i]);
                if (tmpData != null)
                {
                    data.Add(tmpData);
                }
            }

            return data;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }

    public static List<T> ArrayFromJson(string paramJson, FromJsonDelegate paramFromJson)
    {
        try
        {
            JsonData jsonData = JsonMapper.ToObject(paramJson);
            return ArrayFromJson(jsonData, paramFromJson);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }
}