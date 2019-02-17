using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownTimer : MonoBehaviour
{
    public int ClockTime { get; private set; }

    private bool running = false;
    private Coroutine lastCo;

    public void Launch(int i, string tickName, string completeName, IEventParam onTick, IEventParam onComplete)
    {
        ClockTime = i;
        running = true;
        lastCo = StartCoroutine(Tick(tickName, completeName, onTick, onComplete));
    }

    public void Stop()
    {
        if (running)
            running = false;
    }

    IEnumerator Tick(string tickName, string completeName, IEventParam onTick, IEventParam onComplete)
    {
        while (ClockTime > 0 && running)
        {
            EventManager.Instance.Raise(tickName, onTick);
            yield return new WaitForSeconds(1);
            ClockTime--;
        }

        if (running)
        {
            running = false;
            EventManager.Instance.Raise(completeName, onComplete);
        }
    }
}
