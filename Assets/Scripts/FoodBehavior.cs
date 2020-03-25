using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBehavior : MonoBehaviour
{
    private bool one = false;
    public bool active = false;
    
    // Start is called before the first frame update
    void FixedUpdate()
    {
        if (one)
        {
            active = true;
        }
        else
        {
            one = true;
        }
    }
}
