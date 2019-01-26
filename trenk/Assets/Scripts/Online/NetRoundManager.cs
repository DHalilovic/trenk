using System.Collections;
using System.Collections.Generic;
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
    private int gameStep;
    private int cycleStep;
    private bool turnChosen;
    private byte nextHomeMove, nextAwayMove;
    private byte hit;
    private Message currentMessage;

    private void Start()
    {
        manager = GetComponent<NetGameManager>();
    }

    public void OnLeft()
    {
        if (!turnChosen)
        {
            turnChosen = true;
            nextHomeMove = LEFT;
        }
    }

    public void OnRight()
    {
        if (!turnChosen)
        {
            turnChosen = true;
            nextHomeMove = RIGHT;
        }
    }

    private void FixedUpdate()
    {
        // Wait for player inputs for some frames
        if (cycleStep < framesPerStep || currentMessage == null)
        {
            cycleStep++; // Increment cycle progress

            //Debug.Log(gameStep);

            // Get message if possible
            if (currentMessage == null && manager.node.MessageQueue.Count > 0)
            {
                currentMessage = manager.node.MessageQueue.Dequeue();
                // Process message

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
            nextHomeMove = STRAIGHT;
            turnChosen = false;

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
