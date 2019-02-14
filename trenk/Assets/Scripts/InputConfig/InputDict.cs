using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputDict : Singleton<InputDict>
{
    private Dictionary<string, KeyCode> keyCodes;

    public KeyCode getKeyCode(string input)
    {
        return keyCodes[input];
    }

    protected override void Awake()
    {
        base.Awake();

        keyCodes = new Dictionary<string, KeyCode>();

        keyCodes.Add("pause", KeyCode.Backspace);
        keyCodes.Add("confirm", KeyCode.Space);
        keyCodes.Add("up", KeyCode.W);
        keyCodes.Add("down", KeyCode.S);
        keyCodes.Add("left", KeyCode.A);
        keyCodes.Add("right", KeyCode.D);
        keyCodes.Add("up", KeyCode.UpArrow);
        keyCodes.Add("down", KeyCode.DownArrow);
        keyCodes.Add("left", KeyCode.LeftArrow);
        keyCodes.Add("right", KeyCode.RightArrow);
    }
}
