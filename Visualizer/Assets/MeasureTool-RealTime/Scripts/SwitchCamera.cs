using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    public Camera[] cameras; // Array de c�maras para alternar.
    private int currentCameraIndex = 0; // �ndice de la c�mara activa.

    void Start()
    {
        // Activar solo la primera c�mara y desactivar las dem�s.
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(i == currentCameraIndex);
        }
    }

    void Update()
    {
        // Mover a la siguiente c�mara con la flecha derecha.
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SwitchingCamera(1);
        }

        // Mover a la c�mara anterior con la flecha izquierda.
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SwitchingCamera(-1);
        }
    }

    void SwitchingCamera(int direction)
    {
        // Desactivar la c�mara actual.
        cameras[currentCameraIndex].gameObject.SetActive(false);

        // Cambiar el �ndice de la c�mara.
        currentCameraIndex += direction;

        // Asegurarse de que el �ndice est� dentro de los l�mites.
        if (currentCameraIndex < 0)
            currentCameraIndex = cameras.Length - 1;
        else if (currentCameraIndex >= cameras.Length)
            currentCameraIndex = 0;

        // Activar la nueva c�mara.
        cameras[currentCameraIndex].gameObject.SetActive(true);
    }
}
