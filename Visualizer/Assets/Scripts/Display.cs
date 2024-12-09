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
