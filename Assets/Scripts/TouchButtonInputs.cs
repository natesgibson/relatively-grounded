using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchButtonInputs : MonoBehaviour
{
    public static bool uiPlayerJump;
    public static bool uiPlayerCounter;
    public static bool uiPlayerClock;

    private void Start()
    {
        uiPlayerJump = false;
        uiPlayerCounter = false;
        uiPlayerClock = false;
    }

    public void TogglePlayerJump()
    {
        uiPlayerJump = !uiPlayerJump;
    }

    public void TogglePlayerRotateCounter()
    {
        uiPlayerCounter = !uiPlayerCounter;
    }

    public void TogglePlayerRotateClock()
    {
        uiPlayerClock = !uiPlayerClock;
    }
}
