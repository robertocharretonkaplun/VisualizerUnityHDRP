using System.Windows.Forms;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

/// <summary>
/// Allows dragging materials and dropping them onto a model to apply the material. 
/// It also communicates with SelectTransformGizmo for proper interaction between both systems.
/// </summary>
public class MaterialDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Material materialToApply; // Material to apply upon drop
    public SelectTransformGizmo selectTransformGizmo; // Reference to the SelectTransformGizmo script
    private Camera mainCamera;
    private Image materialImage; // Image of the material to visually represent dragging in the canvas
    private RectTransform rectTransform; // RectTransform component controlling the position and size
    private Vector3 originalScale; // Original scale of the image
    private Vector3 originalPosition; // Original position of the image

    private void Awake()
    {
        // Find the SelectTransformGizmo in the scene
        selectTransformGizmo = FindObjectOfType<SelectTransformGizmo>();

        // Check if the selectTransformGizmo is correctly assigned
        Debug.Log(selectTransformGizmo != null ? "SelectTransformGizmo está asignado." : "SelectTransformGizmo no está asignado.");
    }
    /// <summary>
    /// Sets up the main camera and references to the `Image` and `RectTransform` components.
    /// </summary>
    private void Start()
    {
        mainCamera = Camera.main; // Get the main camera of the scene
        materialImage = GetComponent<Image>(); // Get the Image component from the GameObject
        rectTransform = GetComponent<RectTransform>(); // Get the `RectTransform` component used to modify position, scale, etc.

        // Store the original scale and position of the image before dragging
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.position; 
    }
    /// <summary>
    /// Method executed when the material begins to drag.
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Change the transparency of the image to indicate dragging
        if (materialImage != null)
        {
            Color color = materialImage.color;
            color.a = 0.7f; // Change the transparency to 70%
            materialImage.color = color;

            // Increase the size of the icon for a drag effect
            rectTransform.localScale = originalScale * 1.2f;
        }
    }
    /// <summary>
    /// During dragging, a visual effect can be shown if necessary.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        // Update the image's position to follow the mouse cursor during dragging
        rectTransform.position = eventData.position;
    }
    /// <summary>
    /// Method executed when dragging ends, and applies the material if possible.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        // Restores the transparency, size, and position of the image after the drag ends
        if (materialImage != null)
        {
            Color color = materialImage.color;
            color.a = 1.0f; // Restores transparency to 100%
            materialImage.color = color;

            // Restores the original size
            rectTransform.localScale = originalScale;

            // Restores the original position
            rectTransform.position = originalPosition;
        }
        // Casts a ray from the mouse position to detect if it hits an object
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Applies the material to the object's hierarchy or its MeshRenderers
            ApplyMaterialToHierarchyOrMeshRenderer(hit.transform, materialToApply);

            // Notifies SelectTransformGizmo that a material has been applied to the selected object
            if (selectTransformGizmo != null && selectTransformGizmo.GetCurrentSelection() == hit.transform)
            {
                // Calls a method in SelectTransformGizmo to handle material updates
                selectTransformGizmo.OnMaterialApplied(hit.transform, materialToApply);
            }
        }
    }
    /// <summary>
    /// Applies the material to all `MeshRenderers` in the parent within the object's `Transform` hierarchy.
    /// If the object has no children, it applies the material to all sub-indices of the `MeshRenderer`.
    /// Excludes objects with the "Floor" tag, as the material will not be applied to them.
    /// </summary>
    /// <param name="target">The target `Transform` object to which the material will be applied.</param>
    /// <param name="material">The material to be applied.</param>
    private void ApplyMaterialToHierarchyOrMeshRenderer(Transform target, Material material)
    {
        // Check if the object has the "Floor" tag before applying the material
        if (target.CompareTag("Floor"))
        {
            return; // Do not apply the material if the object has the "Floor" tag
        }

        // Apply the material to all MeshRenderers of the object and its children
        MeshRenderer[] renderers = target.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer renderer in renderers) 
        {
            // Check if the object is selected
            bool isSelected = selectTransformGizmo != null && selectTransformGizmo.GetCurrentSelection() == target;

            // Get the current materials
            Material[] currentMaterials = renderer.materials;

            // If the object is selected, replace all materials except the last one
            if (isSelected)
            {
                int materialCount = currentMaterials.Length;

                // If there is more than one material, keep the last one for the selection material
                if (materialCount > 1)
                {
                    // Create a new array of materials keeping the last material (selection)
                    Material[] newMaterials = new Material[materialCount];

                    // Replace all materials except the last one
                    for (int i = 0; i < materialCount - 1; i++)
                    {
                        newMaterials[i] = material;
                    }

                    // Keep the last material intact (selection material)
                    newMaterials[materialCount - 1] = currentMaterials[materialCount - 1];

                    // Apply the new materials to the MeshRenderer
                    renderer.materials = newMaterials;
                }
                else
                {
                    // If there's only one material, replace it entirely
                    renderer.material = material;
                }
            }
            else
            {
                // If the object is not selected, replace all materials normally
                for (int i = 0; i < currentMaterials.Length; i++)
                {
                    currentMaterials[i] = material;
                }
                renderer.materials = currentMaterials;
            }
        }
    }
    /// <summary>
    /// Replaces all sub-materials of a single `MeshRenderer` with a new material.
    /// </summary>
    /// <param name="renderer">The target `MeshRenderer`.</param>
    /// <param name="material">The new material to be applied to all sub-indices.</param>
    private void ApplyMaterialToAllSubMaterials(Renderer renderer, Material material)
    {
        Material[] materials = renderer.materials; // Get all sub-materials from the `MeshRenderer`

        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = material; // Replace each sub-material with the new material
        }
        renderer.materials = materials; // Assign the new materials
    }
}