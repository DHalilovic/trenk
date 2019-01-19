using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputEventHandler : MonoBehaviour {
    private InputDict inp;

    void Start()
    {
        inp = InputDict.Instance;
    }

    void Update()
    {
        if (Input.GetKeyDown(inp.getKeyCode("left")))
        {
            InputEventManager.RaiseOnLeft();
        }
        if (Input.GetKeyDown(inp.getKeyCode("right")))
        {
            InputEventManager.RaiseOnRight();
        }
    }
}
