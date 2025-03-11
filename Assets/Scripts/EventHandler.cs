using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    public static EventHandler Instance; // Singleton for global access

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Define the delegate and event
    public delegate void DeselectTurretEventHandler();
    public event DeselectTurretEventHandler OnDeselectTurretEvent;

    public delegate void DeselectEnemyEventHandler();

    public delegate void StartGameEventHandler();

    public event StartGameEventHandler StartGameEvent;

    public event DeselectEnemyEventHandler OnDeselectEnemyEvent;

    public delegate void CloseRoundPopupEventHandler();

    public event CloseRoundPopupEventHandler OnCloseRoundPopupEvent;

    public delegate void TowersLoadedEventHandler();

    public event TowersLoadedEventHandler OnTowersLoadedEvent;

    public delegate void EnemiesLoadedEventHandler();

    public event EnemiesLoadedEventHandler OnEnemiesLoadedEvent;

    public delegate void RightClickEventHandler();

    // Declare an event that passes a bool
    public event Action<bool> OnTimeToggledEvent;

    public event Action<int> OnRightClickEvent;

    public event Action<int> OnInputToggledEvent;

    // Trigger the event
    public void InvokeTimeToggledEvent(bool value)
    {
        OnTimeToggledEvent?.Invoke(value);
    }

    public void InvokeInputToggledEvent(int value)
    {
        OnInputToggledEvent?.Invoke(value);
    }

    // Public method to invoke the event
    public void InvokeDeselectTurretEvent()
    {
        OnDeselectTurretEvent?.Invoke();
    }

    public void InvokeDeselectEnemyEvent()
    {
        OnDeselectEnemyEvent?.Invoke();
    }
    public void InvokeCloseRoundPopupEvent()
    {
        OnCloseRoundPopupEvent?.Invoke();
    }

    public void InvokeStartGameEvent()
    {
        StartGameEvent?.Invoke();
    }

    public void InvokeRightClickEvent(int value)
    {
        OnRightClickEvent?.Invoke(value);
    }

    public void InvokeTowersLoadedEvent()
    {
        OnTowersLoadedEvent?.Invoke();
    }

    public void InvokeEnemiesLoadedEvent()
    {
        OnEnemiesLoadedEvent?.Invoke();
    }
}
