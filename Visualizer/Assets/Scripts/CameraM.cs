using UnityEngine;
/// <summary>
/// Script de la cámara que controla el movimiento, rotación y zoom de la cámara.
/// </summary>
public class CameraM : MonoBehaviour
{
    public float moveSpeed = 10f; // Movement speed
    public float rotationSpeed = 100f; // Rotation speed
    public float zoomSpeed = 10f; // Zoom speed
    public float panSpeed = 10f; // Panning speed
    public float shiftMultiplier = 2f; // Speed multiplier when holding Shift
    private Vector3 lastMousePosition; // Last mouse position
    public float focusDistance = 5f; // Distance from the object when focusing

    //Reference to main camera
    public Camera MainCamera;

    private void Update()
    {
        // Change speed when holding Shift
        float currentMoveSpeed = moveSpeed;

        if(MainCamera.enabled)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                currentMoveSpeed *= shiftMultiplier;
            }

            // Camera movement
            if (Input.GetMouseButton(1))
            {
                float moveX = Input.GetAxis("Horizontal") * currentMoveSpeed * Time.deltaTime;
                float moveZ = Input.GetAxis("Vertical") * currentMoveSpeed * Time.deltaTime;
                float moveY = 0f;

                // Vertical movement
                if (Input.GetKey(KeyCode.Q))
                {
                    moveY = -currentMoveSpeed * Time.deltaTime;
                }
                else if (Input.GetKey(KeyCode.E))
                {
                    moveY = currentMoveSpeed * Time.deltaTime;
                }

                transform.Translate(new Vector3(moveX, moveY, moveZ));
            }

            // Camera rotation
            if (Input.GetMouseButton(1))
            {
                float rotationX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
                float rotationY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
                transform.Rotate(Vector3.up, rotationX, Space.World);
                transform.Rotate(Vector3.right, -rotationY, Space.Self);
            }

            // Camera zoom
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            transform.Translate(Vector3.forward * scroll * zoomSpeed, Space.Self);

            // Camera panning
            if (Input.GetMouseButton(2))
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;
                Vector3 translation = new Vector3(-delta.x, -delta.y, 0) * panSpeed * Time.deltaTime;
                transform.Translate(translation, Space.Self);
            }

            lastMousePosition = Input.mousePosition;
        }
        
    }
    // Se enfoca en un objeto y se mueve a una distancia determinada
    public void FocusOnObject(Transform target)
    {
        Vector3 focusPoint = target.position - target.forward * focusDistance;
        transform.position = focusPoint; // Immediately move to the focus point
        transform.LookAt(target); // Immediately rotate to face the target
    }

    public void SetTargetObject(Transform target)
    {
        FocusOnObject(target); // Call the focus method
    }
}
