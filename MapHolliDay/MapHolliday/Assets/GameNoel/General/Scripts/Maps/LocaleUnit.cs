using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocaleUnit : MonoBehaviour
{
    OnlineMapsOSMNominatimResult data;
    public Text display_name;
    public Text position;
    public void Init(OnlineMapsOSMNominatimResult _data)
    {
        data = _data;
        display_name.text = data.display_name;
        position.text = data.location.ToString();
    }
    public void OnUnitClick()
    {
        GSHome.Instance.localeSearch.OnUnitClick(data);
    }
}
