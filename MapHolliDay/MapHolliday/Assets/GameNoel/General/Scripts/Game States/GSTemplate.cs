using System.Collections;
using UnityEngine;

////////////////////////////////////////////////////////
//Author:
//TODO: a game state sample
////////////////////////////////////////////////////////

public class GSTemplate : IState
{
    public GameObject GuiMain;

    protected bool IsFirstTime;

    protected override void Awake()
    {
        base.Awake();
        GuiMain.SetActive(false);
        IsFirstTime = true;
    }

    /// <summary>
    ///     One time when start
    /// </summary>
    protected virtual void Init()
    {
    }

    protected virtual void OnBackKey()
    {
    }

    protected virtual void OnCheatState()
    {
    }

    public override void OnSuspend()
    {
        base.OnSuspend();
        GameStatesManager.OnBackKey = null;
        GameStatesManager.OnCheatState = null;
        GuiMain.SetActive(false);
    }

    public override void OnResume()
    {
        base.OnResume();
        GameStatesManager.Instance.InputProcessor = GuiMain;
        GameStatesManager.OnBackKey = OnBackKey;
        GameStatesManager.OnCheatState = OnCheatState;
        GuiMain.SetActive(true);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        if (IsFirstTime)
        {
            IsFirstTime = false;
            Init();
        }
        OnResume();
    }

    public override void OnExit()
    {
        base.OnExit();
        OnSuspend();
    }
}