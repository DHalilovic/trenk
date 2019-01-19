using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputEventManager : MonoBehaviour
{
    public delegate void OnPause();
    public static event OnPause OnPauseEvent;

    public delegate void OnJump();
    public static event OnJump OnJumpEvent;

    public delegate void OnLeft();
    public static event OnLeft OnLeftEvent;

    public delegate void OnRight();
    public static event OnRight OnRightEvent;

    public static void RaiseOnPause()
    {
        if (OnPauseEvent != null)
            OnPauseEvent();
    }

    public static void RaiseOnJump()
    {
        if (OnJumpEvent != null)
            OnJumpEvent();
    }

    public static void RaiseOnLeft()
    {
        if (OnLeftEvent != null)
            OnLeftEvent();
    }

    public static void RaiseOnRight()
    {
        if (OnRightEvent != null)
            OnRightEvent();
    }
}
