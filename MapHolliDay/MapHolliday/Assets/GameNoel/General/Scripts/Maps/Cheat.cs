using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Cheat : MonoBehaviour {

    // Use this for initialization
    public Text txtShow;
	void Start () {
        StaticClass.isCheating = false;

    }
	
	public void OnCheatClick()
    {
        StaticClass.isCheating = !StaticClass.isCheating;
        ShowInfo();
    }
   public void ShowInfo()
    {
        if (Input.location.isEnabledByUser)
        {
            Input.location.Start();
            txtShow.text = Input.location.lastData.latitude + "//" + Input.location.lastData.longitude;
                }
    }
}
