using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocaleLableMain : LocaleLable
{
    public GameObject xButton;
    public override void Init(OnlineMapsOSMNominatimResult _data)
    {
        base.Init(_data);
        xButton.SetActive(true);
        GSHome.Instance.LockPositionClick(data);
        GSHome.Instance.localeCurrent.Init(data);
    }
    public void OnClickX()
    {
        txt.text = "Try \"Work\"";
        xButton.SetActive(false);
        GSHome.Instance.localeCurrent.Show(false);
    }
}
