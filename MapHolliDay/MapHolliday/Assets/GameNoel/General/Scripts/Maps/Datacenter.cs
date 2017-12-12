using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Datacenter : MonoBehaviour {

    public static Datacenter instance;
    public List<PositionWELCenter> listCenter;
    private void Awake()
    {
        if (!instance)
            instance = this;
        
    }
}
