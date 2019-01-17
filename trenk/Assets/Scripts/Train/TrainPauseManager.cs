using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainPauseManager : MonoBehaviour
{
    public GameEvent onPause, onUnpause;

    private bool paused;

    private void OnEnable()
    {
        InputEventManager.OnPauseEvent += Pause;
    }

    // Call for pausing or unpausing
    private void Pause()
    {
        if (paused)
            onUnpause.Raise();
        else
            onPause.Raise();

        // Switch paused state
        paused = !paused;
    }
}
