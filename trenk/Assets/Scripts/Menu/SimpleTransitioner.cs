using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTransitioner : MonoBehaviour, ITransitioner
{
    public void TransitionIn(GameObject o)
    {
        o.SetActive(true);
    }

    public void TransitionOut(GameObject o)
    {
        o.SetActive(false);
    }
}
