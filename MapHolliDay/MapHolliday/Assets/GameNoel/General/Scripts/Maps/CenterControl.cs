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
        for (int i = 0; i < Datacenter.instance.listCenter.Count; i++)
        {
            PositionWELCenter pos = Datacenter.instance.listCenter[i];
            Location l2 = new Location();
            l2.Longitude = pos.pos.x;
            l2.Latitude = pos.pos.y;
            double rs = Utils.CalculateDistance(l1, l2);
            txtListKhoangcach[i].text = Datacenter.instance.listCenter[i].name +" "+ Math.Round( rs,2).ToString() +"km";

        }
    }
    void ViewCurrentPosition()
    {
        //curentPosition = OnlineMapsLocationService.instance.position;
        //if (curentPosition != Vector2.zero)
        //{
        //    StartCoroutine(Init());
        //}
        //else
        //{
        //    if (Input.location.isEnabledByUser)
        //    {
        //        Input.location.Start();
        //        float lati = Input.location.lastData.latitude;
        //        float longti = Input.location.lastData.longitude;
        //        curentPosition = new Vector2(lati, longti);
        //    }
        //    Invoke("ViewCurrentPosition", .5f);
        //}

    }

    public IEnumerator Init()
    {
        //  Vector2 position = OnlineMapsLocationService.instance.position;
        if (!isInitSuccess)
        {
            Utils.RemoveAllChildren(parentPnbuttonCenter);
            for (int i = 0; i < 6; i++)
            {
            //    UpdateDistant(i, curentPosition);
            }

        }

        yield return null;
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
        PanelPopUp.intance.OnInitInforPopUp("No service");
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
        else
        {
            PanelPopUp.intance.OnInitYesNo("No service");
        }
    }

    //void CheckKC(string response)
    //{
    //    float kc;
    //    List<OnlineMapsDirectionStep> steps = OnlineMapsDirectionStep.TryParse(response);
    //    if (steps != null)
    //    {
    //        kc = CaculateeKHoangCach(steps);      
    //    }

    //}
    //void UpdateDistant(int i, Vector2 pos)
    //{
    //    OnlineMapsGoogleDirections query
    //        = OnlineMapsGoogleDirections.Find(OnlineMapsLocationService.instance.position, Datacenter.instance.listCenter[i].pos);
    //    query.OnComplete += OnDistanFindComplete;
    //}
    //int kc;
    //int initCounter = 0;
    //void OnDistanFindComplete(string response)
    //{
    //    //if (!isShow)
    //    //    return;
    //    Debug.Log("h no mơi zo");
    //    List<OnlineMapsDirectionStep> steps = OnlineMapsDirectionStep.TryParse(response);
    //    if (steps != null)
    //    {
    //        kc = CaculateeKHoangCach(steps);

    //    }
    //    int c = initCounter;
    //    GameObject obj = Instantiate(pnButtonCenter, parentPnbuttonCenter);
    //    BtnCenter bt = obj.GetComponent<BtnCenter>();
    //    bt.Init(initCounter, Datacenter.instance.listCenter[initCounter].name, kc);
    //    obj.transform.localScale = new Vector3(1, 1, 1);
    //    obj.transform.localRotation = Quaternion.identity;
    //    bt.GetComponentInChildren<Button>().onClick.AddListener(delegate { OncenterClick(c); });


    //    bt.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { GotoCenter(c); });
    //    initCounter++;
    //    Sort();
    //    if(initCounter==6)
    //    {
    //        isInitSuccess = true;
    //    }
    //}
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
    //public void Sort()
    //{
    //    List<BtnCenter> listBtn = new List<BtnCenter>();
    //    foreach (Transform tran in parentPnbuttonCenter.transform)
    //    {
    //        BtnCenter bt = tran.GetComponent<BtnCenter>();

    //        listBtn.Add(bt);
    //    }
    //    SortedList = listBtn.OrderBy(o => o.distant).ToList();
    //    ix = 0;
    //    foreach (BtnCenter tran in SortedList)
    //    {
    //        tran.transform.SetSiblingIndex(ix);
    //        ix++;
    //    }
    //}
    //public int CaculateeKHoangCach(List<OnlineMapsDirectionStep> _routes)
    //{
    //    int rs = 0;
    //    for (int i = 0; i < _routes.Count; i++)
    //    {
    //        rs += _routes[i].distance;
    //    }
    //    return rs;
    //}
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