using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Permite arrastrar materiales y soltarlos sobre un modelo para aplicar el material.
/// </summary>
public class MaterialDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Material materialToApply; // Material que se aplicar� al soltar
    private Camera mainCamera;
    private Image materialImage; // Imagen del material para representar visualmente el arrastre
    private RectTransform rectTransform;
    private Vector3 originalScale; // Escala original de la imagen
    private Vector3 originalPosition; // Posici�n original de la imagen

    private void Start()
    {
        mainCamera = Camera.main;
        materialImage = GetComponent<Image>(); // Obtener el componente Image del GameObject
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.position; // Guardar la posici�n original
    }

    /// <summary>
    /// Inicia el arrastre.
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Cambia la transparencia de la imagen para indicar el arrastre
        if (materialImage != null)
        {
            Color color = materialImage.color;
            color.a = 0.7f; // Cambia la transparencia a 70%
            materialImage.color = color;

            // Aumenta el tama�o del icono para el efecto de arrastre
            rectTransform.localScale = originalScale * 1.2f;
        }
    }

    /// <summary>
    /// Durante el arrastre, se puede mostrar un efecto visual si es necesario.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        // Hace que el objeto siga al rat�n
        rectTransform.position = eventData.position;
    }

    /// <summary>
    /// Al finalizar el arrastre, se aplica el nuevo material si se suelta sobre el objeto.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        // Restablece la transparencia, el tama�o y la posici�n de la imagen al finalizar el arrastre
        if (materialImage != null)
        {
            Color color = materialImage.color;
            color.a = 1.0f; // Restablece la transparencia al 100%
            materialImage.color = color;

            // Restablece el tama�o original
            rectTransform.localScale = originalScale;

            // Restablece la posici�n original
            rectTransform.position = originalPosition;
        }

        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(eventData.position);

        if (Physics.Raycast(ray, out hit))
        {
            Renderer targetRenderer = hit.transform.GetComponent<Renderer>();
            if (targetRenderer != null && materialToApply != null)
            {
                // Cambia el material del objeto sobre el cual se solt�
                targetRenderer.material = materialToApply;
            }
        }
    }
}
