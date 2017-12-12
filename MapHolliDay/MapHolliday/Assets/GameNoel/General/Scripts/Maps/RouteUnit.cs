using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RouteUnit : MonoBehaviour
{
    public Text txtName;
    public Text txtDetail;
    public void Init(string _name, string _info)
    {
        txtName.text = _name;
        txtDetail.text = _info;
    }
}
