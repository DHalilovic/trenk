using System;
using UnityEngine;

[RequireComponent(typeof(NetGameManager))]
public class NetRoundManager : MonoBehaviour, Movement
{
    public int framesPerStep = 5;

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
    private Message currentMessage;

    private void Start()
    {
        manager = GetComponent<NetGameManager>();
    }

    public void OnLeft()
    {
        if (!moveChosen)
        {
            moveChosen = true;
            nextHomeMove = LEFT;
            SendInput(nextHomeMove);
        }
    }

    public void OnRight()
    {
        if (!moveChosen)
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
        // Send last-minute "straight" message if no move chosen
        if (cycleStep == framesPerStep - 1 && !moveChosen)
            SendInput(nextHomeMove);

        // Wait for player inputs for some frames
        if (currentMessage == null || cycleStep < framesPerStep - 1)
        {
            // Increment cycle progress
            cycleStep++;

            //Debug.Log(gameStep);

            // Get message if possible
            if (currentMessage == null && manager.Node.MessageQueue.Count > 0)
            {
                currentMessage = manager.Node.MessageQueue.Dequeue();

                // Process message
                if (currentMessage.Type == Message.MessageType.INPUT)
                {
                    InputMessage body = (InputMessage)currentMessage.Body;

                    short awayGameStep = body.GameStep;
                    nextAwayMove = body.Direction;
                }
                else
                    currentMessage = null;
            }
        }
        else
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
            }

            cycleStep = 0; // Reset cycle progress

            // Reset input polling
            nextHomeMove = nextAwayMove = STRAIGHT;
            moveChosen = false;

            // Discard current message
            currentMessage = null;

            // Increment overall frame counter
            gameStep++;
        }
    }

    // Reset steps for new round gameplay
    public void Reset()
    {
        gameStep = 0;
        cycleStep = 0;
    }
}
