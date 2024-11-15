using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private void Update()
    {
        if (Camera.main != null)
        {
            transform.LookAt(Camera.main.transform);
            //Adjust text to dont inverse
            transform.Rotate(0, 180, 0);  
        }
    }
}
