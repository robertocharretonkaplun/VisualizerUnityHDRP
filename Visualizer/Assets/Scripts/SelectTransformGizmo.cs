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
/// Clase responsable de manejar la selección de objetos en la escena
/// y aplicar un material de selección temporal cuando un objeto está seleccionado.
/// También permite la manipulación del objeto seleccionado a través de un gizmo de transformación
/// y tiene comunicacion con SaveSystem y MaterialDragHandler.
/// </summary>
public class SelectTransformGizmo : MonoBehaviour
{
    public Material selectionMaterial; // Material aplicado al objeto cuando está seleccionado.
    public RuntimeHierarchy runtimeHierarchy; // Referencia a RuntimeHierarchy
    public CameraM sceneCamera; // Referencia al script de la cámara
    public MaterialDragHandler materialDragHandler; // Añadir referencia al MaterialDragHandler

    private Dictionary<Transform, Material[]> originalMaterialsSelection = new Dictionary<Transform, Material[]>(); // Diccionario que almacena los materiales originales de los objetos seleccionados y los restaura.
    private Transform selection; // Objeto actualmente seleccionado.
    private RaycastHit raycastHit; // Almacena la información del rayo que colisiona con objetos en la escena.
    private RaycastHit raycastHitHandle; // Almacena la información del rayo que colisiona con el gizmo de transformación.
    private GameObject runtimeTransformGameObj; // El objeto que contiene el gizmo de transformación en tiempo de ejecución.
    private RuntimeTransformHandle runtimeTransformHandle; // El script que maneja el gizmo de transformación.
    private int runtimeTransformLayer = 6; // La capa de objetos interactuables por el gizmo.
    private int runtimeTransformLayerMask; // Máscara de capa para filtrar objetos interactuables.
    private bool focusOnSelectionRequested; // Indica si se ha solicitado enfocar la cámara en el objeto seleccionado.

    public event Action<Transform> OnSelectionChanged;  // Evento que se dispara cuando la selección cambia.

    /// <summary>
    /// Método que se ejecuta al iniciar el componente. Se busca el script `MaterialDragHandler`
    /// en la escena y se asigna.
    /// </summary>
    private void Awake()
    {
        // Busca y asigna el script MaterialDragHandler en la escena.
        materialDragHandler = FindObjectOfType<MaterialDragHandler>();

        // Verificar si el selectTransformGizmo está asignado correctamente
        Debug.Log(materialDragHandler != null ? "materialDragHandler está asignado." : "materialDragHandler no está asignado.");
    }
    /// <summary>
    /// Método que se ejecuta cuando el objeto se activa. Inicializa el gizmo de transformación
    /// y configura las capas y eventos necesarios.
    /// </summary>
    private void Start()
    {
        // Crea un nuevo GameObject para manejar el gizmo de transformación en tiempo de ejecución.
        runtimeTransformGameObj = new GameObject();
        runtimeTransformHandle = runtimeTransformGameObj.AddComponent<RuntimeTransformHandle>();
        runtimeTransformGameObj.layer = runtimeTransformLayer;

        // Configura la máscara de capa para filtrar objetos interactuables por el gizmo.
        runtimeTransformLayerMask = 1 << runtimeTransformLayer; // Layer number represented by a single bit in the 32-bit integer using bit shift

        // Configura el tipo de transformación del gizmo (en este caso, solo posición).
        runtimeTransformHandle.type = HandleType.POSITION;
        runtimeTransformHandle.autoScale = true;
        runtimeTransformHandle.autoScaleFactor = 1.0f;

        // Desactiva el gizmo hasta que sea necesario.
        runtimeTransformGameObj.SetActive(false);

        // Asigna una etiqueta al gizmo para que pueda ser desactivado posteriormente.
        runtimeTransformGameObj.tag = "DeactivatableObject";

        // Suscribe el método `HandleSelectionChanged` al evento OnSelectionChanged.
        OnSelectionChanged += HandleSelectionChanged;

        // Si hay una jerarquía en tiempo de ejecución disponible, se suscribe a los cambios de selección.
        if (runtimeHierarchy != null)
        {
            // Suscribir un método que maneje los cambios de selección en la jerarquía (si está disponible).
            // runtimeHierarchy.OnSelectionChanged += HandleHierarchySelectionChanged;
        }
    }

    /// <summary>
    /// Verifica y registra si se ha aplicado un material al objeto actual en el `MeshRenderer`.
    /// </summary>
    /// <param name="renderer">El `MeshRenderer` del objeto.</param>
    private void CheckAndLogMaterialApplication(MeshRenderer renderer)
    {
        // Verifica si el `MeshRenderer` tiene materiales y si contiene algún material específico.
        if (renderer.materials.Length > 0 && ArrayContainsMaterial(renderer.materials, selectionMaterial))
        {
            Debug.Log("Se ha aplicado un nuevo material desde D&D");
        }
    }
    /// <summary>
    /// Método que se ejecuta en cada frame para gestionar la selección de objetos, la activación del gizmo de transformación
    /// y la aplicación de materiales de selección. También permite eliminar el objeto seleccionado.
    /// </summary>
    private void Update()
    {
        // Selección y enfoque en el objeto
        if (Input.GetKeyDown(KeyCode.F) && focusOnSelectionRequested)
        {
            if (sceneCamera != null && selection != null)
            {
                sceneCamera.SetTargetObject(selection); // Enfocar la cámara en el objeto seleccionado
            }
            focusOnSelectionRequested = false; // Resetea la bandera
        }
        // Selección de objetos con clic izquierdo del ratón
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            ApplyLayerToChildren(runtimeTransformGameObj); // Asegurarse de que el gizmo de transformación esté en la capa correcta
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit)) // Lanzar un rayo desde la cámara a través de la posición del ratón
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHitHandle, Mathf.Infinity, runtimeTransformLayerMask)) // Raycast towards runtime transform handle only
                {
                    // No hacer nada si el rayo colisiona con el gizmo de transformación
                }
                else if (raycastHit.transform.CompareTag("Selectable")) // Comprobar si el objeto con el que colisiona el rayo tiene el tag "Selectable"
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
                            originalMaterialsSelection[renderer.transform] = renderer.materials; // Guardar los materiales originales
                            ApplyMaterial(renderer, selectionMaterial); // Aplicar el nuevo material de seleccion
                            CheckAndLogMaterialApplication(renderer); // Verificar y registrar la aplicación del material
                            //Debug.Log("Hola"); // Mensaje en la consola
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
    /// Obtiene el objeto actualmente seleccionado.
    /// </summary>
    /// <returns>El objeto seleccionado.</returns>
    public Transform GetCurrentSelection()
    {
        return selection; // Devuelve el objeto seleccionado
    }
    /// <summary>
    /// Aplica un material al renderer de un objeto.
    /// </summary>
    /// <param name="renderer">El MeshRenderer al que se le aplicará el material.</param>
    /// <param name="material">El material que se aplicará.</param>
    private void ApplyMaterial(MeshRenderer renderer, Material material)
    {
        Material[] newMaterials = new Material[renderer.materials.Length + 1]; // Crear un nuevo array de materiales
        renderer.materials.CopyTo(newMaterials, 0); // Copiar los materiales originales al nuevo array
        newMaterials[newMaterials.Length - 1] = material; // Agregar el nuevo material al final del array
        renderer.materials = newMaterials; // Aplicar el nuevo array de materiales
    }
    /// <summary>
    /// Maneja el cambio de selección en la jerarquía.
    /// </summary>
    /// <param name="newSelection">El nuevo objeto seleccionado.</param>
    private void HandleSelectionChanged(Transform newSelection)
    {
        if (runtimeHierarchy != null)
        {
            runtimeHierarchy.Select(newSelection, RuntimeHierarchy.SelectOptions.FocusOnSelection); // Actualizar la selección en RuntimeHierarchy
        }
        if (sceneCamera != null)
        {
            focusOnSelectionRequested = true; // Set flag to request focus on the next F key press
        }
    }

    // <summary>
    /// Este método se invoca para aplicar un nuevo material a un objeto seleccionado.
    /// Si el objeto en cuestión es el actualmente seleccionado, actualiza su material
    /// en el diccionario de materiales originales.
    ///
    /// <param name="target">El objeto `Transform` al que se le aplica el nuevo material.</param>
    /// <param name="newMaterial">El nuevo material que se aplicará al objeto.</param>
    /// </summary>
    public void OnMaterialApplied(Transform target, Material newMaterial)
    {
        // Verifica si el objeto objetivo es el actualmente seleccionado
        if (target == selection)
        {
            Debug.Log("Se ha aplicado un nuevo material al objeto seleccionado con SelectTG.");

            // Obtiene todos los MeshRenderers del objeto seleccionado y de sus hijos
            MeshRenderer[] renderers = target.GetComponentsInChildren<MeshRenderer>();

            // Recorre todos los MeshRenderers y actualiza el material en el diccionario
            foreach (MeshRenderer renderer in renderers)
            {
                // Verifica si este renderer ya tiene materiales originales almacenados
                if (originalMaterialsSelection.ContainsKey(renderer.transform))
                {
                    // Actualiza el array de materiales originales para este renderer
                    Material[] originalMaterials = originalMaterialsSelection[renderer.transform];

                    // Si el número de materiales no coincide, crea un nuevo array con el tamaño adecuado
                    if (originalMaterials.Length != renderer.materials.Length)
                    {
                        originalMaterialsSelection[renderer.transform] = new Material[renderer.materials.Length];
                    }

                    // Actualiza los materiales originales con el nuevo material
                    for (int i = 0; i < renderer.materials.Length; i++)
                    {
                        originalMaterialsSelection[renderer.transform][i] = newMaterial;
                    }
                }
                else
                {
                    // Si el renderer no tiene materiales guardados, se agrega al diccionario
                    originalMaterialsSelection.Add(renderer.transform, new Material[renderer.materials.Length]);

                    // Inicializa con el nuevo material aplicado
                    for (int i = 0; i < renderer.materials.Length; i++)
                    {
                        originalMaterialsSelection[renderer.transform][i] = newMaterial;
                    }
                }
            }
        }
    }
    /// <summary>
    /// Maneja el cambio de selección en la jerarquía.
    /// </summary>
    /// <param name="newSelection">El nuevo objeto seleccionado.</param>
    private void HandleHierarchySelectionChanged(Transform newSelection)
    {
        HandleSelectionChanged(newSelection); // Manejar el hierarchy selection change
    }
    /// <summary>
    /// Aplica la capa del GameObject padre a todos sus hijos.
    /// </summary>
    /// <param name="parentGameObj">El GameObject padre cuyo layer se aplicará a los hijos.</param>
    private void ApplyLayerToChildren(GameObject parentGameObj)
    {
        foreach (Transform transform1 in parentGameObj.transform)
        {
            int layer = parentGameObj.layer; // Obtener la capa del GameObject padre
            transform1.gameObject.layer = layer; // Aplicar la capa al hijo
            foreach (Transform transform2 in transform1)
            {
                transform2.gameObject.layer = layer; // Aplicar la capa a los nietos
                foreach (Transform transform3 in transform2)
                {
                    transform3.gameObject.layer = layer; // Aplicar la capa a los bisnietos
                    foreach (Transform transform4 in transform3)
                    {
                        transform4.gameObject.layer = layer; // Aplicar la capa a los tataranietos
                        foreach (Transform transform5 in transform4)
                        {
                            transform5.gameObject.layer = layer; // Aplicar la capa a los ya no me se mas terminos pero el que sigue
                        }
                    }
                }
            }
        }
    }
    // <summary>
    /// Comprueba si un material ya existe en un array de materiales.
    /// </summary>
    /// <param name="materials">Array de materiales a comprobar.</param>
    /// <param name="material">El material que se busca.</param>
    /// <returns>True si el material existe en el array, de lo contrario False.</returns>
    private bool ArrayContainsMaterial(Material[] materials, Material material) // Check if a material already exists in an array of materials
    {
        foreach (var mat in materials)
        {
            if (mat == material)
            {
                return true; // El material existe
            }
        }
        return false; // El material no existe
    }
    /// <summary>
    /// Obtiene todos los MeshRenderers de un objeto y sus hijos.
    /// </summary>
    /// <param name="transform">El objeto del cual se obtendrán los MeshRenderers.</param>
    /// <returns>Array de MeshRenderers encontrados.</returns>
    private MeshRenderer[] GetMeshRenderers(Transform transform)
    {
        List<MeshRenderer> renderers = new List<MeshRenderer>(transform.GetComponentsInChildren<MeshRenderer>());// Obtener todos los MeshRenderers en el objeto y sus hijos
        return renderers.ToArray(); // Devolver como array
    }
    /// <summary>
    /// Establece los materiales originales en un objeto y sus hijos.
    /// </summary>
    /// <param name="transform">El objeto en el que se restaurarán los materiales.</param>
    /// <param name="materialsDict">Diccionario que contiene los materiales originales.</param>
    /// <param name="isSelection">Indica si se está restaurando desde una selección.</param>
    private void SetMaterials(Transform transform, Dictionary<Transform, Material[]> materialsDict, bool isSelection)
    {
        MeshRenderer[] renderers = GetMeshRenderers(transform); // Obtener los MeshRenderers del objeto
        foreach (MeshRenderer renderer in renderers)
        {
            if (materialsDict.TryGetValue(renderer.transform, out Material[] originalMaterials))
            {
                renderer.materials = originalMaterials; // Restaurar los materiales originales
                if (isSelection)
                {
                    materialsDict.Remove(renderer.transform); // Eliminar el objeto del diccionario si es parte de la selección
                }
            }
        }
    }
    /// <summary>
    /// Obtiene el GameObject del transform dinámico.
    /// </summary>
    /// <returns>El GameObject dinámico que contiene el RuntimeTransformHandle.</returns>
    public GameObject GetRuntimeTransformGameObject()
    {
        return runtimeTransformGameObj; // Devuelve el GameObject dinámico runtimeTransformGameObj que tiene asignado un RuntimeTransformHandle proporcionando funcionalidades para manipular transfromaciones del Obj seleccionado
    }
}
