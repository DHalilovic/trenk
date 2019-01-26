using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetGameManager))]
public class NetRoundManager : MonoBehaviour, Movement
{
    public int framesPerStep = 5;

    private NetGameManager manager;
    private int gameStep;
    private int cycleStep;
    private bool turnChosen, requestLeft, requestRight;
    private byte hit;

    private void Start()
    {
        manager = GetComponent<NetGameManager>();
    }

    public void OnLeft()
    {
        if (!turnChosen)
            requestLeft = turnChosen = true;
    }

    public void OnRight()
    {
        if (!turnChosen)
            requestRight = turnChosen = true;
    }

    private void FixedUpdate()
    {
        // Wait for player inputs for some frames
        if (cycleStep < framesPerStep)
        {
            cycleStep++; // Increment cycle progress
        }
        else
        {
            byte homeRot = 0;

            // Change player direction based on input
            if (requestLeft)
                homeRot = manager.RotateHomeLeft();
            else if (requestRight)
                homeRot = manager.RotateHomeRight();
            else
                homeRot = manager.HomeRot;

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
            turnChosen = requestLeft = requestRight = false;
        }

        // Increment overall frame counter
        gameStep++;
    }

    // Reset steps for new round gameplay
    public void Reset()
    {
        gameStep = 0;
        cycleStep = 0;
    }
}
