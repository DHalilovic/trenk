using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostGameStarter : VsGameStarter
{
    public GameEvent onRoundPrepare, OnRoundEnd; // Events to signal

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }
}
