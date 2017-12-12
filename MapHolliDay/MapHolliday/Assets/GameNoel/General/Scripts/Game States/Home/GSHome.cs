using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GSHome : GSTemplate
{
    public static GSHome Instance { get; private set; }


    public LocaleLable FromLocale;
    public LocaleLable ToLocale;
    
    public Shader TileShader;

    private OnlineMaps _api;
    private OnlineMapsMarker _searchMarker;

    public LocaleSearch localeSearch;
    public LocaleCurrent localeCurrent;
    public GameObject localeObj;
    public DirectionsUI directionsObj;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    protected override void Init()
    {
        base.Init();
     //   localeSearch.Show(false);
    //   localeCurrent.Show(false);
       // localeObj.SetActive(true);
     //   directionsObj.gameObject.SetActive(false);
        OnlineMapsLocationService.instance.StartLocationService();

        _api = OnlineMaps.instance;
        _api.labels = true;
        _api.traffic = false;
    }
    public void ClickHideDirection()
    {
        localeObj.SetActive(true);
        directionsObj.gameObject.SetActive(false);
    }
    public void OnUnitClickDirections(OnlineMapsOSMNominatimResult data)
    {
        localeObj.SetActive(false);
        directionsObj.gameObject.SetActive(true);
        ToLocale.Init(data);
    }
    public void LockPositionClick(OnlineMapsOSMNominatimResult data)
    {
        SetSearchMarker(data.location, data.display_name);
        if (_api.zoom < 13)
            _api.zoom = 13;
        _api.position = data.location;
        _api.Redraw();
    }
    private void SetSearchMarker(Vector2 position, string label)
    {
        if (_searchMarker == null)
        {
            Debug.Log("add search marker");
            _searchMarker = _api.AddMarker(position, label);
        }
        else
        {
            _searchMarker.position = position;
            _searchMarker.label = label;
        }

    }

    public void OnDirectionButtonClick()
    {
        if (FromLocale.data ==null|| ToLocale.data == null)
            return;
        PopupManager.Instance.ShowLoading();
        // Begin to search a route from Los Angeles to the specified coordinates.
        OnlineMapsSkobblerDirections query = OnlineMapsSkobblerDirections.Find(FromLocale.data.location, ToLocale.data.location, "bc7b4da77e971c12cb0e069bffcf2771");
        // Specifies that search results must be sent to OnFindDirectionComplete.
        query.OnComplete += OnFindDirectionComplete;
    }
    public void OnDirectionGoogleButtonClick()
    {
        if (FromLocale.data == null || ToLocale.data == null)
            return;
        PopupManager.Instance.ShowLoading();
        // Begin to search a route from Los Angeles to the specified coordinates.
        OnlineMapsGoogleDirections query
            = OnlineMapsGoogleDirections.Find(FromLocale.data.location, ToLocale.data.location);
        // Specifies that search results must be sent to OnFindDirectionComplete.
        query.OnComplete += OnFindDirectionGoogleComplete;
    }
    private void OnFindDirectionComplete(string response)
    {
        JsonData jsonData = JsonMapper.ToObject(response);
        OnlineMapsSkobblerDirectionsResult result = OnlineMapsSkobblerDirectionsResult.FromJson(jsonData["route"]);
        if (result != null)
        {
            directionsObj.InitRoute(result.Advisors);
            // Get all the points of the route.
            List<Vector2> points = result.GetPoints();
            // Create a line, on the basis of points of the route.
            OnlineMapsDrawingLine route = new OnlineMapsDrawingLine(points, Color.yellow);
            // Draw the line route on the map.
            OnlineMaps.instance.AddDrawingElement(route);
        }
        else
        {
            Debug.Log("Find direction failed");
        }
        PopupManager.Instance.HideLoading();
    }

    private void OnFindDirectionGoogleComplete(string response)
    {
        // Get the route steps.
        List<OnlineMapsDirectionStep> steps = OnlineMapsDirectionStep.TryParse(response);
        if (steps != null)
        {
            directionsObj.InitRoute(steps);
            // Get all the points of the route.
            List<Vector2> points = OnlineMapsDirectionStep.GetPoints(steps);
            // Create a line, on the basis of points of the route.
            OnlineMapsDrawingLine route = new OnlineMapsDrawingLine(points, Color.green);
            // Draw the line route on the map.
            OnlineMaps.instance.AddDrawingElement(route);
        }
        else
        {
            Debug.Log("Find direction failed");
        }
        PopupManager.Instance.HideLoading();
    }

    public void OnZoomInButtonClick()
    {
        _api.zoom++;
    }

    public void OnZoomOutButtonClick()
    {
        _api.zoom--;
    }

    public void OnMyLocaltionButtonClick()
    {
        Debug.Log("co zo");
        Vector2 position = OnlineMapsLocationService.instance.position;
        if (position != Vector2.zero)
        {
            if (_searchMarker == null)
            {
                _searchMarker = _api.AddMarker(position);
            }
            else
            {
                _searchMarker.position = position;
                _searchMarker.label = "Current Location";
            }

            if (_api.zoom < 13)
                _api.zoom = 13;

            _api.position = position;
            _api.Redraw();
        }
    }
}