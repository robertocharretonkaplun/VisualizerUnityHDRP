using UnityEngine;

public class WindowTracker : MonoBehaviour
{
    // Asigna este material en el inspector o desde otro script
    public Material windowMaterial;
    public Transform windowObject;  // Objeto que representa la ventana
    
    // Nombres de las propiedades en el Shader Graph
    private string windowPositionProperty = "_WindowPosition";
    private string windowScaleProperty = "_WindowScale";

    void Update()
    {
        if (windowObject != null && windowMaterial != null)
        {
            // Rastrear la posición y escala del objeto ventana
            Vector3 windowPosition = windowObject.position;
            Vector3 windowScale = windowObject.localScale;

            // Pasar la posición y la escala al material del Shader Graph
            windowMaterial.SetVector(windowPositionProperty, windowPosition);
            windowMaterial.SetVector(windowScaleProperty, windowScale);
        }
    }
}
