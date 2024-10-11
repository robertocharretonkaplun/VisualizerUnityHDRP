using UnityEngine;

[ExecuteInEditMode]
public class PlaneC : MonoBehaviour
{
    public Transform planeTransform; // El plano que define la posición y rotación del corte
    public Material sliceMaterial; // El material con el shader de corte

    void Update()
    {
        if (planeTransform != null && sliceMaterial != null)
        {
            // Pasar la posición del plano al shader
            sliceMaterial.SetVector("_PlanePosition", planeTransform.position);
            
            // Pasar la normal del plano (orientación) al shader
            sliceMaterial.SetVector("_PlaneNormal", planeTransform.up);
        }
    }
}
