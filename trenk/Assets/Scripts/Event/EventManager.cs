using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    private Dictionary<string, Action<IEventParam>> events;

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    protected void Init()
    {
        if (events == null)
            events = new Dictionary<string, Action<IEventParam>>();
    }

    public static void Subscribe(string eventName, Action<IEventParam> listener)
    {
        if (Instance.events.TryGetValue(eventName, out Action<IEventParam> thisEvent))
        {
            // Add more events to existing one
            thisEvent += listener;

            // Update dict
            Instance.events[eventName] = thisEvent;
        }
        else
        {
            // Add event to dict for first time
            thisEvent += listener;
            Instance.events.Add(eventName, thisEvent);
        }
    }

    public static void Unsubscribe(string eventName, Action<IEventParam> listener)
    {
        if (Instance != null
            && Instance.events.TryGetValue(eventName, out Action<IEventParam> thisEvent))
        {
            //Remove event from existing one
            thisEvent -= listener;

            //Update dict
            Instance.events[eventName] = thisEvent;
        }
    }

    public static void Raise(string eventName, IEventParam eventParam)
    {
        if (Instance.events.TryGetValue(eventName, out Action<IEventParam> thisEvent))
            thisEvent.Invoke(eventParam);
    }
}
