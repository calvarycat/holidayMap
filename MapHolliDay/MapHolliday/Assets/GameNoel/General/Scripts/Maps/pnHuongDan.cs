using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pnHuongDan : MonoBehaviour
{

    public GameObject root;
    public CenterControl pnTrungTam;
    public CenterWithoutService pnCenterWithoutService;

    public void Onshow(bool _isShow)
    {
        root.SetActive(_isShow);

    }
    public void OnBtnChonDiaDiemClick()
    {
#if UNITY_PHONE
        Input.location.Start();
        if (!Input.location.isEnabledByUser)
        {
             PanelPopUp.intance.OnInitYesNo("To continues using this app, please enable local services", OnOkClick, OnCanCleClick);
           // PanelPopUp.intance.OnInitInforPopUp("To continues using this app, please enable local services");

        }
        else
        {
           GoOnLocalServices();
        }
#else

         GoOnLocalServices();
      //  GoWithoutService();
#endif
        isCheckClick = true;
    }
    public void OnCanCleClick()
    {
        isCheckClick = false;
        CancelInvoke("OnOkClick");
        GoWithoutService();
        Debug.Log("OnCancel Click");
        // load page offline

    }
    bool isCheckClick = false;
    public void OnOkClick()
    {
        if(isCheckClick)
        {
            if (!Input.location.isEnabledByUser)
            {
                PanelPopUp.intance.OnInitInforPopUp("", "Go to setting enable service");
                Invoke("OnOkClick", 5f);
            }
            else
            {
                GoOnLocalServices();
            }

        }

    }
    void GoOnLocalServices()
    {
        Input.location.Start();   
        Onshow(false);
        pnTrungTam.OnShow(true);
    }
    void GoWithoutService()
    {
        Onshow(false);
        pnCenterWithoutService.OnShow(true);
    }
}
