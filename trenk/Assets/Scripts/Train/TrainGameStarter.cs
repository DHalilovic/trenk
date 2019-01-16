using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainGameStarter : MonoBehaviour
{
    public GameObject playerPrefab; // Template GameObject for players
    public GameObject fencePrefab;
    public GameEvent onRoundPrepare;
    public int arenaHeight = 30;

    //  Grid space markers
    [HideInInspector] public const byte EMPTY = 0;
    [HideInInspector] public const byte P1 = 1;
    [HideInInspector] public const byte P2 = 2;
    [HideInInspector] public const byte HAZARD = 3;

    // Indicate which direction each player is travelling
    [HideInInspector] public const byte UP = 0;
    [HideInInspector] public const byte RIGHT = 1;
    [HideInInspector] public const byte DOWN = 2;
    [HideInInspector] public const byte LEFT = 3;

    private byte[,] board; // Stores positional data in arena
    public byte[,] Board { get { return board; } }

    private GameObject homePlayer;

    void Start()
    {
        // Initialize underlying arena
        board = new byte[arenaHeight, arenaHeight];

        // Ready board and physical arena
        for (int i = 1; i < arenaHeight - 1; i++)
        {
            board[i, 0] = HAZARD;
            board[0, i] = HAZARD;
            board[i, arenaHeight - 1] = HAZARD;
            board[arenaHeight - 1, i] = HAZARD;

            GameObject.Instantiate(fencePrefab, new Vector3(i, 0, 0), Quaternion.identity);
            GameObject.Instantiate(fencePrefab, new Vector3(0, 0, i), Quaternion.identity);
            GameObject.Instantiate(fencePrefab, new Vector3(i, 0, arenaHeight - 1), Quaternion.identity);
            GameObject.Instantiate(fencePrefab, new Vector3(arenaHeight - 1, 0, i), Quaternion.identity);
        }

        // Initialize player
        homePlayer = Instantiate(playerPrefab);
        homePlayer.transform.position = new Vector3((int)(arenaHeight / 3), 0, (int)(arenaHeight / 2));
        // Commence round readying
        onRoundPrepare.Raise();
    }
}
