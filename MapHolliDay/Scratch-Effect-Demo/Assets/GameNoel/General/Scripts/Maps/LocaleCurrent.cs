using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocaleCurrent : MonoBehaviour
{
    OnlineMapsOSMNominatimResult data;
    public Text display_name;
    public Text position;
    public void Init(OnlineMapsOSMNominatimResult _data)
    {
        data = _data;
        display_name.text = data.display_name;
        position.text = data.location.ToString();
        Show(true);
    }
    public void OnDirectionClick()
    {
        GSHome.Instance.OnUnitClickDirections(data);
        Show(false);
    }
    public void Show(bool value)
    {
        gameObject.SetActive(value);
    }
}
