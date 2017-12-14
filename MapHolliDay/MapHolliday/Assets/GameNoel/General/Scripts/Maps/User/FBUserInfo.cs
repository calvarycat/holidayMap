using UnityEngine;
using System.Collections;

// json class for FB json result
public class FBUserInfo
{
    public string id;
    public string name;
}

// json class for FB json result
public class FBUserScore
{
    public int score;
    public FBUserInfo user;
}

// customize class for highscore display
public class FBUserHighScore
{
    public string Id;
    public string Name;
    public bool IsPlayedApp;
    public int Score;
    public Texture2D Picture;
}