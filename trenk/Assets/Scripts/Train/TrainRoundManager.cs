using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrainGameStarter))]
public class TrainRoundManager : MonoBehaviour, Movement
{
    public int framesPerStep = 5;

    private TrainGameStarter starter;
    private int gameStep;
    private int cycleStep;
    private bool turnChosen, requestLeft, requestRight;

    private void Start()
    {
        starter = GetComponent<TrainGameStarter>();
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

            //// If an input hasn't been made this cycle...
            //if (!turnChosen)
            //{
            //    // Poll inputs
            //    if (Input.GetKeyDown(KeyCode.A))
            //    {
            //        requestLeft = turnChosen = true;
            //    }
            //    else if (Input.GetKeyDown(KeyCode.D))
            //    {
            //        requestRight = turnChosen = true;
            //    }
            //}
        }
        else
        {
            byte homeRot = 0;

            // Change player direction based on input
            if (requestLeft)
                starter.RotateHomeLeft();
            else if (requestRight)
                starter.RotateHomeRight();
            else
                homeRot = starter.HomeRot;

            bool hit = starter.MovePlayer();

            //if (hit)
                

            cycleStep = 0; // Reset cycle progress

            // Reset input polling
            turnChosen = requestLeft = requestRight = false;
        }

        // Increment overall frame counter
        gameStep++;
    }

    public void Reset()
    {
        gameStep = 0;
        cycleStep = 0;
    }
}
