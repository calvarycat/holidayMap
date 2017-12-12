using UnityEngine;
using System;
using System.Collections;

public class GameStatesManager : MonoBehaviour
{
    public static GameStatesManager Instance { get; private set; }

    public GameObject InputProcessor { get; set; }
    public static Action OnBackKey { get; set; }
    public static Action OnCheatState { get; set; }
    public StateMachine MyStateMachine;
    public IState DefaultState;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        MyStateMachine.PushState(DefaultState);
    }

    private void Update()
    {
#if !LIVE
#if UNITY_EDITOR
        if (OnCheatState != null && Input.GetKeyDown(KeyCode.F2))
        {
            OnCheatState();
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            OnCheat();
        }
#else
        if (OnCheatState != null && Input.touches.Length == 3)
        {
            OnCheatState();
        }
        if (Input.touches.Length == 4)
        {
            OnCheat();
        }
#endif
#endif
    }

    public void OnCheat()
    {
        PopupManager.Instance.InitMessage("OnCheat");
    }
}