using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownTimer : MonoBehaviour
{
    public string tickEventName, completeEventName;
    private int clockTime;

    public void Launch(int i, IEventParam onTick, IEventParam onComplete)
    {
        clockTime = i;
        StartCoroutine(Tick(onTick, onComplete));
    }

    public void Stop()
    {
        StopAllCoroutines();
    }

    IEnumerator Tick(IEventParam onTick, IEventParam onComplete)
    {
        while (clockTime > 0)
        {
            EventManager.Instance.Raise(tickEventName, onTick);
            yield return new WaitForSeconds(1);
            clockTime--;
        }

        EventManager.Instance.Raise(completeEventName, onComplete);
    }
}
