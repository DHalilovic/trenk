using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
public class MovementInput : MonoBehaviour
{
    private Movement m;

    void Start()
    {
        m = GetComponent<Movement>();
    }

    private void OnEnable()
    {
        InputEventManager.OnLeftEvent += OnLeft;
        InputEventManager.OnRightEvent += OnRight;
    }

    private void OnDisable()
    {
        InputEventManager.OnLeftEvent -= OnLeft;
        InputEventManager.OnRightEvent -= OnRight;
    }

    private void OnLeft()
    {
        m.OnLeft();
    }

    private void OnRight()
    {
        m.OnRight();
    }
}
