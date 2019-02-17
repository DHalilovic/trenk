using System.Collections;
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

        //// Let origin finish before target begins
        //yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(t);   

        target.In();

        yield return null;
    }
}
