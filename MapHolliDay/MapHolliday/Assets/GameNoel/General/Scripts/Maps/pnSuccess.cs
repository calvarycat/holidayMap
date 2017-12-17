using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pnSuccess : MonoBehaviour {
    public GameObject pnSelfie;
    public GameObject root;
   
    public void OnShow(bool isShow)
    {
        root.gameObject.SetActive(isShow);
       
    }
    public void OnSelfClick()
    {
        pnSelfie.gameObject.SetActive(true);
        OnShow(false);
    }
}
