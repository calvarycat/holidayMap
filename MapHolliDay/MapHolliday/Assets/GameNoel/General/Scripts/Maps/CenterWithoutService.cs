using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class CenterWithoutService : MonoBehaviour {

    public pnHuongDan pnHuongDan;
    private OnlineMaps _api;
    public GameObject Root;
    public PnInCenter pnInCenter;   

   
    public int ChooseCenter = -1;
    void Start()
    {
      
        _api = OnlineMaps.instance;
       
    }    
    public void OnShow(bool _isShow)
    {
        Root.gameObject.SetActive(_isShow);
        _api = OnlineMaps.instance;     
    }
    public void GotoCenter(int centerID)
    {
        if (centerID != -1)
        {
            Root.gameObject.SetActive(false);
            pnInCenter.InitCenter(centerID);
            Debug.Log("Ban da chon center " + centerID);
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
