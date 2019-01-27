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
        events = new Dictionary<string, Action<IEventParam>>();
    }

    public void Subscribe(string eventName, Action<IEventParam> listener)
    {
        if (events.TryGetValue(eventName, out Action<IEventParam> thisEvent))
        {
            // Add more events to existing one
            thisEvent += listener;

            // Update dict
            events[eventName] = thisEvent;
        }
        else
        {
            // Add event to dict for first time
            thisEvent += listener;
            events.Add(eventName, thisEvent);
        }
    }

    public void Unsubscribe(string eventName, Action<IEventParam> listener)
    {
        if (events.TryGetValue(eventName, out Action<IEventParam> thisEvent))
        {
            //Remove event from existing one
            thisEvent -= listener;

            //Update dict
            events[eventName] = thisEvent;
        }
    }

    public void Raise(string eventName, IEventParam eventParam)
    {
        if (events.TryGetValue(eventName, out Action<IEventParam> thisEvent))
            thisEvent.Invoke(eventParam);
    }
}
