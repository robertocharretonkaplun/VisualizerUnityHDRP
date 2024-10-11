using UnityEngine;

public class ResizableWall : MonoBehaviour
{
    public Transform windowHole; // La ventana o el agujero de la ventana
    public float wallWidth = 10f; // Ancho de la pared
    public float wallHeight = 5f; // Altura de la pared
    public float holeWidth = 2f;  // Ancho del agujero
    public float holeHeight = 2f; // Altura del agujero

    void Update()
    {
        // Cambiar el tamaño de la pared
        transform.localScale = new Vector3(wallWidth, wallHeight, transform.localScale.z);

        // Posicionar el agujero de la ventana en el centro de la pared
        if (windowHole != null)
        {
            windowHole.localScale = new Vector3(holeWidth, holeHeight, windowHole.localScale.z);
            windowHole.localPosition = new Vector3(0, 0, 0); // Asegurar que el agujero esté centrado o cambiar si es necesario
        }
    }
}
