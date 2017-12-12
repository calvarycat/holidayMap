using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class IState : MonoBehaviour
{
    protected virtual void Awake()
    {
    }

    public virtual void OnSuspend()
    {
    }

    public virtual void OnResume()
    {
    }

    public virtual void OnEnter()
    {
    }

    public virtual void OnExit()
    {
    }
}

public class StateMachine : MonoBehaviour
{
    private readonly Stack<IState> _stateStack = new Stack<IState>();

    public void PushState(IState state)
    {
        IState prevState = null;
        if (_stateStack.Count > 0)
        {
            prevState = _stateStack.Peek();
            prevState.OnSuspend();
        }
        _stateStack.Push(state);
        state.OnEnter();
    }

    public void SwitchState(IState state, bool sound = true)
    {
        IState prevState = null;
        while (_stateStack.Count > 0)
        {
            prevState = _stateStack.Pop();
            prevState.OnExit();
        }
        _stateStack.Push(state);
        state.OnEnter();
    }

    public void PopState()
    {
        IState prevState = null;
        if (_stateStack.Count > 0)
        {
            prevState = _stateStack.Pop();
            prevState.OnExit();
        }
        IState thisState = _stateStack.Peek();
        thisState.OnResume();
    }

    public IState currentState
    {
        get
        {
            if (_stateStack.Count > 0) return _stateStack.Peek();
            return null;
        }
    }
}