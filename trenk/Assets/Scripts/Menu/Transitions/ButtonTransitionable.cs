using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTransitionable : Transitionable
{
    public Button[] buttons;

    public override float In()
    {
        foreach (Button button in buttons)
            button.interactable = true;

        return 0;
    }

    public override float Out()
    {
        foreach (Button button in buttons)
            button.interactable = false;

        return 0;
    }
}
