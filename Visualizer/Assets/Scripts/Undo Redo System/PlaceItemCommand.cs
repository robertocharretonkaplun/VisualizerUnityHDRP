using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Comando para colocar un ítem en el juego.
/// </summary>
public class PlaceItemCommand : IAction
{
    // Variables de tipo estándar (STD)
    /// <summary>
    /// Prefab del ítem a colocar.
    /// </summary>
    private GameObject itemPrefab;
    
    /// <summary>
    /// Instancia del ítem colocado.
    /// </summary>
    private GameObject itemInstance;
    
    /// <summary>
    /// Posición donde se colocará el ítem.
    /// </summary>
    private Vector3 position;
    
    /// <summary>
    /// Rotación del ítem.
    /// </summary>
    private Quaternion rotation;
    
    /// <summary>
    /// Transform del padre del ítem.
    /// </summary>
    private Transform parent;

    // Constructor
    /// <summary>
    /// Constructor del comando PlaceItemCommand.
    /// </summary>
    /// <param name="itemPrefab">El prefab del ítem a colocar.</param>
    /// <param name="position">La posición donde colocar el ítem.</param>
    /// <param name="rotation">La rotación del ítem.</param>
    /// <param name="parent">El transform del objeto padre, si existe.</param>
    public PlaceItemCommand(GameObject itemPrefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        this.itemPrefab = itemPrefab;
        this.position = position;
        this.rotation = rotation;
        this.parent = parent;
    }

    // Métodos
    /// <summary>
    /// Ejecuta el comando de colocar el ítem.
    /// </summary>
    public void ExecuteCommand()
    {
        itemInstance = GameObject.Instantiate(itemPrefab, position, rotation, parent);
    }

    /// <summary>
    /// Deshace el comando de colocar el ítem.
    /// </summary>
    public void UndoCommand()
    {
        if (itemInstance != null)
        {
            GameObject.Destroy(itemInstance);
        }
    }
}
