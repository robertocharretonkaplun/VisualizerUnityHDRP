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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using RuntimeHandle;

public class SelectTransformGizmo : MonoBehaviour
{
    public Material highlightMaterial;
    public Material selectionMaterial;

    private Material[] originalMaterialsHighlight;
    private Material[] originalMaterialsSelection;
    private Transform highlight;
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
    }

    void Update()
    {
        // Highlight
        if (highlight != null)
        {
            highlight.GetComponent<MeshRenderer>().materials = originalMaterialsHighlight;
            highlight = null;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit)) //Make sure you have EventSystem in the hierarchy before using EventSystem
        {
            highlight = raycastHit.transform;
            if (highlight.CompareTag("Selectable") && highlight != selection)
            {
                MeshRenderer highlightRenderer = highlight.GetComponent<MeshRenderer>();
                if (!ArrayContainsMaterial(highlightRenderer.materials, highlightMaterial))
                {
                    originalMaterialsHighlight = highlightRenderer.materials;
                    Material[] newMaterials = new Material[originalMaterialsHighlight.Length + 1];
                    originalMaterialsHighlight.CopyTo(newMaterials, 0);
                    newMaterials[newMaterials.Length - 1] = highlightMaterial;
                    highlightRenderer.materials = newMaterials;
                }
            }
            else
            {
                highlight = null;
            }
        }

        // Selection
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            ApplyLayerToChildren(runtimeTransformGameObj);
            if (Physics.Raycast(ray, out raycastHit))
            {
                if (Physics.Raycast(ray, out raycastHitHandle, Mathf.Infinity, runtimeTransformLayerMask)) //Raycast towards runtime transform handle only
                {
                }
                else if (highlight) //Select the highlighted object
                {
                    if (selection != null)
                    {
                        selection.GetComponent<MeshRenderer>().materials = originalMaterialsSelection; //Restore the original material
                    }
                    selection = raycastHit.transform;
                    MeshRenderer selectionRenderer = selection.GetComponent<MeshRenderer>();
                    if (!ArrayContainsMaterial(selectionRenderer.materials, selectionMaterial)) //Add the selection material
                    {
                        originalMaterialsSelection = originalMaterialsHighlight; //Save the original material
                        Material[] newMaterials = new Material[originalMaterialsSelection.Length + 1]; //Create a new material array
                        originalMaterialsSelection.CopyTo(newMaterials, 0); //Copy the original materials to the new array
                        newMaterials[newMaterials.Length - 1] = selectionMaterial; //Add the selection material to the new array
                        selectionRenderer.materials = newMaterials; //Assign the new material array to the selected object
                        runtimeTransformHandle.target = selection; //Assign the selected object to the runtime transform handle
                        runtimeTransformGameObj.SetActive(true); //Activate the runtime transform handle
                        OnSelectionChanged?.Invoke(selection); // Notify selection change
                    }
                    highlight = null; //Disable highlighting
                }
                else
                {
                    if (selection)
                    {
                        selection.GetComponent<MeshRenderer>().materials = originalMaterialsSelection; //Restore the original material
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
                    selection.GetComponent<MeshRenderer>().materials = originalMaterialsSelection; //Restore the original material
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
}
