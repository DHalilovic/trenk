﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transitioner : MonoBehaviour
{
    public void GoOneWay(Transitionable origin, Transitionable target)
    {
        StartCoroutine(GoOneWayCo(origin, target));
    }

    IEnumerator GoOneWayCo(Transitionable origin, Transitionable target)
    {
        float t = origin.Out();
        yield return new WaitForSeconds(t);

        origin.gameObject.SetActive(false);
        target.gameObject.SetActive(true);

        target.In();
    }
}
