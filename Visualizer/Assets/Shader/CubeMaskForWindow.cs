
using UnityEngine;

[ExecuteInEditMode]
public class CubeMaskForWindow : MonoBehaviour
{
    public Transform cubeTransform; // El cubo que define el área de corte (ventana)
    public Material wallMaterial; // El material con el shader de recorte

    void Update()
    {
        if (cubeTransform != null && wallMaterial != null)
        {
            // Calcular los límites mínimos y máximos del cubo en espacio mundial
            Vector3 cubeMin = cubeTransform.position - (cubeTransform.localScale / 2);
            Vector3 cubeMax = cubeTransform.position + (cubeTransform.localScale / 2);

            // Pasar los límites del cubo al shader
            wallMaterial.SetVector("_CubeMin", cubeMin);
            wallMaterial.SetVector("_CubeMax", cubeMax);
        }
    }
}
