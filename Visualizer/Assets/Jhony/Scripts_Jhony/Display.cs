using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Display : MonoBehaviour
{
    //Metodo para asignar un material a todos los renderer de un modelo y a sus hijos
    public void SetMaterial(GameObject model, Material material)
    {
        //Recorremos todos lops compopnentes Renderer en el moodelo y sus hijos
        foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>())
        {
            //Asignamos el material proporcionado a cada renderer
            renderer.material = material;
        }
    }
}
