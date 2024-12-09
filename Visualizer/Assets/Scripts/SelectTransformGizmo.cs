//MIT License
//Copyright (c) 2023 DA LAB (https://www.youtube.com/@DA-LAB)
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using RuntimeHandle;
using RuntimeInspectorNamespace;
/// <summary>
/// Class responsible for managing the selection of objects in the scene
/// and applying a temporary selection material when an object is selected.
/// It also allows manipulation of the selected object through a transformation gizmo
/// and communicates with the SaveSystem and MaterialDragHandler.
/// </summary>
public class SelectTransformGizmo : MonoBehaviour
{
    public Material selectionMaterial; // Material applied to the object when it is selected.
    public RuntimeHierarchy runtimeHierarchy; // Reference to the RuntimeHierarchy
    public CameraM sceneCamera; // Reference to the camera script
    public MaterialDragHandler materialDragHandler; // Reference to the MaterialDragHandler

    // Dictionary storing the original materials of the selected objects for restoration.
    private Dictionary<Transform, Material[]> originalMaterialsSelection = new Dictionary<Transform, Material[]>(); 

    private Transform selection; // Currently selected object.
    private RaycastHit raycastHit; // Stores information about the ray that collides with objects in the scene.
    private RaycastHit raycastHitHandle; // Stores information about the ray that collides with the transformation gizmo.
    private GameObject runtimeTransformGameObj; // The object that contains the runtime transformation gizmo.
    private RuntimeTransformHandle runtimeTransformHandle; // The script that manages the transformation gizmo.
    private int runtimeTransformLayer = 6; // The layer of interactable objects for the gizmo.
    private int runtimeTransformLayerMask; // Layer mask to filter interactable objects.
    private bool focusOnSelectionRequested; // Indicates if there is a request to focus the camera on the selected object.

    public event Action<Transform> OnSelectionChanged; // Event triggered when the selection changes.

    /// <summary>
    /// Method that executes when the component starts. It finds and assigns the MaterialDragHandler script
    /// in the scene.
    /// </summary>
    private void Awake()
    {
        // Finds and assigns the MaterialDragHandler script in the scene.
        materialDragHandler = FindObjectOfType<MaterialDragHandler>();
    }
    /// <summary>
    /// Method that runs when the object is activated. Initializes the transformation gizmo
    /// and sets up the necessary layers and events.
    /// </summary>
    private void Start()
    {
        // Create a new GameObject to handle the runtime transformation gizmo.
        runtimeTransformGameObj = new GameObject();
        runtimeTransformHandle = runtimeTransformGameObj.AddComponent<RuntimeTransformHandle>();
        runtimeTransformGameObj.layer = runtimeTransformLayer;

        // Set up the layer mask to filter objects interactable by the gizmo.
        runtimeTransformLayerMask = 1 << runtimeTransformLayer; // Layer number represented by a single bit in the 32-bit integer using bit shift

        // Set the gizmo's transformation type (in this case, position only).
        runtimeTransformHandle.type = HandleType.POSITION;
        runtimeTransformHandle.autoScale = true;
        runtimeTransformHandle.autoScaleFactor = 1.0f;

        // Deactivate the gizmo until it is needed.
        runtimeTransformGameObj.SetActive(false);

        // Assign a tag to the gizmo so it can be deactivated later.
        runtimeTransformGameObj.tag = "DeactivatableObject";

        // Subscribe the `HandleSelectionChanged` method to the OnSelectionChanged event.
        OnSelectionChanged += HandleSelectionChanged;

        // If a runtime hierarchy is available, subscribe to selection changes.
        if (runtimeHierarchy != null)
        {
            // Subscribe a method to handle selection changes in the hierarchy (if available).
            // runtimeHierarchy.OnSelectionChanged += HandleHierarchySelectionChanged;
        }
    }
    /// <summary>
    /// Checks and logs if a material has been applied to the current object in the `MeshRenderer`.
    /// </summary>
    /// <param name="renderer">The `MeshRenderer` of the object.</param>
    private void CheckAndLogMaterialApplication(MeshRenderer renderer)
    {
        // Checks if the `MeshRenderer` has materials and if it contains a specific material.
        if (renderer.materials.Length > 0 && ArrayContainsMaterial(renderer.materials, selectionMaterial))
        {
            Debug.Log("Se ha aplicado un nuevo material desde D&D");
        }
    }
    /// <summary>
    /// Method executed every frame to handle object selection, activation of the transformation gizmo,
    /// and application of selection materials. It also allows the deletion of the selected object.
    /// </summary>
    private void Update()
    {
        // Object selection and camera focus
        if (Input.GetKeyDown(KeyCode.F) && focusOnSelectionRequested)
        {
            if (sceneCamera != null && selection != null)
            {
                sceneCamera.SetTargetObject(selection); // Focus the camera on the selected object
            }
            focusOnSelectionRequested = false; // Reset the flag
        }
        // Object selection with left mouse click
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            ApplyLayerToChildren(runtimeTransformGameObj); // Ensure the transform gizmo is in the correct layer
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit)) // Cast a ray from the camera through the mouse position
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHitHandle, Mathf.Infinity, runtimeTransformLayerMask)) // Raycast towards runtime transform handle only
                {
                    // Do nothing if the ray collides with the transformation gizmo
                }
                else if (raycastHit.transform.CompareTag("Selectable")) // Check if the hit object is tagged as "Selectable"
                {
                    if (selection != null)
                    {
                        SetMaterials(selection, originalMaterialsSelection, true); // Restore the original material
                    }
                    selection = raycastHit.transform;
                    MeshRenderer[] selectionRenderers = GetMeshRenderers(selection);
                    foreach (MeshRenderer renderer in selectionRenderers)
                    {
                        if (!ArrayContainsMaterial(renderer.materials, selectionMaterial)) // Add the selection material
                        {
                            originalMaterialsSelection[renderer.transform] = renderer.materials; // Save the original materials
                            ApplyMaterial(renderer, selectionMaterial); // Apply the selection material
                            CheckAndLogMaterialApplication(renderer); // Check and log the material application
                        }
                    }
                    runtimeTransformHandle.target = selection; // Assign the selected object to the runtime transform handle
                    runtimeTransformGameObj.SetActive(true); // Activate the runtime transform handle
                    OnSelectionChanged?.Invoke(selection); // Notify selection change
                    focusOnSelectionRequested = true; // Set flag to request focus
                }
                else
                {
                    if (selection)
                    {
                        SetMaterials(selection, originalMaterialsSelection, true); // Restore the original material
                        selection = null;
                        runtimeTransformGameObj.SetActive(false);
                        OnSelectionChanged?.Invoke(null); // Notify selection change
                    }
                }
            }
            else
            {
                if (selection)
                {
                    SetMaterials(selection, originalMaterialsSelection, true); // Restore the original material
                    selection = null;
                    runtimeTransformGameObj.SetActive(false); // Deactivate the runtime transform handle
                    OnSelectionChanged?.Invoke(null); // Notify selection change
                }
            }
        }

        // Function to delete the selected object
        if (Input.GetKeyDown(KeyCode.Delete) && selection != null)
        {
            Destroy(selection.gameObject);
            selection = null;
            runtimeTransformGameObj.SetActive(false);
            OnSelectionChanged?.Invoke(null); // Notify selection change
        }

        // Hot Keys for move, rotate, scale, local and Global/World transform
        if (runtimeTransformGameObj.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                runtimeTransformHandle.type = HandleType.POSITION;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                runtimeTransformHandle.type = HandleType.ROTATION;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                runtimeTransformHandle.type = HandleType.SCALE;
            }
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (Input.GetKeyDown(KeyCode.G))
                {
                    runtimeTransformHandle.space = HandleSpace.WORLD;
                }
                if (Input.GetKeyDown(KeyCode.L))
                {
                    runtimeTransformHandle.space = HandleSpace.LOCAL;
                }
            }
        }
    }
    /// <summary>
    /// Gets the currently selected object.
    /// </summary>
    /// <returns>The selected object.</returns>
    public Transform GetCurrentSelection()
    {
        return selection; // Returns the selected object
    }
    /// <summary>
    /// Applies a material to the renderer of an object.
    /// </summary>
    /// <param name="renderer">The MeshRenderer to which the material will be applied.</param>
    /// <param name="material">The material to be applied.</param>
    private void ApplyMaterial(MeshRenderer renderer, Material material)
    {
        Material[] newMaterials = new Material[renderer.materials.Length + 1]; // Create a new array of materials
        renderer.materials.CopyTo(newMaterials, 0); // Copy the original materials to the new array
        newMaterials[newMaterials.Length - 1] = material; // Add the new material to the end of the array
        renderer.materials = newMaterials; // Apply the new array of materials
    }
    /// <summary>
    /// Handles the selection change in the hierarchy.
    /// </summary>
    /// <param name="newSelection">The new selected object.</param>
    private void HandleSelectionChanged(Transform newSelection)
    {
        if (runtimeHierarchy != null)
        {
            runtimeHierarchy.Select(newSelection, RuntimeHierarchy.SelectOptions.FocusOnSelection); // Update the selection in RuntimeHierarchy
        }
        if (sceneCamera != null)
        {
            focusOnSelectionRequested = true; // Set flag to request focus on the next F key press
        }
    }
    /// <summary>
    /// This method is invoked to apply a new material to a selected object.
    /// If the object in question is the currently selected one, it updates its material
    /// in the original materials dictionary.
    /// 
    /// <param name="target">The `Transform` object to which the new material will be applied.</param>
    /// <param name="newMaterial">The new material to be applied to the object.</param>
    /// </summary>
    public void OnMaterialApplied(Transform target, Material newMaterial)
    {
        // Check if the target object is the currently selected one
        if (target == selection)
        {
            // Get all MeshRenderers from the selected object and its children
            MeshRenderer[] renderers = target.GetComponentsInChildren<MeshRenderer>();

            // Iterate through all the MeshRenderers and update the material in the dictionary
            foreach (MeshRenderer renderer in renderers)
            {
                // Check if this renderer already has original materials stored
                if (originalMaterialsSelection.ContainsKey(renderer.transform))
                {
                    // Update the original materials array for this renderer
                    Material[] originalMaterials = originalMaterialsSelection[renderer.transform];

                    // Create a new materials array if the renderer has more than one material
                    if (renderer.materials.Length > 1)
                    {
                        // Create a new materials array with reduced size
                        Material[] newMaterialsArray = new Material[renderer.materials.Length - 1];

                        // Copy all materials except the last one
                        for (int i = 0; i < newMaterialsArray.Length; i++)
                        {
                            newMaterialsArray[i] = renderer.materials[i];
                        }

                        // Assign the new array to the MeshRenderer
                        renderer.materials = newMaterialsArray;

                        // Update the original materials array in the dictionary
                        originalMaterialsSelection[renderer.transform] = newMaterialsArray; // Update the dictionary with the new array
                    }
                    else
                    {
                        Debug.LogWarning("El MeshRenderer tiene sólo un material, no se puede eliminar el último.");
                    }
                }
                else
                {
                    // If the renderer does not have stored materials, add it to the dictionary
                    originalMaterialsSelection.Add(renderer.transform, new Material[renderer.materials.Length]);

                    // Initialize with the newly applied material
                    originalMaterialsSelection[renderer.transform][0] = newMaterial; // Assign the new material
                }
            }
        }
    }
    /// <summary>
    /// Handles the change of selection in the hierarchy.
    /// </summary>
    /// <param name="newSelection">The newly selected object.</param>
    private void HandleHierarchySelectionChanged(Transform newSelection)
    {
        HandleSelectionChanged(newSelection); // Handle the hierarchy selection change
    }
    /// <summary>
    /// Applies the layer of the parent GameObject to all its children.
    /// </summary>
    /// <param name="parentGameObj">The parent GameObject whose layer will be applied to the children.</param>
    private void ApplyLayerToChildren(GameObject parentGameObj)
    {
        foreach (Transform transform1 in parentGameObj.transform)
        {
            int layer = parentGameObj.layer; // Get the layer of the parent GameObject
            transform1.gameObject.layer = layer; // Apply the layer to the child
            foreach (Transform transform2 in transform1)
            {
                transform2.gameObject.layer = layer; // Apply the layer to the grandchildren
                foreach (Transform transform3 in transform2)
                {
                    transform3.gameObject.layer = layer; // Apply the layer to the great-grandchildren
                    foreach (Transform transform4 in transform3)
                    {
                        transform4.gameObject.layer = layer; // Apply the layer to the great-great-grandchildren
                        foreach (Transform transform5 in transform4)
                        {
                            transform5.gameObject.layer = layer; //Apply the layer to I don't know any more terms
                        }
                    }
                }
            }
        }
    }
    /// <summary>
    /// Checks if a material already exists in an array of materials.
    /// </summary>
    /// <param name="materials">Array of materials to check.</param>
    /// <param name="material">The material being searched for.</param>
    /// <returns>True if the material exists in the array, otherwise False.</returns>
    private bool ArrayContainsMaterial(Material[] materials, Material material) // Check if a material already exists in an array of materials
    {
        foreach (var mat in materials)
        {
            if (mat == material)
            {
                return true; // The material exists
            }
        }
        return false; // The material does not exist
    }
    /// <summary>
    /// Retrieves all MeshRenderers from an object and its children.
    /// </summary>
    /// <param name="transform">The object from which the MeshRenderers will be retrieved.</param>
    /// <returns>Array of MeshRenderers found.</returns>
    private MeshRenderer[] GetMeshRenderers(Transform transform)
    {
        List<MeshRenderer> renderers = new List<MeshRenderer>(transform.GetComponentsInChildren<MeshRenderer>());// Get all MeshRenderers from the object and its children
        return renderers.ToArray(); // Return as an array
    }
    /// <summary>
    /// Restores the original materials on an object and its children.
    /// </summary>
    /// <param name="transform">The object where the materials will be restored.</param>
    /// <param name="materialsDict">Dictionary containing the original materials.</param>
    /// <param name="isSelection">Indicates if the restoration is part of a selection process.</param>
    private void SetMaterials(Transform transform, Dictionary<Transform, Material[]> materialsDict, bool isSelection)
    {
        MeshRenderer[] renderers = GetMeshRenderers(transform); // Get the MeshRenderers of the object
        foreach (MeshRenderer renderer in renderers)
        {
            if (materialsDict.TryGetValue(renderer.transform, out Material[] originalMaterials))
            {
                renderer.materials = originalMaterials; // Restore the original materials
                if (isSelection)
                {
                    materialsDict.Remove(renderer.transform); // Remove the object from the dictionary if it's part of the selection
                }
            }
        }
    }
    /// <summary>
    /// Retrieves the GameObject of the dynamic transform.
    /// </summary>
    /// <returns>The dynamic GameObject that contains the RuntimeTransformHandle.</returns>
    public GameObject GetRuntimeTransformGameObject()
    {
        return runtimeTransformGameObj; // Returns the dynamic GameObject 'runtimeTransformGameObj' which has a RuntimeTransformHandle assigned, providing functionalities to manipulate the selected object's transformations
    }
}
