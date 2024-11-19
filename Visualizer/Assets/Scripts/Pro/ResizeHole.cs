using UnityEngine;

public class ResizeHole : MonoBehaviour
{
    public WallGenerator wall; // Reference to the WallGenerator script
    public bool isLeftCorner = true; // Identify if it's the left or right corner

    private Vector3 screenPoint;
    private Vector3 offset;

    void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;

        // Update the hole size or position depending on which handle is dragged
        if (isLeftCorner)
        {
            Vector2 newHolePosition = new Vector2(curPosition.x - wall.transform.position.x, curPosition.y - wall.transform.position.y);
            wall.UpdateHole(newHolePosition, wall.holeSize);
        }
        else
        {
            Vector2 newHoleSize = new Vector2(curPosition.x - wall.holePosition.x, curPosition.y - wall.holePosition.y);
            wall.UpdateHole(wall.holePosition, newHoleSize);
        }
    }
}
