using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CountdownTimer))]
public class VsStarter : MonoBehaviour
{
    private CountdownTimer timer;
    private Action<IEventParam> startListener, countdownListener;

    private void Awake()
    {
        timer = GetComponent<CountdownTimer>();

        startListener = (e) =>
        {
            timer.Launch(3, "pre-tick", "countdown", new IntParam(timer.ClockTime), null);
        };

        countdownListener = (e) =>
        {
            timer.Launch(3, "countdown-tick", "start", new IntParam(timer.ClockTime), null);
        };
    }

    private void OnEnable()
    {
        EventManager m = EventManager.Instance;

        if (m != null)
        {
            EventManager.Instance.Subscribe("in-scene", startListener);
            EventManager.Instance.Subscribe("countdown", countdownListener);
        }
    }

    private void OnDisable()
    {
        EventManager m = EventManager.Instance;

        if (m != null)
        {
            EventManager.Instance.Unsubscribe("in-scene", startListener);
            EventManager.Instance.Unsubscribe("countdown", countdownListener);
        }
    }
}
