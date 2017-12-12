using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pnHuongDan : MonoBehaviour {

    public GameObject root;
    public CenterControl pnTrungTam;
    public void Onshow(bool _isShow)
    {
        root.SetActive(_isShow);

    }
    public void OnBtnChonDiaDiemClick()
    {
#if UNITY_IPHONE
        Input.location.Start();
        if (!Input.location.isEnabledByUser)
        {
            // PanelPopUp.intance.OnInitYesNo("Mo LocalService", OnOkClick, OnCanCleClick);
            PanelPopUp.intance.OnInitInforPopUp("To continues using this app, please enable local services");

        }
        else
        {
            OnOkClick();
        }
#else
        OnOkClick();
#endif

    }
    public void OnCanCleClick()
    {
        Debug.Log("OnCancel Click");

    }
    public void OnOkClick()
    {
        Input.location.Start();
        Debug.Log("On ok Click");
        Onshow(false);
        pnTrungTam.OnShow(true);
    }
}
