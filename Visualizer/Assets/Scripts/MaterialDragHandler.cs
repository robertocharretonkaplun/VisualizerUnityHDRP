using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Permite arrastrar materiales y soltarlos sobre un modelo para aplicar el material.
/// </summary>
public class MaterialDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Material materialToApply; // Material que se aplicará al soltar
    private Camera mainCamera;
    private Image materialImage; // Imagen del material para representar visualmente el arrastre
    private RectTransform rectTransform;
    private Vector3 originalScale; // Escala original de la imagen
    private Vector3 originalPosition; // Posición original de la imagen

    private void Start()
    {
        mainCamera = Camera.main;
        materialImage = GetComponent<Image>(); // Obtener el componente Image del GameObject
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.position; // Guardar la posición original
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

            // Aumenta el tamaño del icono para el efecto de arrastre
            rectTransform.localScale = originalScale * 1.2f;
        }
    }

    /// <summary>
    /// Durante el arrastre, se puede mostrar un efecto visual si es necesario.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        // Hace que el objeto siga al ratón
        rectTransform.position = eventData.position;
    }

    /// <summary>
    /// Al finalizar el arrastre, se aplica el nuevo material si se suelta sobre el objeto.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        // Restablece la transparencia, el tamaño y la posición de la imagen al finalizar el arrastre
        if (materialImage != null)
        {
            Color color = materialImage.color;
            color.a = 1.0f; // Restablece la transparencia al 100%
            materialImage.color = color;

            // Restablece el tamaño original
            rectTransform.localScale = originalScale;

            // Restablece la posición original
            rectTransform.position = originalPosition;
        }

        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(eventData.position);

        if (Physics.Raycast(ray, out hit) && materialToApply != null)
        {
            ApplyMaterialToHierarchy(hit.transform, materialToApply);
        }
    }

    /// <summary>
    /// Aplica el material a todos los Renderers en la jerarquía del objeto transform,
    /// excepto si el objeto tiene el tag "Floor".
    /// </summary>
    /// <param name="parent">Transform del objeto padre.</param>
    /// <param name="material">Material a aplicar.</param>
    private void ApplyMaterialToHierarchy(Transform parent, Material material)
    {
        // Verifica si el objeto tiene el tag "Floor" antes de aplicar el material
        if (parent.CompareTag("Floor"))
        {
            return; // No aplica el material si el objeto tiene el tag "Floor"
        }

        // Aplica el material al Renderer del objeto actual si existe
        Renderer parentRenderer = parent.GetComponent<Renderer>();
        if (parentRenderer != null)
        {
            parentRenderer.material = material;
        }

        // Recorre todos los hijos y aplica el material
        foreach (Transform child in parent)
        {
            ApplyMaterialToHierarchy(child, material);
        }
    }
}
