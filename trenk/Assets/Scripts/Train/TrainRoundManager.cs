using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrainGameStarter))]
public class TrainRoundManager : MonoBehaviour
{
    public int frameStep = 5;

    private byte[,] board;
    private int gameFrame;
    private int cycleFrame;
    private bool turnChosen, requestLeft, requestRight;

    private void Start()
    {
        board = GetComponent<TrainGameStarter>().Board;
    }

    private void FixedUpdate()
    {
        // Wait for player inputs for some frames
        if (cycleFrame < frameStep)
        {
            cycleFrame++; // Increment cycle progress

            // If an input hasn't been made this cycle...
            if (!turnChosen)
            {
                // Poll inputs
                if (Input.GetKeyDown(KeyCode.A))
                {
                    requestLeft = turnChosen = true;
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    requestRight = turnChosen = true;
                }
            }
        }
        else
        {
            // Change world transforms
            if (requestLeft)
                Debug.Log("l");
            else if (requestRight)
                Debug.Log("r");
            else
                Debug.Log("_");

            cycleFrame = 0; // Reset cycle progress
            // Reset input polling
            turnChosen = requestLeft = requestRight = false;
        }

        // Increment overall frame counter
        gameFrame++;
    }
}
