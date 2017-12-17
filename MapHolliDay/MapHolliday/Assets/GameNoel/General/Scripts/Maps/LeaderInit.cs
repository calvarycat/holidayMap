using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderInit : MonoBehaviour {

    public Text txtRank;
    public Text txtUserName;
    public Text txttimeplay;
    public RawImage imgIcon;
   
    public void LoadLeaderItem(int rank, Result rs)
    {
        txtRank.text = rank.ToString();
        txtUserName.text = rs.name;
        txttimeplay.text = Utils.SecondToString((int)rs.timeplay);// rs.timeplay.ToString();
        if(!string.IsNullOrEmpty(rs.icon) )
        {
           
            imgIcon.texture= Utils.StringToTexture(rs.icon);
        }
       
    }
}
