using UnityEngine;

[ExecuteInEditMode]
public class SlicePlaneWithCubeMask : MonoBehaviour
{
    public Transform planeTransform; // El plano que define el corte
    public Transform cubeTransform; // El cubo que limita el corte
    public Material sliceMaterial; // El material con el shader de corte

    void Update()
    {
        if (planeTransform != null && cubeTransform != null && sliceMaterial != null)
        {
            // Actualizar posición y normal del plano
            sliceMaterial.SetVector("_PlanePosition", planeTransform.position);
            sliceMaterial.SetVector("_PlaneNormal", planeTransform.up);

            // Obtener los límites del cubo (mínimo y máximo en el espacio mundial)
            Vector3 cubeMin = cubeTransform.position - cubeTransform.localScale / 2;
            Vector3 cubeMax = cubeTransform.position + cubeTransform.localScale / 2;

            // Pasar los límites del cubo al shader
            sliceMaterial.SetVector("_CubeMin", cubeMin);
            sliceMaterial.SetVector("_CubeMax", cubeMax);
        }
    }
}
