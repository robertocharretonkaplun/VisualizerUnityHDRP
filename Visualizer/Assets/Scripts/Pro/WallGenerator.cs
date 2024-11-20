using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    public Transform leftHandle;
    public Transform rightHandle;

    public Vector2 holePosition = new Vector2(1f, 1f); // Starting position of the hole relative to the wall
    public Vector2 holeSize = new Vector2(2f, 2f); // Size of the hole (width and height)

    private MeshFilter meshFilter;
    private Mesh mesh;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;

        // Set handle positions to match the hole corners
        SetHandlePositions();
    }

    void SetHandlePositions()
    {
        // Assuming the wall's pivot is in the center, adjust for position and size
        Vector3 wallCenter = transform.position;
        Vector3 wallScale = transform.localScale;

        // Calculate the actual world position of the hole based on the wall's scale
        Vector3 leftCorner = wallCenter + new Vector3((holePosition.x - wallScale.x / 2) * wallScale.x, (holePosition.y - wallScale.y / 2) * wallScale.y, 0);
        Vector3 rightCorner = leftCorner + new Vector3(holeSize.x * wallScale.x, holeSize.y * wallScale.y, 0);

        leftHandle.position = leftCorner;
        rightHandle.position = rightCorner;
    }

    public void UpdateHole(Vector2 newHolePosition, Vector2 newHoleSize)
    {
        holePosition = newHolePosition;
        holeSize = newHoleSize;

        SetHandlePositions(); // Update handle positions when the hole changes
        ModifyWallMesh(); // Update the wall's mesh to reflect the new hole
    }

    // Modify the cube's mesh to create the hole
    void ModifyWallMesh()
    {
        Vector3[] vertices = mesh.vertices;
        Vector3 wallScale = transform.localScale;

        // Modify or remove vertices that fall within the hole's position and size
        // Simple example: you could modify vertices based on the hole area
        // Actual logic would depend on your mesh structure (e.g., scaled cube)

        // Apply the changes to the mesh
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh; // Update the mesh
    }
}
