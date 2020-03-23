using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointBehavior : MonoBehaviour
{
    private void FixedUpdate()
    {
        // Keeping local transform
        Transform oldParent = transform.parent;
        transform.parent = null;
        transform.localScale = new Vector3(0.05f, 0.05f);
        transform.parent = oldParent;
    }
}
