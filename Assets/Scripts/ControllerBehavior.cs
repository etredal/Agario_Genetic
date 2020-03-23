using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerBehavior : MonoBehaviour
{
    public float gameSpeed = 1f;

    private void Start()
    {
        // The speed at which the game is played.  1f is realtime.
        Time.timeScale = gameSpeed;
    }
}
