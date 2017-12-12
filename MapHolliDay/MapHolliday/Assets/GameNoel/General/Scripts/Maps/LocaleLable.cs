using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocaleLable : MonoBehaviour {

    public Text txt;
    public bool isShowYourLocale;
    public OnlineMapsOSMNominatimResult data;
    public virtual void Init(OnlineMapsOSMNominatimResult _data)
    {
        data = _data;
        txt.text = data.display_name;
	}
	public void OnClickLable()
    {
        GSHome.Instance.localeSearch.Init(this);
	}
}
