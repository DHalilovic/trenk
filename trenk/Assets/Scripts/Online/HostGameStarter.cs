using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostGameStarter : VsGameStarter
{
    public GameEvent onRoundPrepare, OnRoundEnd; // Events to signal

    public override void Start()
    {
        // Configure board, scene
        base.Start();
        // Verify connection with opponent via ready message
        // Count down, sending client each number...
        // Start, sending host start message
        // P2P lockstep, exchange button presses per gameStep
        // Await ready, and repeat...
    }
}
