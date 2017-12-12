using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerInit : MonoBehaviour {

    // Use this for initialization
    public Text oder;
    public Text answer;

    public void Init (string _oder,string _answer) {
        oder.text = _oder;
        answer.text = _answer;
    }
	

}
