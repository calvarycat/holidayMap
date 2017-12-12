using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GSLogin : GSTemplate
{
    public static GSLogin Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    protected override void Init()
    {
        base.Init();
        SceneManager.LoadScene(SceneName.Home);
    }
}