using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GenerateMeshForWarping : MonoBehaviour
{
    public float minDistance;
    public List<Transform> trackedObjects; // Lista de objetos de corte
    public MeshFilter filter;

    private Mesh mesh;
    private Vector3[] vertices;
    private Vector3[] normals;
    private Vector2[] uvs;
    private int[] triangles;
    private bool[] trianglesDisabled;
    private List<int>[] trisWithVertex;

    private Vector3[] origvertices;
    private Vector3[] orignormals;
    private Vector2[] origuvs;
    private int[] origtriangles;

    private List<Vector3> previousTrackedPositions = new List<Vector3>(); // Lista para guardar las posiciones previas de los objetos de corte

    void Start()
    {
        mesh = new Mesh();
        filter = GetComponent<MeshFilter>();
        orignormals = filter.mesh.normals;
        origvertices = filter.mesh.vertices;
        origuvs = filter.mesh.uv;
        origtriangles = filter.mesh.triangles;

        vertices = new Vector3[origvertices.Length];
        normals = new Vector3[orignormals.Length];
        uvs = new Vector2[origuvs.Length];
        triangles = new int[origtriangles.Length];
        trianglesDisabled = new bool[origtriangles.Length / 3]; // Un bool por triángulo

        orignormals.CopyTo(normals, 0);
        origvertices.CopyTo(vertices, 0);
        origtriangles.CopyTo(triangles, 0);
        origuvs.CopyTo(uvs, 0);

        trisWithVertex = new List<int>[origvertices.Length];
        for (int i = 0; i < origvertices.Length; ++i)
        {
            trisWithVertex[i] = new List<int>();
        }
        for (int i = 0; i < origtriangles.Length; i += 3)
        {
            trisWithVertex[origtriangles[i]].Add(i);
            trisWithVertex[origtriangles[i + 1]].Add(i);
            trisWithVertex[origtriangles[i + 2]].Add(i);
        }

        foreach (var trackedObject in trackedObjects)
        {
            previousTrackedPositions.Add(trackedObject.position); // Inicializar posiciones previas
        }

        Remesh(); // Primera generación de la malla
    }

    void Update()
    {
        bool positionChanged = false;

        // Revisar si alguno de los objetos de corte cambió de posición
        for (int i = 0; i < trackedObjects.Count; i++)
        {
            if (trackedObjects[i].position != previousTrackedPositions[i])
            {
                positionChanged = true;
                previousTrackedPositions[i] = trackedObjects[i].position; // Actualizar posición previa
            }
        }

        if (positionChanged)
        {
            Remesh(); // Remesh solo si la posición ha cambiado
        }
    }

    public void Remesh()
    {
        filter.mesh = GenerateMeshWithHoles();
    }

    Mesh GenerateMeshWithHoles()
    {
        foreach (Transform trackedObject in trackedObjects)
        {
            Bounds cutBounds = trackedObject.GetComponent<Collider>().bounds;

            for (int i = 0; i < origvertices.Length; ++i)
            {
                Vector3 v = transform.localToWorldMatrix.MultiplyPoint3x4(origvertices[i]);

                if (cutBounds.Contains(v))
                {
                    foreach (int triIndex in trisWithVertex[i])
                    {
                        trianglesDisabled[triIndex / 3] = true;
                    }
                }
            }
        }

        List<int> newTriangles = new List<int>();
        for (int i = 0; i < triangles.Length; i += 3)
        {
            if (!trianglesDisabled[i / 3])
            {
                newTriangles.Add(triangles[i]);
                newTriangles.Add(triangles[i + 1]);
                newTriangles.Add(triangles[i + 2]);
            }
        }

        mesh.SetVertices(vertices.ToList());
        mesh.SetNormals(normals.ToList());
        mesh.SetUVs(0, uvs.ToList());
        mesh.SetTriangles(newTriangles, 0);

        for (int i = 0; i < trianglesDisabled.Length; ++i)
            trianglesDisabled[i] = false;

        return mesh;
    }
}
