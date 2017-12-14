using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelPopUp : MonoBehaviour {

    public static PanelPopUp intance;
    public GameObject root;
    public Text txtShowInfo;
    public Text txtTitle;
    public Text txtOk;
    public YesNoInit yesNo;
    public bool isShow;
    private void Awake()
    {
        intance = this;
    }
    public void OnInitInforPopUp(string title="Opps!!",string info="",string ok="OK")
    {
        root.gameObject.SetActive(true);
        txtTitle.text = title;
        txtShowInfo.text = info;
        txtOk.text = ok;
        isShow = true;
    }
    public void OnHidePopUp()
    {
    //    Debug.Log("hide popUp");
        root.SetActive(false);
        txtShowInfo.text = "";
        isShow = false;

    }
    public void OnInitYesNo (string message,
        Action yesCallback = null,
        Action noCallBack = null,
        string yesButonName = "YES",
        string noButtonName = "NO")
    {
       
        yesNo.Init("Allow WSE Festive Challenge to access to your service location", yesCallback, noCallBack);
        yesNo.gameObject.SetActive(true);
    }
}
