using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetGameManager))]
public class NetRoundManager : MonoBehaviour, Movement
{
    public bool Ongoing { get; set; }

    // Indicate which direction each player is travelling
    [HideInInspector] public const byte STRAIGHT = 0;
    [HideInInspector] public const byte LEFT = 1;
    [HideInInspector] public const byte RIGHT = 2;

    private const byte MAX_QUEUED = 2;

    private NetGameManager manager;
    private int framesPerStep;
    private short gameStep;
    private short cycleStep;
    private bool moveChosen;
    private byte nextHomeMove, nextAwayMove;
    private byte hit;
    private Message currentInputMessage;
    private Queue<byte> moveQueue; // At max 2 long
    private Dictionary<short, byte> homeHist, awayHist; // Move histories since last agreed gamestep
    private short lastAgreedStep; // Gamestep on which both players last synced; smallest gamestep between both players

    private void Awake()
    {
        Ongoing = false;
        manager = GetComponent<NetGameManager>();
        framesPerStep = manager.framesPerStep;
        moveQueue = new Queue<byte>();
        homeHist = new Dictionary<short, byte>();
        awayHist = new Dictionary<short, byte>();
    }

    public void OnLeft()
    {
        // If input queue not filled, accept input
        if (Ongoing && moveQueue.Count < MAX_QUEUED)
        {
            moveQueue.Enqueue(LEFT);
        }
    }

    public void OnRight()
    {
        // If input queue not filled, accept input
        if (Ongoing && moveQueue.Count < MAX_QUEUED)
        {
            moveQueue.Enqueue(RIGHT);
        }
    }

    private void SendInput()
    {
        short numMoves = (short)(gameStep - lastAgreedStep + 1);
        short length = (short)(2 + numMoves);

        byte[] sendStep = BitConverter.GetBytes(lastAgreedStep);
        byte[] body = new byte[length];

        body[0] = sendStep[0];
        body[1] = sendStep[1];

        //Debug.Log("Home history Count: " + homeHist.Count);

        for (short i = 0; i < numMoves; i++)
            body[i + 2] = homeHist[(short)(lastAgreedStep + i)];

        //Debug.Log("Home Body 1st: " + body[2]);

        manager.Node.Net.Send((byte)Message.MessageType.INPUT, length, body);
    }

    private void FixedUpdate()
    {
        if (Ongoing)
        {
            // Get messages
            if (currentInputMessage == null && manager.Node.MessageQueue.Count > 0)
            {
                bool found = false; // Determines whether next away move "found"
                currentInputMessage = manager.Node.MessageQueue.Dequeue();

                // Discard message if not input
                if (currentInputMessage.Type == Message.MessageType.INPUT)
                {
                    //Debug.Log("Receiving input");
                    // Retrieve input message
                    InputMessage body = (InputMessage)currentInputMessage.Body;
                    short curStep = body.GameStep;

                    //Debug.Log("Received input");
                    //Debug.Log("Remote gamestep " + curStep);
                    //Debug.Log("Local gamestep " + gameStep);
                    //Debug.Log("Remote moves count : " + body.Moves.Count);

                    // Add new moves to away history
                    for (short i = 0; i < body.Moves.Count; i++, curStep++)
                    {
                        //Debug.Log("Considering move");
                        // Add only if move's gameStep matches/exceeds local gamestep
                        if (curStep >= gameStep)
                        {
                            //Debug.Log("Adding move");
                            if (!awayHist.ContainsKey((short)(body.GameStep + i)))
                                awayHist.Add((short)(body.GameStep + i), body.Moves[i]);
                            //Debug.Log("Move added");
                        }
                    }

                    //Debug.Log("Done adding");

                    // Remove obsolete moves from away history
                    for (short i = lastAgreedStep; i < gameStep; i++)
                    {
                        //Debug.Log("Removing obsolete");
                        awayHist.Remove(i);
                        //Debug.Log("Done removing");
                    }

                    //Debug.Log("Done removing");

                    // If desired gamestep not present, simply wait until next message
                    if (awayHist.ContainsKey(gameStep))
                    {
                        found = true;
                        nextAwayMove = awayHist[gameStep];

                        // Store latest sync progress
                        lastAgreedStep = gameStep;
                    }
                }

                // If move not found, wait until next message
                if (!found)
                {
                    //Debug.Log("Not found");
                    currentInputMessage = null;
                }
            }

            // Wait for player inputs for some frames
            if (cycleStep < framesPerStep)
            {
                // Increment cycle progress
                cycleStep++;

                // Process movement input if available
                if (!moveChosen && moveQueue.Count > 0)
                {
                    //Debug.Log("Processing move");
                    nextHomeMove = moveQueue.Dequeue();
                    homeHist.Add(gameStep, nextHomeMove);
                    SendInput();
                    moveChosen = true;
                }
            }

            if (cycleStep == framesPerStep - 1 && !moveChosen)
            {
                // Send last-second "straight" message if no local input received
                //Debug.Log("Making last-second send");
                nextHomeMove = STRAIGHT;
                homeHist.Add(gameStep, STRAIGHT);
                SendInput();
                moveChosen = true;
            }

            // Move board only when message received and current cycle completed
            if (currentInputMessage != null
                && moveChosen
                && cycleStep > framesPerStep - 1)
            {
                byte homeRot = 0, awayRot = 0;

                // Change player direction based on input
                switch (nextHomeMove)
                {
                    case LEFT:
                        homeRot = manager.RotateHomeLeft();
                        break;
                    case RIGHT:
                        homeRot = manager.RotateHomeRight();
                        break;
                    default:
                        homeRot = manager.HomeRot;
                        break;
                }

                // Change opponent direction based on input
                switch (nextAwayMove)
                {
                    case LEFT:
                        awayRot = manager.RotateAwayLeft();
                        break;
                    case RIGHT:
                        awayRot = manager.RotateAwayRight();
                        break;
                    default:
                        awayRot = manager.AwayRot;
                        break;
                }

                // Move players
                hit = manager.Move();

                // If a player hits something...
                if (hit != 0)
                {
                    // Call for end of round
                    EventManager.Instance.Raise("end", new ByteParam(hit));
                }

                cycleStep = 0; // Reset cycle progress

                // Reset input polling
                nextHomeMove = nextAwayMove = STRAIGHT;
                moveChosen = false;

                // Discard current message
                currentInputMessage = null;

                // Increment overall frame counter
                gameStep++;
            }
        }
    }

    // Reset steps for new round gameplay
    public void Reset()
    {
        gameStep = 0;
        cycleStep = 0;
        lastAgreedStep = 0;
    }
}
