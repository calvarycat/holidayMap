using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionsUI : MonoBehaviour {

    public GameObject routeObj;
    public Transform rect;
    public GameObject pfLocale;
    float total = 0;
    public void InitRoute(List<OnlineMapsSkobblerDirectionsResult.Advisor> _routes)
    {
        Debug.Log("no khong vo row");
        total = 0;
        Utils.RemoveAllChildren(rect);
        for (int i = 0; i < _routes.Count; i++)
        {
            AddItem(_routes[i].Instruction, _routes[i].Distance.ToString() + "m");
            total += _routes[i].Distance;
        }
        Debug.Log(total);
	}
    public void InitRoute(List<OnlineMapsDirectionStep> _routes)
    {
        Utils.RemoveAllChildren(rect);
        for (int i = 0; i < _routes.Count; i++)
        {
            AddItem(_routes[i].stringInstructions, _routes[i].distance.ToString() + "m");
        }
    }
    public void AddItem(string _name, string _info)
    {
        GameObject _obj = Utils.Spawn(pfLocale, rect);
        RouteUnit _route= _obj.GetComponent<RouteUnit>();
        _route.Init(_name, _info);
    }
    public void Show(bool value)
    {
        routeObj.SetActive(value);
    }
    public void OnClickShowRoute()
    {
        Show(!routeObj.activeSelf);
    }
}
