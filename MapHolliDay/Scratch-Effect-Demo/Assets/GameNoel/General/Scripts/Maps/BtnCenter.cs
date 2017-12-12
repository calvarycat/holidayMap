using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnCenter : MonoBehaviour {
    public int id;
    public int distant;
    public Button btnCenter;
    public Text txtName;

    public void Init(int _id,string name,int _distant)
    {
        txtName.text = name + " :  " + _distant.ToString() +"m";
        id = _id;
        distant = _distant;
    }
}
