using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostGameStarter : MonoBehaviour
{
    public int scoreCap = 5; // Score required to win match
    public GameObject player; // Template GameObject for players

    //  Grid space markers
    private const byte EMPTY = 0;
    private const byte P1 = 1;
    private const byte P2 = 2;
    private const byte HAZARD = 3;

    // Indicate which direction each player is travelling
    private const byte UP = 0;
    private const byte RIGHT = 1;
    private const byte DOWN = 2;
    private const byte LEFT = 3;

    private byte[,] board; // Stores positional data in arena

    private int arenaHeight = 30;
    private int homeScore, awayScore;
    private GameObject homePlayer; // Player controlling this device
    private GameObject awayPlayer; // Opponent relative to this device

    void Start()
    {
        // Initialize underlying arena
        board = new byte[arenaHeight, arenaHeight];
        // Verify player connections
        // Spawn player gameObjects
        homePlayer = Instantiate(player);
        awayPlayer = Instantiate(player);
        // Start first round
    }

    private void FixedUpdate()
    {
        
    }
}
