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
    private LinkedList<InputMessage> homeHist, awayHist; // Move histories since last agreed gamestep
    private short lastAgreedStep; // Gamestep on which both players synced

    private void Awake()
    {
        Ongoing = false;
        manager = GetComponent<NetGameManager>();
        framesPerStep = manager.framesPerStep;
        moveQueue = new Queue<byte>();
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

    private void SendInput(byte b)
    {
        byte[] sendStep = BitConverter.GetBytes(gameStep);
        manager.Node.Net.Send((byte)Message.MessageType.INPUT, 3, new byte[] { sendStep[0], sendStep[1], b });
    }

    private void FixedUpdate()
    {
        if (Ongoing)
        {
            // Get message if possible
            if (currentInputMessage == null && manager.Node.MessageQueue.Count > 0)
            {
                currentInputMessage = manager.Node.MessageQueue.Dequeue();

                // Process or discard message
                if (currentInputMessage.Type == Message.MessageType.INPUT)
                {
                    InputMessage body = (InputMessage)currentInputMessage.Body;
                    nextAwayMove = body.Direction;
                }
                else
                    currentInputMessage = null;
            }

            // Wait for player inputs for some frames
            if (cycleStep < framesPerStep)
            {
                // Increment cycle progress
                cycleStep++;
                
                //Debug.Log("CS " + cycleStep);

                // Process movement input if available
                if (!moveChosen && moveQueue.Count > 0)
                {
                    nextHomeMove = moveQueue.Dequeue();
                    SendInput(nextHomeMove);
                    moveChosen = true;
                }
            }

            if (cycleStep == framesPerStep - 1 && !moveChosen)
            {
                // Send last-second "straight" message if no local input received
                SendInput(nextHomeMove);
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

                    // Disable self
                    //this.enabled = false;
                }

                cycleStep = 0; // Reset cycle progress

                // Reset input polling
                nextHomeMove = nextAwayMove = STRAIGHT;
                moveChosen = false;

                // Discard current message
                currentInputMessage = null;

                // Increment overall frame counter
                gameStep++;

               //Debug.Log("GS " + gameStep);
            }
        }
    }

    // Reset steps for new round gameplay
    public void Reset()
    {
        gameStep = 0;
        cycleStep = 0;
    }
}
