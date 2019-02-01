using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownTimer : MonoBehaviour
{
    public int ClockTime { get; private set; }

    public void Launch(int i, string tickName, string completeName, IEventParam onTick, IEventParam onComplete)
    {
        ClockTime = i;
        StartCoroutine(Tick(tickName, completeName, onTick, onComplete));
    }

    public void Stop()
    {
        StopAllCoroutines();
    }

    IEnumerator Tick(string tickName, string completeName, IEventParam onTick, IEventParam onComplete)
    {
        while (ClockTime > 0)
        {
            EventManager.Instance.Raise(tickName, onTick);
            yield return new WaitForSeconds(1);
            ClockTime--;
        }

        EventManager.Instance.Raise(completeName, onComplete);
    }
}
