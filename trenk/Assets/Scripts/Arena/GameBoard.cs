using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard
{
    //  Grid space markers
    [HideInInspector] public const byte EMPTY = 0;
    [HideInInspector] public const byte P1 = 1;
    [HideInInspector] public const byte P2 = 2;
    [HideInInspector] public const byte HAZARD = 3;

    // Indicate which direction each player is travelling
    [HideInInspector] public const byte UP = 0;
    [HideInInspector] public const byte RIGHT = 1;
    [HideInInspector] public const byte DOWN = 2;
    [HideInInspector] public const byte LEFT = 3;

    private static byte[,] board; // Stores positional data in arena
    public static byte[,] Board { get { return board; } }

    public static void Reset(int arenaHeight)
    {
        // Create square board
        board = new byte[arenaHeight, arenaHeight];

        // Set up boundaries
        for (int i = 1; i < arenaHeight - 1; i++)
        {
            board[i, 0] = HAZARD;
            board[0, i] = HAZARD;
            board[i, arenaHeight - 1] = HAZARD;
            board[arenaHeight - 1, i] = HAZARD;
        }
    }
}
