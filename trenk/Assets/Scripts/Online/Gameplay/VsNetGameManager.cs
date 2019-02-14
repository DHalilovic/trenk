using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VsNetGameManager : NetGameManager
{
    private CountdownTimer timer;

    public override void Awake()
    {
        round = GetComponent<NetRoundManager>();
        timer = GetComponent<CountdownTimer>();

        // Prepare listeners
        onConnectListener = (e) => { round.Ongoing = true; };      
    }
}
