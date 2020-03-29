using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputBehavior : MonoBehaviour
{
    public GameObject controller;
    public int inputBoxID = 0;  // 1 is gamespeed, 2 is generations, 3 is genes

    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        InputField input = gameObject.GetComponent<InputField>();
        var se = new InputField.SubmitEvent();
        se.AddListener(SubmitValue);
        input.onEndEdit = se;
    }

    private void SubmitValue(string input)
    {
        if (inputBoxID == 1) {  // gamespeed
            float num;
            bool success = float.TryParse(input, out num);
            if (success) controller.GetComponent<ControllerBehavior>().gameSpeed = num;
        }
        else if (inputBoxID == 2)  // generations
        {
            int num;
            bool success = int.TryParse(input, out num);
            if (success) controller.GetComponent<ControllerBehavior>().generations = num;
        }
        else if (inputBoxID == 3)  // genes
        {
            bool success = true;
            string[] inputList = input.Split(' ');
            float[] tempList = new float[3];
            if (inputList.Length == 3)
            {
                success = success && float.TryParse(inputList[0], out tempList[0]);
                success = success && float.TryParse(inputList[1], out tempList[1]);
                success = success && float.TryParse(inputList[2], out tempList[2]);
            }
            else
            {
                success = false;
            }

            if (success)
            {
                controller.GetComponent<ControllerBehavior>().evalEntityGenes[0] = tempList[0];
                controller.GetComponent<ControllerBehavior>().evalEntityGenes[1] = tempList[1];
                controller.GetComponent<ControllerBehavior>().evalEntityGenes[2] = tempList[2];
            }
        }
    }
}
