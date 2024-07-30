using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using RuntimeHandle;
using RuntimeInspectorNamespace;

public class SelectTransformGizmo : MonoBehaviour
{
    public Material selectionMaterial;
    public RuntimeHierarchy runtimeHierarchy; // Reference to the RuntimeHierarchy

    private Dictionary<Transform, Material[]> originalMaterialsSelection = new Dictionary<Transform, Material[]>();
    private Transform selection;
    private RaycastHit raycastHit;
    private RaycastHit raycastHitHandle;
    private GameObject runtimeTransformGameObj;
    private RuntimeTransformHandle runtimeTransformHandle;
    private int runtimeTransformLayer = 6;
    private int runtimeTransformLayerMask;

    public event Action<Transform> OnSelectionChanged;

    private void Start()
    {
        runtimeTransformGameObj = new GameObject();
        runtimeTransformHandle = runtimeTransformGameObj.AddComponent<RuntimeTransformHandle>();
        runtimeTransformGameObj.layer = runtimeTransformLayer;
        runtimeTransformLayerMask = 1 << runtimeTransformLayer; //Layer number represented by a single bit in the 32-bit integer using bit shift
        runtimeTransformHandle.type = HandleType.POSITION;
        runtimeTransformHandle.autoScale = true;
        runtimeTransformHandle.autoScaleFactor = 1.0f;
        runtimeTransformGameObj.SetActive(false);

        runtimeTransformGameObj.tag = "DeactivatableObject"; // Asigna la etiqueta 'DeactivatableObject' al GameObject creado

        OnSelectionChanged += HandleSelectionChanged; // Subscribe to the event
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Selection
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            ApplyLayerToChildren(runtimeTransformGameObj);
            if (Physics.Raycast(ray, out raycastHit))
            {
                if (Physics.Raycast(ray, out raycastHitHandle, Mathf.Infinity, runtimeTransformLayerMask)) //Raycast towards runtime transform handle only
                {
                    // Do nothing if hit the runtime transform handle
                }
                else if (raycastHit.transform.CompareTag("Selectable")) // Check if the object is selectable
                {
                    if (selection != null)
                    {
                        SetMaterials(selection, originalMaterialsSelection, true); //Restore the original material
                    }
                    selection = raycastHit.transform;
                    MeshRenderer[] selectionRenderers = GetMeshRenderers(selection);
                    foreach (MeshRenderer renderer in selectionRenderers)
                    {
                        if (!ArrayContainsMaterial(renderer.materials, selectionMaterial)) //Add the selection material
                        {
                            originalMaterialsSelection[renderer.transform] = renderer.materials; //Save the original material
                            Material[] newMaterials = new Material[renderer.materials.Length + 1]; //Create a new material array
                            renderer.materials.CopyTo(newMaterials, 0); //Copy the original materials to the new array
                            newMaterials[newMaterials.Length - 1] = selectionMaterial; //Add the selection material to the new array
                            renderer.materials = newMaterials; //Assign the new material array to the selected object
                        }
                    }
                    runtimeTransformHandle.target = selection; //Assign the selected object to the runtime transform handle
                    runtimeTransformGameObj.SetActive(true); //Activate the runtime transform handle
                    OnSelectionChanged?.Invoke(selection); // Notify selection change
                }
                else
                {
                    if (selection)
                    {
                        SetMaterials(selection, originalMaterialsSelection, true); //Restore the original material
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
                    SetMaterials(selection, originalMaterialsSelection, true); //Restore the original material
                    selection = null;
                    runtimeTransformGameObj.SetActive(false); //Deactivate the runtime transform handle
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

    public Transform GetCurrentSelection()
    {
        return selection;
    }

    private void ApplyLayerToChildren(GameObject parentGameObj)
    {
        foreach (Transform transform1 in parentGameObj.transform)
        {
            int layer = parentGameObj.layer;
            transform1.gameObject.layer = layer;
            foreach (Transform transform2 in transform1)
            {
                transform2.gameObject.layer = layer;
                foreach (Transform transform3 in transform2)
                {
                    transform3.gameObject.layer = layer;
                    foreach (Transform transform4 in transform3)
                    {
                        transform4.gameObject.layer = layer;
                        foreach (Transform transform5 in transform4)
                        {
                            transform5.gameObject.layer = layer;
                        }
                    }
                }
            }
        }
    }

    private bool ArrayContainsMaterial(Material[] materials, Material material) //Check if a material already exists in an array of materials
    {
        foreach (var mat in materials)
        {
            if (mat == material)
            {
                return true;
            }
        }
        return false;
    }

    private void HandleSelectionChanged(Transform newSelection)
    {
        if (runtimeHierarchy != null)
        {
            runtimeHierarchy.Select(newSelection, RuntimeHierarchy.SelectOptions.FocusOnSelection); // Update RuntimeHierarchy selection
        }
    }

    private MeshRenderer[] GetMeshRenderers(Transform transform)
    {
        List<MeshRenderer> renderers = new List<MeshRenderer>(transform.GetComponentsInChildren<MeshRenderer>());
        return renderers.ToArray();
    }

    private void SetMaterials(Transform transform, Dictionary<Transform, Material[]> materialsDict, bool isSelection)
    {
        MeshRenderer[] renderers = GetMeshRenderers(transform);
        foreach (MeshRenderer renderer in renderers)
        {
            if (materialsDict.TryGetValue(renderer.transform, out Material[] originalMaterials))
            {
                renderer.materials = originalMaterials;
                if (isSelection)
                {
                    materialsDict.Remove(renderer.transform);
                }
            }
        }
    }

    public GameObject GetRuntimeTransformGameObject()
    {
        return runtimeTransformGameObj; //Crea un GameObj Dinamico runtimeTransformGameObj y le asigna un RuntimeTransformHandle
    }
}

