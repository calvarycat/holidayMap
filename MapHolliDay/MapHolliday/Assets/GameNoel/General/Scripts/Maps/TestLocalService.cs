using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TestLocalService : MonoBehaviour
{
    //void Start()
    //{

    //}
    IEnumerator Start()
    {
       
      
        Input.location.Start();
      
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
            PanelPopUp.intance.OnInitInforPopUp("Time out"+ maxWait);
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            PanelPopUp.intance.OnInitInforPopUp("Time out");
           // yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            PanelPopUp.intance.OnInitInforPopUp("Unable to determine device location");
           // yield break;
        }
        else
        {
            // Access granted and location value could be retrieved
            PanelPopUp.intance.OnInitInforPopUp("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }

        // Stop service if there is no need to query location updates continuously
      //  Input.location.Stop();
    }
}

