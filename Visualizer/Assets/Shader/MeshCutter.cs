using UnityEngine;
using System.Collections.Generic; // Importar para usar List<T>

public class MeshCutter : MonoBehaviour
{
    public Transform cubeTransform; // El cubo que define el área de corte
    public MeshFilter meshFilter; // La malla del objeto que queremos cortar

    void Start()
    {
        CutMesh();
    }

    void CutMesh()
    {
        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();

        // Obtener los límites del cubo
        Vector3 cubeMin = cubeTransform.position - cubeTransform.localScale / 2;
        Vector3 cubeMax = cubeTransform.position + cubeTransform.localScale / 2;

        // Iterar sobre los vértices para ver si están dentro del cubo
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldVertex = meshFilter.transform.TransformPoint(vertices[i]);

            if (!(worldVertex.x >= cubeMin.x && worldVertex.x <= cubeMax.x &&
                  worldVertex.y >= cubeMin.y && worldVertex.y <= cubeMax.y &&
                  worldVertex.z >= cubeMin.z && worldVertex.z <= cubeMax.z))
            {
                // Si el vértice está fuera del cubo, lo añadimos a la nueva malla
                newVertices.Add(vertices[i]);
            }
        }

        // Actualizar la malla con los nuevos vértices
        mesh.vertices = newVertices.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
}
