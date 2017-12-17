using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheat : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StaticClass.isCheating = false;

    }
	
	public void OnCheatClick()
    {
        StaticClass.isCheating = !StaticClass.isCheating;
    }
}
