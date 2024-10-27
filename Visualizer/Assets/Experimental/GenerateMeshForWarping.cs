using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GenerateMeshForWarping : MonoBehaviour
{
    public float minDistance;
    public List<Transform> trackedObjects; // Lista de objetos de corte
    public MeshFilter filter;

    public void Remesh()
    {
        filter.mesh = GenerateMeshWithHoles();
    }

    Mesh mesh;
    Vector3[] vertices;
    Vector3[] normals;
    Vector2[] uvs;
    int[] triangles;
    bool[] trianglesDisabled;
    List<int>[] trisWithVertex;

    Vector3[] origvertices;
    Vector3[] orignormals;
    Vector2[] origuvs;
    int[] origtriangles;

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

    void Update()
    {
        Remesh();
    }
}
