using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFirstPerson : MonoBehaviour
{
    //Rotation speed
    public float rotationSpeed = 100f;

    //Reference to capsule/Player
    public Transform playerBody;

    // Degrees to limit x rotation
    private float xRotation = 0f; 

    void Update()
    {
        //If push right mouse button
        if (Input.GetMouseButton(1)) 
        {
            //Rotate on X axis and rotate my capsule/player
            float rotationX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            playerBody.Rotate(Vector3.up * rotationX);

            //Rotate on Y axis and only rotate camera
            float rotationY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            //Less rotationY to can move camera up or down
            xRotation -= rotationY;

            //Make a clamp to limit rotation degrees and u cant rotate camera back to u
            xRotation = Mathf.Clamp(xRotation, -90f, 90f); 

            //Rotation only affects to my camera
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }
}
