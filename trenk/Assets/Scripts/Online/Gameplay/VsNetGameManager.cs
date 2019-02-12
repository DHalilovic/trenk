using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VsNetGameManager : NetGameManager
{
    public override void Awake()
    {
        round = GetComponent<NetRoundManager>();
        // Prepare listener
        onConnectListener = (e) => { round.Ongoing = true; Debug.Log("Round start"); };
    }
}
