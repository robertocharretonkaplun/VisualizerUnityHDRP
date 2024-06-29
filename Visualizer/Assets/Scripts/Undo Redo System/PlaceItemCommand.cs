using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlaceItemCommand : IAction
{
    private GameObject itemPrefab;
    private GameObject itemInstance;
    private Vector3 position;
    private Quaternion rotation;
    private Transform parent;

    public PlaceItemCommand(GameObject itemPrefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        this.itemPrefab = itemPrefab;
        this.position = position;
        this.rotation = rotation;
        this.parent = parent;
    }

    public void ExecuteCommand()
    {
        itemInstance = GameObject.Instantiate(itemPrefab, position, rotation, parent);
    }

    public void UndoCommand()
    {
        if (itemInstance != null)
        {
            GameObject.Destroy(itemInstance);
        }
    }
}
