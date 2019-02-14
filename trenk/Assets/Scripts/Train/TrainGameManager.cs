using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainGameManager : MonoBehaviour
{
    // Contains gameObject placement on board and in scene
    private struct Position
    {
        public int x, y;

        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public GameObject playerPrefab; // Template GameObject for players
    public GameObject fencePrefab; // Template GameObject for borders
    public GameObject minePrefab; // Template GameObject for mines
    public GameEvent onRoundPrepare, OnRoundEnd; // Events to signal
    public int arenaHeight = 30; // Length, width of square arena
    public int framesPerStep = 3;

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
    private GameObject awayPlayer;

    private Transform playerParent, fenceParent, mineParent; // Empties for organizational purposes

    public GameObject HomePlayer { get { return homePlayer; } }
    public GameObject AwayPlayer { get { return awayPlayer;  } }
    
    private byte homeRot; // Current player direction
    private byte awayRot; // Away player direction
    public byte HomeRot { get { return homeRot; } }
    public byte AwayRot { get { return awayRot; } }

    private Position homeSpawn; // Original spwan point
    private Position homePos; // Current player position
    private Position awaySpawn;
    private Position awayPos;

    void Start()
    {
        // Initialize underlying arena
        board = new byte[arenaHeight, arenaHeight];

        // Get organizers
        playerParent = GameObject.Find("Players").transform;
        fenceParent = GameObject.Find("Fences").transform;
        mineParent = GameObject.Find("Mines").transform;

        // Ready board and physical arena
        for (int i = 1; i < arenaHeight - 1; i++)
        {
            // Place hazards on board
            board[i, 0] = HAZARD;
            board[0, i] = HAZARD;
            board[i, arenaHeight - 1] = HAZARD;
            board[arenaHeight - 1, i] = HAZARD;

            // Place hazards in scene
            GameObject.Instantiate(fencePrefab, new Vector3(i, 0, 0), Quaternion.identity, fenceParent);
            GameObject.Instantiate(fencePrefab, new Vector3(0, 0, i), Quaternion.identity, fenceParent);
            GameObject.Instantiate(fencePrefab, new Vector3(i, 0, arenaHeight - 1), Quaternion.identity, fenceParent);
            GameObject.Instantiate(fencePrefab, new Vector3(arenaHeight - 1, 0, i), Quaternion.identity, fenceParent);
        }

        // Place player on board
        homePos = homeSpawn = new Position(arenaHeight / 3, arenaHeight / 2);
        homeRot = RIGHT;

        // Place away player on board
        awayPos = new Position(arenaHeight - arenaHeight / 3, arenaHeight / 2);
        awayRot = LEFT;
        // Initialize player in scene
        homePlayer = Instantiate(playerPrefab, playerParent);
        homePlayer.transform.position = new Vector3(homePos.x, 0, homePos.y);
        //Initialize away player in scene
        awayPlayer = Instantiate(playerPrefab, playerParent);
        awayPlayer.transform.position = new Vector3(awayPos.x, 0, awayPos.y);

        // Commence round readying
        //onRoundPrepare.Raise();
        EventManager.Instance.Raise("round-prepare", null);
    }

    // Prevent rotation from exceeding one full cycle
    private byte ClampRotation(int rot)
    {
        if (rot < 0)
            rot = 3;
        else if (rot > 3)
            rot = 0;

        return (byte)(rot);
    }

    public byte RotateHomeLeft()
    {
        homeRot = ClampRotation(homeRot + 1);
        return homeRot;
    }

    public byte RotateHomeRight()
    {
        homeRot = ClampRotation(homeRot - 1);
        return homeRot;
    }

    public byte RotateAwayLeft()
    {
        awayRot = ClampRotation(awayRot + 1);
        return awayRot;
    }

    public byte RotateAwayRight()
    {
        awayRot = ClampRotation(awayRot - 1);
        return awayRot;
    }

    // Smoothly move player from start to destination
    IEnumerator Shift(GameObject o, Vector3 start, Vector3 end)
    {
        for (int i = 0; i < framesPerStep; i++)
        {
            // Move object towards destination in even increments
            o.transform.position = Vector3.Lerp(start, end, 1.0f * i / framesPerStep);
            // Wait a frame
            yield return new WaitForFixedUpdate();
        }
    }

    // Move player one space based on position, direction
    public bool Move()
    {
        bool hit = false;

        // Place mine on board for each player
        board[homePos.x, homePos.y] = HAZARD;
        board[awayPos.x, awayPos.y] = HAZARD;

        // Place mine in scene
        GameObject.Instantiate(minePrefab, new Vector3(homePos.x, 0, homePos.y), Quaternion.identity, mineParent);
        GameObject.Instantiate(minePrefab, new Vector3(awayPos.x, 0, awayPos.y), Quaternion.identity, mineParent);

        // Determine resulting player position
        switch (homeRot)
        {
            case UP:
                homePos.y--;
                break;
            case RIGHT:
                homePos.x++;
                break;
            case DOWN:
                homePos.y++;
                break;
            case LEFT:
                homePos.x--;
                break;
        }

        // Determine resulting away player position
        switch (awayRot)
        {
            case UP:
                awayPos.y--;
                break;
            case RIGHT:
                awayPos.x++;
                break;
            case DOWN:
                awayPos.y++;
                break;
            case LEFT:
                awayPos.x--;
                break;
        }

        //// If no collisions...
        //if (board[homePos.x, homePos.y] == EMPTY)
        //{
        //    // Move player on board
        //    board[homePos.x, homePos.y] = P1;
        //    // Move player in scene
        //    //homePlayer.transform.position = new Vector3(homePos.x, 0, homePos.y);
        //    StartCoroutine(Shift(homePlayer, homePlayer.transform.position, new Vector3(homePos.x, 0, homePos.y)));
        //}
        //else
        //    hit = true;

        //return hit;

        // If no collisions...
        if (board[homePos.x, homePos.y] == EMPTY && board[awayPos.x, awayPos.y] == EMPTY)
        {
            // Move player on board
            board[homePos.x, homePos.y] = P1;
            // Move away player on board
            board[awayPos.x, awayPos.y] = P2;
            // Move player in scene
            //homePlayer.transform.position = new Vector3(homePos.x, 0, homePos.y);
            StartCoroutine(Shift(homePlayer, homePlayer.transform.position, new Vector3(homePos.x, 0, homePos.y)));
            // Move away player in scene
            StartCoroutine(Shift(awayPlayer, awayPlayer.transform.position, new Vector3(awayPos.x, 0, awayPos.y)));
        }
        else
            hit = true;

        return hit;
    }

    // Remove mines from board, scene
    public void Clean()
    {
        foreach (Transform child in mineParent.transform)
            GameObject.Destroy(child.gameObject);

        for (int i = 1; i < arenaHeight - 1; i++)
        {
            for (int j = 1; j < arenaHeight - 1; j++)
            {
                if (board[i, j] != EMPTY)
                    board[i, j] = EMPTY;
            }
        }

        homePos = homeSpawn;
        homeRot = RIGHT;

        homePlayer.transform.position = new Vector3(homePos.x, 0, homePos.y);
    }
}
