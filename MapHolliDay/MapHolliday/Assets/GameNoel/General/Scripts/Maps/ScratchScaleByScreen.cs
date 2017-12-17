using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScratchScaleByScreen : MonoBehaviour {

    // Use this for initialization
    public GameObject objScratch;
	void Start () {
        
        float ratio = Screen.width / (float)Screen.height;
        Debug.Log(ratio);
       if(ratio == 0.5635965)
        {
            
        }
        if (ratio == 0.625)
        {
          
        }
        if (ratio >= 0.75)
        {
          
            objScratch.transform.localScale = new Vector3(100, 66, 1);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
