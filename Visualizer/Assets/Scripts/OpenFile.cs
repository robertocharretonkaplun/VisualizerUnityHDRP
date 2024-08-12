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
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using SFB;                              //Libreria para Standalone file browser
using UnityEngine.Networking;           // Proporciona clases y métodos para realizar solicitudes web y manejar la red (Descargar archivos o interactuar con servicios web)
using Dummiesman;                       //Libreria para cargar modelos OBJ 
// -------------------------------
// OpenFile Class
// -------------------------------
public class OpenFile : MonoBehaviour
{
    // -------------------------------
    // Variables Publicas
    // -------------------------------
    [Header("Configuración del modelo")]
    public Material defaultMaterial;    //Material que le asignamos por defecto a los modelos cargados
    // -------------------------------
    // Variables Privadas
    // -------------------------------
    [Header("Referencias a los componente")]
    private Display display;            //Referencia al componente display
    private SaveSystem saveSystem;      //Referencia al componente componente SaveSystem

    // -------------------------------
    // Método de Inicialización
    // -------------------------------
    /// <summary>
    /// Método Start o inicio
    /// </summary>
    /// <remarks>
    /// Este método no recibe parámetros ni devuelve ningún valor. Es llamado automáticamente por el motor de Unity al iniciar el script.
    /// </remarks>
    private void Start()
    {
        display = gameObject.GetComponent<Display>();       // Obtener referencia al componente Display adjunto al mismo GameObject
        saveSystem = gameObject.GetComponent<SaveSystem>(); // Obtener referencia al componente SaveSystem adjunto al mismo GameObject
    }
    /// <summary>
    /// Funcionalidad en web
    /// </summary>
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    public void OnClickOpen()
    {
        UploadFile(gameObject.name, "OnFileUpload", ".obj", false); // Llama a una función de JavaScript para subir archivos en WebGL
    }

    public void OnFileUpload(string url)
    {
        StartCoroutine(OutputRoutineOpen(url));                     // Inicia la rutina para procesar el archivo subido
    }
#else
    // -------------------------------
    // Métodos
    // -------------------------------
    /// <summary>
    /// Método para abrir el panel de selección de archivos en otras plataformas
    /// </summary>
    /// <remarks>
    /// Este método no recibe parámetros y no devuelve ningún valor. Abre un panel de diálogo para seleccionar archivos y, si se selecciona un archivo, inicia una corrutina para procesar el archivo seleccionado.
    /// </remarks>
    public void OnClickOpen()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "obj", false); // Abre un panel de diálogo para seleccionar archivos en otras plataformas
        if (paths.Length > 0)
        {
            StartCoroutine(OutputRoutineOpen(new System.Uri(paths[0]).AbsoluteUri));         // Inicia la rutina para procesar el archivo seleccionado
        }
    }
#endif
    // -------------------------------
    // Corrutinas
    // -------------------------------
    /// <summary>
    /// Corrutina para procesar el archivo abierto.
    /// </summary>
    /// <param name="url">La URL del archivo que se va a procesar.</param>
    /// <returns>Devuelve un IEnumerator que permite la ejecución de la corrutina.</returns>
    private IEnumerator OutputRoutineOpen(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);                                                  // Crea una solicitud para obtener el archivo desde la URL
        yield return www.SendWebRequest();                                                               // Envía la solicitud y espera la respuesta

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("WWW ERROR: " + www.error);                                                        // Registro de error si la solicitud no tiene éxito
        }
        else
        {
            MemoryStream textStream = new MemoryStream(Encoding.UTF8.GetBytes(www.downloadHandler.text));// Convierte el texto descargado en un stream de memoria
            GameObject model = new OBJLoader().Load(textStream);                                         // Carga el modelo OBJ desde el stream
            model.transform.localScale = new Vector3(1, 1, 1);                                           // Ajusta la escala del modelo cargado

            // Combina todas las mallas del modelo en una sola
            CombineMeshes(model);
            display.SetMaterial(model, defaultMaterial);                                                 // Asigna el material por defecto al modelo usando el componente Display

            // Asigna la etiqueta "Selectable" a todos los hijos del modelo que tienen componentes Renderer
            foreach (var renderer in model.GetComponentsInChildren<Renderer>())
            {
                renderer.gameObject.tag = "Selectable";

                // Agrega un BoxCollider si no existe en los objetos con Renderer
                if (renderer.gameObject.GetComponent<BoxCollider>() == null)
                {
                    renderer.gameObject.AddComponent<BoxCollider>();
                }
            }

            Debug.Log("Tag assigned to all children with Renderer components.");                           // Registro de confirmación en la consola
            saveSystem.SetModel(model, url);                                                               // Llama al método SetModel del componente SaveSystem para guardar el modelo y su ruta
        }
    }

    // -------------------------------
    // Métodos de Utilidad
    // -------------------------------
    /// <summary>
    /// Método para combinar las mallas de un modelo en una sola
    /// </summary>/// </summary>
    /// <param name="model">El objeto modelo cuyo MeshFilters se combinarán.</param>
    /// <remarks>
    /// Este método no devuelve ningún valor. Combina todas las mallas (MeshFilters) de un modelo en una sola malla utilizando un MeshFilter combinado y un MeshRenderer. Luego, destruye los objetos originales de las mallas combinadas.
    /// </remarks>
    private void CombineMeshes(GameObject model)
    {
        MeshFilter[] meshFilters = model.GetComponentsInChildren<MeshFilter>();     // Obtiene todos los MeshFilters en los hijos del modelo
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];        // Arreglo de instancias combinadas

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);                             // Desactiva los objetos originales de las mallas combinadas
        }

        MeshFilter combinedMeshFilter = model.AddComponent<MeshFilter>();           // Añade un MeshFilter al modelo combinado
        combinedMeshFilter.mesh = new Mesh();                                       // Crea una nueva malla para el modelo combinado
        combinedMeshFilter.mesh.CombineMeshes(combine, true, true);                 // Combina las mallas en una sola malla para el MeshFilter

        MeshRenderer combinedMeshRenderer = model.AddComponent<MeshRenderer>();     // Añade un MeshRenderer al modelo combinado
        combinedMeshRenderer.sharedMaterial = defaultMaterial;                      // Asigna el material por defecto al MeshRenderer

        for (int i = 0; i < meshFilters.Length; i++)
        {
            Destroy(meshFilters[i].gameObject);                                     // Destruye los objetos originales de las mallas combinadas
        }
    }
}
