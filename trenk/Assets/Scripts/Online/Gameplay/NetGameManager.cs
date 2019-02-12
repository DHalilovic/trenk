using System;
using System.Collections;
using UnityEngine;

public class NetGameManager : MonoBehaviour
{
    public string gameStartEvent = "connect";
    public string gameEndEvent = "end";
    public int framesPerStep = 6;
    public GameObject playerPrefab; // Template GameObject for players
    public GameObject fencePrefab; // Template GameObject for borders
    public GameObject minePrefab; // Template GameObject for mines
    public Transform playerParent, fenceParent, mineParent; // Empties for organizational purposes
    public int arenaHeight = 50; // Length, width of square arena

    //  Grid space markers
    [HideInInspector] public const byte EMPTY = 0;
    [HideInInspector] public const byte HOME = 1;
    [HideInInspector] public const byte AWAY = 2;
    [HideInInspector] public const byte HAZARD = 3;

    // Indicate which direction each player is travelling
    [HideInInspector] public const byte UP = 0;
    [HideInInspector] public const byte RIGHT = 1;
    [HideInInspector] public const byte DOWN = 2;
    [HideInInspector] public const byte LEFT = 3;

    public NodeManager Node { get; protected set; }
    public byte[,] Board { get; protected set; } // Stores positional data in arena get 
    public int HomeScore { get; protected set; }
    public int AwayScore { get; protected set; }
    public GameObject HomePlayer { get; protected set; } // Player controlling this device
    public GameObject AwayPlayer { get; protected set; } // Opponent relative to this device
    public byte HomeRot { get; protected set; } // Current player direction
    public byte AwayRot { get; protected set; } // Current opponent direction

    protected Position homePos, awayPos; // Current positions on board
    protected Action<IEventParam> onConnectListener, endListener;
    protected NetRoundManager round;

    // Contains gameObject placement on board and in scene
    protected struct Position
    {
        public int x, y;

        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static bool operator ==(Position a, Position b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(Position a, Position b)
        {
            return !(a == b);
        }
    }

    public virtual void Awake()
    {
        round = GetComponent<NetRoundManager>();

        // Prepare listener
        onConnectListener = new Action<IEventParam>((e) => { round.Ongoing = true; round.enabled = true; Debug.Log("Round start");  });
        endListener = (e) => { round.enabled = false; round.Ongoing = false; };
    }

    public virtual void Start()
    {
        // Initialize underlying arena
        Board = new byte[arenaHeight, arenaHeight];

        // Retrieve Node GameObject
        Node = GameObject.Find("Node").GetComponent<NodeManager>();

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

    protected virtual void OnEnable()
    {
        EventManager e = EventManager.Instance;

        if (e != null)
        {
            EventManager.Instance.Subscribe(gameStartEvent, onConnectListener);
            EventManager.Instance.Subscribe(gameEndEvent, endListener);
            //Debug.Log("Subbed");
        }
    }

    protected virtual void OnDisable()
    {
        EventManager e = EventManager.Instance;

        if (e != null)
        {
            EventManager.Instance.Unsubscribe(gameStartEvent, onConnectListener);
            EventManager.Instance.Unsubscribe(gameEndEvent, endListener);
        }
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

    // Smoovely move player from start to destination
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

    // Move players one space based on position, direction
    public byte Move()
    {
        byte hit = 0;

        // Place mines on board
        Board[homePos.x, homePos.y] = HAZARD;
        Board[awayPos.x, awayPos.y] = HAZARD;
        // Place mines in scene
        GameObject.Instantiate(minePrefab, new Vector3(homePos.x, 0, homePos.y), Quaternion.identity, mineParent);
        GameObject.Instantiate(minePrefab, new Vector3(awayPos.x, 0, awayPos.y), Quaternion.identity, mineParent);

        // Determine resulting local player position via input
        switch (HomeRot)
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

        // Determine resulting opponent position via network messages
        switch (AwayRot)
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

        // If players collide...
        if (homePos == awayPos)
        {
            // Declare tie
            hit = HAZARD;
            Debug.Log("TIE");
        }
        else
        {
            // Otherwise determine if players hit something else
            bool homeSafe = Board[homePos.x, homePos.y] == EMPTY || Board[homePos.x, homePos.y] == HOME;
            bool awaySafe = Board[awayPos.x, awayPos.y] == EMPTY || Board[awayPos.x, awayPos.y] == AWAY;

            // If neither player collided...
            if (homeSafe && awaySafe)
            {
                // Move local players on board
                Board[homePos.x, homePos.y] = HOME;
                Board[awayPos.x, awayPos.y] = AWAY;

                // Move local players in scene
                StartCoroutine(Shift(HomePlayer, HomePlayer.transform.position, new Vector3(homePos.x, 0, homePos.y)));
                StartCoroutine(Shift(AwayPlayer, AwayPlayer.transform.position, new Vector3(awayPos.x, 0, awayPos.y)));
            }
            else if (!homeSafe)
                hit = HOME;
            else if (!awaySafe)
                hit = AWAY;
            else
                hit = HAZARD;
        }

        return hit;
    }
}
