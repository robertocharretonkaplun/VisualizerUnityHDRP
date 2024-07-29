using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// -------------------------------
// Display Class
// -------------------------------
public class Display : MonoBehaviour
{
    // -------------------------------
    // Métodos
    // -------------------------------
    /// <summary>
    /// Asigna un material a todos los renders de un modelo y sus hijos.
    /// </summary>
    /// <param name="model">El objeto modelo al que se le asignará el material.</param>
    /// <param name="material">El material que se asignará a los renderizadores.</param>
    /// <returns>Este método no devuelve ningún valor.</returns>
    public void SetMaterial(GameObject model, Material material)
    {
        //Recorremos todos lops compopnentes Renderer en el moodelo y sus hijos
        foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>())    
        {
            
            renderer.material = material;   //Asignamos el material proporcionado a cada renderer
        }
    }
}
