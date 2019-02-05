using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrainGameManager))]
public class TrainRoundManager : MonoBehaviour, Movement
{
    // Indicate which direction each player is travelling
    [HideInInspector] public const byte STRAIGHT = 0;
    [HideInInspector] public const byte LEFT = 1;
    [HideInInspector] public const byte RIGHT = 2;

    private const byte MAX_QUEUED = 2;

    private TrainGameManager manager;
    private int framesPerStep;
    private int gameStep;
    private int cycleStep;
    private byte nextMove;
    private Queue<byte> moveQueue; // At max 2 long

    private void Start()
    {
        manager = GetComponent<TrainGameManager>();
        framesPerStep = manager.framesPerStep;
        moveQueue = new Queue<byte>();
    }

    public void OnLeft()
    {
        if (moveQueue.Count < MAX_QUEUED)
            moveQueue.Enqueue(LEFT);
    }

    public void OnRight()
    {
        if (moveQueue.Count < MAX_QUEUED)
            moveQueue.Enqueue(RIGHT);
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

            if (moveQueue.Count > 0)
                nextMove = moveQueue.Dequeue();

            switch (nextMove)
            {
                // Change player direction based on input
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

            bool hit = manager.Move();

            // If player hits something...
            if (hit)
            {
                // Call for end of round
                manager.OnRoundEnd.Raise();
            }

            cycleStep = 0; // Reset cycle progress

            // Reset input polling
            nextMove = STRAIGHT;

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
