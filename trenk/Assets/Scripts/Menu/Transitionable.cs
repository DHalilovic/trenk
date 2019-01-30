using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Transitionable : MonoBehaviour
{
    public abstract float In(); // Start in-transition

    public abstract float Out(); // Start out-transition
}
