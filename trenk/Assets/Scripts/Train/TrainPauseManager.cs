using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainPauseManager : MonoBehaviour
{
    private void OnEnable()
    {
        InputEventManager.OnPauseEvent += OnPause;
    }

    private void OnDisable()
    {
        InputEventManager.OnPauseEvent -= OnPause;
    }

    public void OnPause()
    {
        Debug.Log("Poop");
        EventManager.Instance.Raise("request-scene", new IntParam(0));
    }
}
