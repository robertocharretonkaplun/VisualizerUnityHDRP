using System.Windows.Forms;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

/// <summary>
/// Permite arrastrar materiales y soltarlos sobre un modelo para aplicar el material ademas se comunica con SelectTransformGizmo para un buen funcionamiento entre ambos.
/// </summary>
public class MaterialDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Material materialToApply; // Material que se aplicará al soltar
    public SelectTransformGizmo selectTransformGizmo; // Referencia al script SelectTransformGizmo
    private Camera mainCamera;
    private Image materialImage; // Imagen del material para representar visualmente el arrastre en el canvas
    private RectTransform rectTransform; // Componente de la imagen que controla la posición y el tamaño
    private Vector3 originalScale; // Escala original de la imagen
    private Vector3 originalPosition; // Posición original de la imagen

    private void Awake()
    {
        // Buscar el SelectTransformGizmo en la escena
        selectTransformGizmo = FindObjectOfType<SelectTransformGizmo>();

        // Verificar si el selectTransformGizmo está asignado correctamente
        Debug.Log(selectTransformGizmo != null ? "SelectTransformGizmo está asignado." : "SelectTransformGizmo no está asignado.");
    }

    /// <summary>
    /// Configura la cámara principal y las referencias a los componentes `Image` y `RectTransform`.
    /// </summary>
    private void Start()
    {
        mainCamera = Camera.main; // Obtiene la cámara principal de la escena
        materialImage = GetComponent<Image>(); // Obtener el componente Image del GameObject
        rectTransform = GetComponent<RectTransform>(); // Obtiene el componente `RectTransform`, que se utiliza para modificar posición, escala, etc.

        // Guarda la escala y la posición originales de la imagen antes del arrastre
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.position; 
    }

    /// <summary>
    /// Método que se ejecuta cuando se comienza a arrastrar el material.
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
        // Actualiza la posición de la imagen para que siga el cursor del ratón durante el arrastre
        rectTransform.position = eventData.position;
    }

    // <summary>
    /// Método que se ejecuta al finalizar el arrastre, y aplica el material si es posible.
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
        // Lanza un rayo desde la posición del ratón para detectar si golpea un objeto
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Aplica el material a la jerarquía del objeto o a sus MeshRenderers
            ApplyMaterialToHierarchyOrMeshRenderer(hit.transform, materialToApply);

            // Comunicar a SelectTransformGizmo que se ha aplicado un material al objeto seleccionado
            if (selectTransformGizmo != null && selectTransformGizmo.GetCurrentSelection() == hit.transform)
            {
                // Llama a un método en SelectTransformGizmo que maneje la actualización de materiales
                selectTransformGizmo.OnMaterialApplied(hit.transform, materialToApply);
            }
        }
    }

    /// <summary>
    /// Aplica el material a todos los `MeshRenderers` del padre dentro de la jerarquía del objeto `Transform`.
    /// Si el objeto no tiene hijos, aplica el material a todos los subíndices del `MeshRenderer`.
    /// Excluye los objetos que tienen el tag "Floor", para los cuales no se aplica el material.
    /// </summary>
    /// <param name="target">El objeto `Transform` objetivo al cual se le aplicará el material.</param>
    /// <param name="material">El material que se aplicará.</param>
    private void ApplyMaterialToHierarchyOrMeshRenderer(Transform target, Material material)
    {
        // Verifica si el objeto tiene el tag "Floor" antes de aplicar el material
        if (target.CompareTag("Floor"))
        {
            return; // No aplica el material si el objeto tiene el tag "Floor"
        }

        // Aplica el material a todos los MeshRenderers del objeto y sus hijos
        MeshRenderer[] renderers = target.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer renderer in renderers) 
        {
            // Verifica si el objeto está seleccionado
            bool isSelected = selectTransformGizmo != null && selectTransformGizmo.GetCurrentSelection() == target;

            // Obtener los materiales actuales
            Material[] currentMaterials = renderer.materials;

            // Si el objeto está seleccionado, se reemplazan todos los materiales menos el último
            if (isSelected)
            {
                int materialCount = currentMaterials.Length;

                // Si hay más de un material, se reserva el último para el material de selección
                if (materialCount > 1)
                {
                    // Crear un nuevo array de materiales que conserve el último material (de selección)
                    Material[] newMaterials = new Material[materialCount];

                    // Reemplazar todos los materiales excepto el último
                    for (int i = 0; i < materialCount - 1; i++)
                    {
                        newMaterials[i] = material;
                    }

                    // Mantener el último material intacto (el de selección)
                    newMaterials[materialCount - 1] = currentMaterials[materialCount - 1];

                    // Aplicar los nuevos materiales al MeshRenderer
                    renderer.materials = newMaterials;
                }
                else
                {
                    // Si solo hay un material, lo reemplaza por completo
                    renderer.material = material;
                }

                Debug.Log("Material aplicado al objeto seleccionado, manteniendo el material de selección.");
            }
            else
            {
                // Si el objeto no está seleccionado, reemplazar todos los materiales normalmente
                for (int i = 0; i < currentMaterials.Length; i++)
                {
                    currentMaterials[i] = material;
                }

                renderer.materials = currentMaterials;

                Debug.Log("Material aplicado al objeto no seleccionado.");
            }
        }
    }

    /// <summary>
    /// Reemplaza todos los submateriales de un solo `MeshRenderer` con un nuevo material.
    /// </summary>
    /// <param name="renderer">El `MeshRenderer` objetivo.</param>
    /// <param name="material">El nuevo material que se aplicará a todos los subíndices.</param>
    private void ApplyMaterialToAllSubMaterials(Renderer renderer, Material material)
    {
        Material[] materials = renderer.materials; // Obtiene todos los submateriales del `MeshRenderer`

        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = material; // Reemplazar cada submaterial con el nuevo material
        }
        renderer.materials = materials; // Asignar los nuevos materiales
    }
}
