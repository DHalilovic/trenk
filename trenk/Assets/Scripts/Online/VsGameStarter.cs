using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VsGameStarter : MonoBehaviour
{
    public int scoreCap = 5; // Score required to win match
    public GameObject playerPrefab; // Template GameObject for players
    public GameObject fencePrefab; // Template GameObject for borders
    public GameObject minePrefab; // Template GameObject for mines
    public Transform playerParent, fenceParent, mineParent; // Empties for organizational purposes
    public int arenaHeight = 50; // Length, width of square arena

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

    public byte[,] Board { get; protected set; } // Stores positional data in arena get 
    public int HomeScore { get; protected set; }
    public int AwayScore { get; protected set; }
    public GameObject HomePlayer { get; protected set; } // Player controlling this device
    public GameObject AwayPlayer { get; protected set; } // Opponent relative to this device
    public byte HomeRot { get; protected set; } // Current player direction
    public byte AwayRot { get; protected set; } // Current opponent direction

    private Position homePos, awayPos; // Current positions on board

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

    void Start()
    {
        // Initialize underlying arena
        Board = new byte[arenaHeight, arenaHeight];

        // Ready board and physical arena
        for (int i = 1; i < arenaHeight - 1; i++)
        {
            // Place hazards on board
            Board[i, 0] = HAZARD;
            Board[0, i] = HAZARD;
            Board[i, arenaHeight - 1] = HAZARD;
            Board[arenaHeight - 1, i] = HAZARD;

            // Place hazards in scene
            GameObject.Instantiate(fencePrefab, new Vector3(i, 0, 0), Quaternion.identity, fenceParent);
            GameObject.Instantiate(fencePrefab, new Vector3(0, 0, i), Quaternion.identity, fenceParent);
            GameObject.Instantiate(fencePrefab, new Vector3(i, 0, arenaHeight - 1), Quaternion.identity, fenceParent);
            GameObject.Instantiate(fencePrefab, new Vector3(arenaHeight - 1, 0, i), Quaternion.identity, fenceParent);
        }

        // Place players on board (home on left, away on right)
        homePos = new Position(arenaHeight / 4, arenaHeight / 2);
        HomeRot = RIGHT;
        awayPos = new Position(3 * arenaHeight / 4, arenaHeight / 2);
        AwayRot = LEFT;

        // Spawn players in scene
        HomePlayer = Instantiate(playerPrefab, playerParent);
        AwayPlayer = Instantiate(playerPrefab, playerParent);
        // Position players in scene
        HomePlayer.transform.position = new Vector3(homePos.x, 0, homePos.y);
        AwayPlayer.transform.position = new Vector3(awayPos.x, 0, awayPos.y);
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
        HomeRot = ClampRotation(HomeRot + 1);
        return HomeRot;
    }

    public byte RotateHomeRight()
    {
        HomeRot = ClampRotation(HomeRot - 1);
        return HomeRot;
    }

    public byte RotateAwayLeft()
    {
        AwayRot = ClampRotation(AwayRot + 1);
        return AwayRot;
    }

    public byte RotateAwayRight()
    {
        AwayRot = ClampRotation(AwayRot - 1);
        return AwayRot;
    }
}
