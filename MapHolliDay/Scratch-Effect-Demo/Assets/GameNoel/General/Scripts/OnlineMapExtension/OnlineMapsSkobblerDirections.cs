using System;
using System.Collections;
using System.Text;
using UnityEngine;

/// <summary>
///     This class is used to search for a route by address or coordinates.\n
///     You can create a new instance using OnlineMapsSkobblerDirections.Find.\n
///     https://developer.skobbler.com/getting-started/web#sec2
/// </summary>
public class OnlineMapsSkobblerDirections : OnlineMapsTextWebService
{
    protected OnlineMapsSkobblerDirections()
    {
    }

    protected OnlineMapsSkobblerDirections(string start, string destination, string key)
    {
        _status = OnlineMapsQueryStatus.downloading;
        StringBuilder url = new StringBuilder();
        url.AppendFormat(
            "http://{0}.tor.skobbler.net/tor/RSngx/calcroute/json/20_5/en/{0}?start={1}&dest={2}&profile=carShortest&advice=yes",
            key,
            start,
            destination);

        Debug.Log(url);

        www = OnlineMapsUtils.GetWWW(url);
        www.OnComplete += OnRequestComplete;
    }

    public static OnlineMapsSkobblerDirections Find(Vector2 start, Vector2 destination, string key)
    {
        return new OnlineMapsSkobblerDirections(start.y + "," + start.x,
            destination.y + "," + destination.x,
            key);
    }
}