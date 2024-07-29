using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// -------------------------------
// Display Class
// -------------------------------
public class Display : MonoBehaviour
{
    // -------------------------------
    // M�todos
    // -------------------------------
    /// <summary>
    /// Asigna un material a todos los renders de un modelo y sus hijos.
    /// </summary>
    /// <param name="model">El objeto modelo al que se le asignar� el material.</param>
    /// <param name="material">El material que se asignar� a los renderizadores.</param>
    /// <returns>Este m�todo no devuelve ning�n valor.</returns>
    public void SetMaterial(GameObject model, Material material)
    {
        //Recorremos todos lops compopnentes Renderer en el moodelo y sus hijos
        foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>())    
        {
            
            renderer.material = material;   //Asignamos el material proporcionado a cada renderer
        }
    }
}
