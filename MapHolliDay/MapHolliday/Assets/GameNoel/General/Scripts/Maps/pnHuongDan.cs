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

#if UNITY_EDITOR
        GoOnLocalServices();
        return;
#endif
      
        if (!Input.location.isEnabledByUser)
        {
            PanelPopUp.intance.OnInitYesNo("To continues using this app, please enable local services", OnOkClick, OnCanCleClick);

        }
        else
        {
            GoOnLocalServices();
        }

        isCheckClick = true;
    }
    public void OnCanCleClick()
    {
        isCheckClick = false;
        CancelInvoke("OnOkClick");
        GoWithoutService();
    

    }
    bool isCheckClick = false;
    public void OnOkClick()
    {
        if (isCheckClick)
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
       
        Onshow(false);
        pnTrungTam.OnShow(true);
    }
    void GoWithoutService()
    {
        Onshow(false);
        pnCenterWithoutService.OnShow(true);
    }
    public void ObBackToMain()
    {
        Debug.Log("On Back To Main");
    }
}
