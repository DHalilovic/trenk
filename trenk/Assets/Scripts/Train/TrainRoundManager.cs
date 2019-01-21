using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrainGameStarter))]
public class TrainRoundManager : MonoBehaviour, Movement
{
    private TrainGameStarter starter;
    private int framesPerStep;
    private int gameStep;
    private int cycleStep;
    private bool turnChosen, requestLeft, requestRight;

    private void Start()
    {
        starter = GetComponent<TrainGameStarter>();
        framesPerStep = starter.framesPerStep;
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
                homeRot = starter.RotateHomeLeft();
            else if (requestRight)
                homeRot = starter.RotateHomeRight();
            else
                homeRot = starter.HomeRot;

            bool hit = starter.Move();
            
            // If player hits something...
            if (hit)
            {
                // Call for end of round
                starter.OnRoundEnd.Raise();
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
