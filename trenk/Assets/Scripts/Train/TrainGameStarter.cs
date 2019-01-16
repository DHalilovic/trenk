using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainGameStarter : MonoBehaviour
{
    public GameObject playerPrefab; // Template GameObject for players
    public GameObject fencePrefab;
    public GameEvent onRoundPrepare;
    public int arenaHeight = 30;

    private GameObject homePlayer;

    void Start()
    {
        // Initialize underlying arena
        GameBoard.Reset(arenaHeight);

        // Ready physical arena
        for (int i = 1; i < arenaHeight - 1; i++)
        {
            GameObject.Instantiate(fencePrefab, new Vector3(i, 0, 0), Quaternion.identity);
            GameObject.Instantiate(fencePrefab, new Vector3(0, 0, i), Quaternion.identity);
            GameObject.Instantiate(fencePrefab, new Vector3(i, 0, arenaHeight - 1), Quaternion.identity);
            GameObject.Instantiate(fencePrefab, new Vector3(arenaHeight - 1, 0, i), Quaternion.identity);
        }

        // Initialize player
        homePlayer = Instantiate(playerPrefab);
        // Commence round readying
        onRoundPrepare.Raise();
    }
}
