﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonBehavior : MonoBehaviour
{
    public void ButtonOneBehavior()
    {
        SceneManager.LoadScene("Generation");
    }

    public void ButtonTwoBehavior()
    {
        SceneManager.LoadScene("Evaluation");
    }
}
