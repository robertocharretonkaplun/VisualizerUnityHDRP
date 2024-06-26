using UnityEngine;

public class CameraM : MonoBehaviour
{
    public float moveSpeed = 10f; // Velocidad de movimiento
    public float rotationSpeed = 100f; // Velocidad de rotación
    public float zoomSpeed = 10f; // Velocidad de zoom
    public float panSpeed = 10f; // Velocidad de paneo
    public float shiftMultiplier = 2f; // Multiplicador de velocidad al mantener Shift
    private Vector3 lastMousePosition; // Última posición del mouse

    void Update()
    {
        // Cambio de velocidad al mantener Shift
        float currentMoveSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentMoveSpeed *= shiftMultiplier;
        }

        // Traslacion de la cámara (Movement)
        if (Input.GetMouseButton(1))
        {
            float moveX = Input.GetAxis("Horizontal") * currentMoveSpeed * Time.deltaTime;
            float moveZ = Input.GetAxis("Vertical") * currentMoveSpeed * Time.deltaTime;
            float moveY = 0f;

            // Movimiento vertical y horizontal 
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

        // Rotación de la cámara (Rotation)
        if (Input.GetMouseButton(1))
        {
            float rotationX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float rotationY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, rotationX, Space.World);
            transform.Rotate(Vector3.right, -rotationY, Space.Self);
        }

        // Zoom de la cámara (Zoom)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        transform.Translate(Vector3.forward * scroll * zoomSpeed, Space.Self);

        // Paneo de la cámara (Panning)
        if (Input.GetMouseButton(2))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            Vector3 translation = new Vector3(-delta.x, -delta.y, 0) * panSpeed * Time.deltaTime;
            transform.Translate(translation, Space.Self);
        }

        lastMousePosition = Input.mousePosition;
    }
}
