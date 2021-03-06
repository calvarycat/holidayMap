﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PnInCenter : MonoBehaviour
{

    public GameObject root;
    public string[] listTestTutorial;
    public Text txtIndex;
    public Text txtTut;
    public GameObject camDevice;
    public CenterControl pnTrungTam;
    public CenterWithoutService pncenterWithout;
    int centerID;
    int indexText = 0;
    PositionWELCenter datacenter;
    public Text txtDiaChi;
    public Text txtName;
    public void InitCenter(int _centerid)
    {
        centerID = _centerid;
        OnShow(true);
        // show infor center
        LoadCenter(_centerid);
        UpdateTutorial(0);
    }
    public void LoadCenter(int centerID)
    {
        datacenter = Datacenter.instance.listCenter[centerID];
        // load dia chir
        txtDiaChi.text = datacenter.Address;
        txtName.text = datacenter.name;
    }
    public void OnShow(bool _isShow)
    {
        root.gameObject.SetActive(_isShow);
    }

    public void OnButtonPreClick()
    {
        if (indexText > 0)
        {
            indexText--;
      
           // indexText = listTestTutorial.Length - 1;
        }      
        UpdateTutorial(indexText);
    }
    public void OnButtonNextClick()
    {
       
        if (indexText < listTestTutorial.Length-1)
        {
            indexText++;        
        }
    
        UpdateTutorial(indexText);
    }
    public GameObject btnBatDau;
    public GameObject imgStep1;
    public void UpdateTutorial(int _idx)
    {
        txtIndex.text = (indexText + 1).ToString() + "/" + listTestTutorial.Length;
        txtTut.text = listTestTutorial[_idx];   
        if (_idx + 1 == listTestTutorial.Length)
        {
            btnBatDau.gameObject.SetActive(true);
        }
        else
        {
            btnBatDau.gameObject.SetActive(false);
        }
        if(_idx==0)
        {
            imgStep1.gameObject.SetActive(true);
        }else
        {
            imgStep1.gameObject.SetActive(false);
        }
    }
    public void OnBatDauClick()
    {
        Debug.Log("On button bắt đầu click");
        OnShow(false);
        camDevice.SetActive(true);
    }
    public void OnButtonQuayLaiSauClick()
    {
        OnShow(false);// tắt đi mở lại cái chọn địa điểm
        if (Input.location.isEnabledByUser)
        {
            pnTrungTam.OnShow(true);
        }
        else
        {
            pncenterWithout.OnShow(true);
        }

    }
    #region  im right here
    public GameObject pnImRightHere;
    public GameObject[] HideImRightHere;
    public void OnButtonImRightHereClick()
    {
      //  Debug.Log("On button im right here Click" + CheckGotoCenter(centerID));
#if UNITY_PHONE
        if (!Input.location.isEnabledByUser)
        {
            PanelPopUp.intance.OnInitInforPopUp("Opps!!", "Vui Lòng bật local service!! ");

        }
        else
        {
            if (CheckGotoCenter(centerID))
            {
                pnImRightHere.gameObject.SetActive(true);
                HideImRightHere[0].gameObject.SetActive(false);
                HideImRightHere[1].gameObject.SetActive(false);
                HideImRightHere[2].gameObject.SetActive(false);
            }
            else
            {
                PanelPopUp.intance.OnInitInforPopUp("Opps!!", "Bạn chưa đến đúng vị trí. Vui lòng thử lại!! ", "Đồng ý");
            }
        }
#else
        ForEditor();
#endif
    }
    void ForEditor()
    {
        if (CheckGotoCenter(centerID))
        {
          //  PanelPopUp.intance.OnInitInforPopUp("", st);
            pnImRightHere.gameObject.SetActive(true);
            HideImRightHere[0].gameObject.SetActive(false);
            HideImRightHere[1].gameObject.SetActive(false);
            HideImRightHere[2].gameObject.SetActive(false);
        }
        else
        {
            // PanelPopUp.intance.OnInitInforPopUp("", st);
            PanelPopUp.intance.OnInitInforPopUp("Opps!!", "Bạn chưa đến đúng vị trí. Vui lòng thử lại!! ", "Đồng ý");
        }
    }
    string st;
    public bool CheckGotoCenter(int CenterID)
    {
        if (StaticClass.isCheating)
        {
            return true;
        }
        // return true;
#if UNITY_EDITOR
        return true;

#else
    
         Input.location.Start();
        
        Vector2 currentPos = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);


        Location l1 = new Location();
        l1.Latitude = Input.location.lastData.latitude;
        l1.Longitude= Input.location.lastData.longitude;
      

        PositionWELCenter pos = Datacenter.instance.listCenter[CenterID];
        Location l2 = new Location();

        //l2.Longitude = 106.683395;// pos.pos.x;
        //l2.Latitude = 10.776305 ;// pos.pos.y;
        l2.Longitude =  pos.pos.x;
        l2.Latitude =  pos.pos.y;
                                
        double rs = Utils.CalculateDistance(l1, l2);
        st= CenterID.ToString()+ rs.ToString() + "/[" + l1.Latitude.ToString() + "," + l1.Longitude.ToString() + "]/[" + l2.Latitude.ToString() + "," + l2.Longitude + "]";
    
       if(rs<0.5)
        {
            return true;
        }
        return false;
#endif
    }
    #endregion
}
