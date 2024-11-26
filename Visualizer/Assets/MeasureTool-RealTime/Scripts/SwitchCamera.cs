using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    public Camera[] cameras; // Array de cámaras para alternar.
    private int currentCameraIndex = 0; // Índice de la cámara activa.

    void Start()
    {
        // Activar solo la primera cámara y desactivar las demás.
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(i == currentCameraIndex);
        }
    }

    void Update()
    {
        // Mover a la siguiente cámara con la flecha derecha.
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SwitchingCamera(1);
        }

        // Mover a la cámara anterior con la flecha izquierda.
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SwitchingCamera(-1);
        }
    }

    void SwitchingCamera(int direction)
    {
        // Desactivar la cámara actual.
        cameras[currentCameraIndex].gameObject.SetActive(false);

        // Cambiar el índice de la cámara.
        currentCameraIndex += direction;

        // Asegurarse de que el índice esté dentro de los límites.
        if (currentCameraIndex < 0)
            currentCameraIndex = cameras.Length - 1;
        else if (currentCameraIndex >= cameras.Length)
            currentCameraIndex = 0;

        // Activar la nueva cámara.
        cameras[currentCameraIndex].gameObject.SetActive(true);
    }
}
