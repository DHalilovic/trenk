using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITransitioner
{
    // Appearing transition
    void TransitionIn(GameObject o);

    // Dissappearing transition
    void TransitionOut(GameObject o);
}
