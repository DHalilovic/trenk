using System;
using UnityEngine;

[RequireComponent(typeof(NetGameManager))]
public class NetRoundManager : MonoBehaviour, Movement
{
    public int framesPerStep = 5;

    public bool Ongoing { get; set; }

    // Indicate which direction each player is travelling
    [HideInInspector] public const byte STRAIGHT = 0;
    [HideInInspector] public const byte LEFT = 1;
    [HideInInspector] public const byte RIGHT = 2;

    private NetGameManager manager;
    private short gameStep;
    private short cycleStep;
    private bool moveChosen;
    private byte nextHomeMove, nextAwayMove;
    private byte hit;
    private Message currentInputMessage;

    private void Awake()
    {
        Ongoing = false;
        manager = GetComponent<NetGameManager>();
    }

    public void OnLeft()
    {
        if (!moveChosen && Ongoing)
        {
            moveChosen = true;
            nextHomeMove = LEFT;
            SendInput(nextHomeMove);
        }
    }

    public void OnRight()
    {
        if (!moveChosen && Ongoing)
        {
            moveChosen = true;
            nextHomeMove = RIGHT;
            SendInput(nextHomeMove);
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
            if (cycleStep < framesPerStep - 1)
            {
                // Increment cycle progress
                cycleStep++;
            }
            else if (cycleStep == framesPerStep - 1 && !moveChosen)
            {
                // Send last-second "straight" message if no local input received
                SendInput(nextHomeMove);
            }

            // Move board only when message received and current cycle completed
            if (currentInputMessage != null && cycleStep >= framesPerStep - 1)
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
                    // Check who died
                    switch (hit)
                    {
                        case NetGameManager.HOME:
                            // Local player died

                            break;
                        case NetGameManager.AWAY:
                            // Opponent died

                            break;
                        case NetGameManager.HAZARD:
                            // Both players died simultanously

                            break;
                    }

                    // Call for end of round
                    //manager.OnRoundEnd.Raise();
                    this.enabled = false;
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
    }
}
