using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CenterControl : MonoBehaviour
{

    public pnHuongDan pnHuongDan;
    private OnlineMaps _api;
    public GameObject Root;
    public PnInCenter pnInCenter;
    private OnlineMapsMarker _searchMarker;
    public GameObject pnButtonCenter;
    public Transform parentPnbuttonCenter;
    Vector2 curentPosition;
    public int ChooseCenter = -1;
    bool isInitSuccess;
    public Text[] txtListKhoangcach;
    void Start()
    {
        _api = OnlineMaps.instance;
    }

    bool isShow;
    public void OnShow(bool _isShow)
    {
        isShow = _isShow;
        Root.gameObject.SetActive(_isShow);

     
        UpdateDistantCenter();
    }

    public void UpdateDistantCenter()
    {
        if (!Input.location.isEnabledByUser)
        {
            return;
        }
        Input.location.Start();
        Vector2 currentPos = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
        Location l1 = new Location();
        l1.Latitude = Input.location.lastData.latitude;
        l1.Longitude = Input.location.lastData.longitude;
        List<double> listKC = new List<double>();
        for (int i = 0; i < Datacenter.instance.listCenter.Count; i++)
        {
            PositionWELCenter pos = Datacenter.instance.listCenter[i];
            Location l2 = new Location();
            l2.Longitude = pos.pos.x;
            l2.Latitude = pos.pos.y;
            double rs = Utils.CalculateDistance(l1, l2);
            txtListKhoangcach[i].text = Datacenter.instance.listCenter[i].name +" "+ Math.Round( rs,2).ToString() +"km";
            listKC.Add(rs);
        }
        Sort(listKC);
    }
  
  
    public void OnClickChooseCenter()
    {
        if (ChooseCenter != -1)
        {
            Root.gameObject.SetActive(false);
            pnInCenter.InitCenter(ChooseCenter);
            Debug.Log("Ban da chon center " + ChooseCenter);
        }
        else
        {
            PanelPopUp.intance.OnInitYesNo("Vui Lòng chọn 1 trung tâm");
        }

    }
    
    public void OncenterClick(int id)
    {
       // PanelPopUp.intance.OnInitInforPopUp("No service");
        Vector2 position = Datacenter.instance.listCenter[id].pos;
        if (position != Vector2.zero)
        {
            if (_searchMarker == null)
            {
                _searchMarker = _api.AddMarker(position);
            }
            else
            {
                _searchMarker.position = position;
                _searchMarker.label = "center Localtion";
            }

            _api.zoom = 15;

            _api.position = position;
            _api.Redraw();
            ChooseCenter = id;
        }
        //else
        //{
        //    PanelPopUp.intance.OnInitInforPopUp("Opps","No services!");
        //}
    }

   
    public void GotoCenter(int centerID)
    {
      
        if (centerID != -1)
        {
            Root.gameObject.SetActive(false);
            pnInCenter.InitCenter(centerID);

        }
       

    }
    public List<BtnCenter> SortedList;
    int ix;
    public void Sort(List<double> listKC)
    {

        List<BtnCenter> listBtn = new List<BtnCenter>();
        int j = 0;
        foreach (Transform tran in parentPnbuttonCenter.transform)
        {
            BtnCenter bt = tran.GetComponent<BtnCenter>();
            bt.distant = listKC[j];
            listBtn.Add(bt);
            j++;
           
        }
        SortedList = listBtn.OrderBy(o => o.distant).ToList();
        ix = 0;
        foreach (BtnCenter tran in SortedList)
        {
            tran.transform.SetSiblingIndex(ix);
            ix++;
        }
    }

    public void OnButtonBackClick()
    {
        Debug.Log("On back click");
        OnShow(false);
        pnHuongDan.Onshow(true);

    }
    public void OnButtonHomeClick()
    {
        Debug.Log("On home click");
    }
}
[System.Serializable]
public class PositionWELCenter
{
    public string name = "Default";
    public Vector2 pos = Vector2.zero;
    public string Address = "";
}