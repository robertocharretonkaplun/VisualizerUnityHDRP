using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is an other example on how to implement undo redo, changing the color of the gameobject.
/// </summary>

public class ChangeMaterialCommand : IAction
{
    private Material oldMaterial;
    private Material newMaterial;
    private GameObject objectToChangeMaterial;
  
    public ChangeMaterialCommand(Material oldMaterial,Material newMaterial,GameObject objectToChangeMaterial)
    {
        this.oldMaterial = oldMaterial;
        this.newMaterial = newMaterial;
        this.objectToChangeMaterial = objectToChangeMaterial;
    }

    public void ExecuteCommand()
    {
        objectToChangeMaterial.GetComponent<Renderer>().material = newMaterial;
    }

    public void UndoCommand()
    {
        objectToChangeMaterial.GetComponent<Renderer>().material = oldMaterial;
    }
}
