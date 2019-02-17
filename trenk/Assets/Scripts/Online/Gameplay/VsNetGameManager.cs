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
        onConnectListener = new Action<IEventParam>((e) => { round.Ongoing = true; });      
    }
}
