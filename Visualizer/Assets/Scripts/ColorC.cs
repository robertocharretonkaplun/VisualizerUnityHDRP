using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Cambia el color de un botón de UI cuando un objeto está activo.
/// </summary>
public class ColorC : MonoBehaviour
{
    // Referencia al objeto que queremos observar
    public GameObject targetObject;

    // Referencia al botón de UI
    public Button uiButton;

    // Color activo del botón
    public Color activeColor = Color.red;

    // Color original del botón
    private Color originalColor;

    // Imagen del botón
    private Image buttonImage;

    void Start()
    {
        if (uiButton != null)
        {
            // Obtener la imagen del botón
            buttonImage = uiButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                // Guardar el color original del botón
                originalColor = buttonImage.color;
            }
        }
    }

    void Update()
    {
        if (targetObject != null && buttonImage != null)
        {
            // Comprobar si el objeto está activo
            if (targetObject.activeSelf)
            {
                // Cambiar el color del botón al color activo
                buttonImage.color = activeColor;
            }
            else
            {
                // Restaurar el color original del botón
                buttonImage.color = originalColor;
            }
        }
    }
}
