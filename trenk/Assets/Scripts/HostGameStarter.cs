using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostGameStarter : MonoBehaviour
{
    public int scoreCap = 5;
    public GameObject player;

    private const byte EMPTY = 0;
    private const byte P1 = 1;
    private const byte P2 = 2;
    private const byte HAZARD = 3;

    private const byte UP = 0;
    private const byte RIGHT = 1;
    private const byte DOWN = 2;
    private const byte LEFT = 3;

    private short[,] board;
    private int arenaHeight = 30;
    private int[] scores;
    private GameObject[] players;

    public int P1Score;

    void Start()
    {
        board = new short[arenaHeight, arenaHeight];
        // Verify player connections
        // Spawn player gameObjects
        players = new GameObject[2];
        players[0] = Instantiate(player);
        players[1] = Instantiate(player);
        // Start first round
    }

    
}
