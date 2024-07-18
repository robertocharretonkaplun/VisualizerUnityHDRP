using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using SFB;
using UnityEngine.Networking;
using Dummiesman;

public class OpenFile : MonoBehaviour
{
    public Material defaultMaterial;
    private Display display;
    private SaveSystem saveSystem;

    private void Start()
    {
        display = gameObject.GetComponent<Display>(); // Obtener referencia al componente Display adjunto al mismo GameObject
        saveSystem = gameObject.GetComponent<SaveSystem>(); // Obtener referencia al componente SaveSystem adjunto al mismo GameObject
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    public void OnClickOpen()
    {
        UploadFile(gameObject.name, "OnFileUpload", ".obj", false); // Llama a una función de JavaScript para subir archivos en WebGL
    }

    public void OnFileUpload(string url)
    {
        StartCoroutine(OutputRoutineOpen(url)); // Inicia la rutina para procesar el archivo subido
    }
#else
    public void OnClickOpen()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "obj", false); // Abre un panel de diálogo para seleccionar archivos en otras plataformas
        if (paths.Length > 0)
        {
            StartCoroutine(OutputRoutineOpen(new System.Uri(paths[0]).AbsoluteUri)); // Inicia la rutina para procesar el archivo seleccionado
        }
    }
#endif

    private IEnumerator OutputRoutineOpen(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url); // Crea una solicitud para obtener el archivo desde la URL
        yield return www.SendWebRequest(); // Envía la solicitud y espera la respuesta

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("WWW ERROR: " + www.error); // Registro de error si la solicitud no tiene éxito
        }
        else
        {
            MemoryStream textStream = new MemoryStream(Encoding.UTF8.GetBytes(www.downloadHandler.text)); // Convierte el texto descargado en un stream de memoria
            GameObject model = new OBJLoader().Load(textStream); // Carga el modelo OBJ desde el stream

            model.transform.localScale = new Vector3(1, 1, 1); // Ajusta la escala del modelo cargado

            // Combina todas las mallas del modelo en una sola
            CombineMeshes(model);

            display.SetMaterial(model, defaultMaterial); // Asigna el material por defecto al modelo usando el componente Display

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

            Debug.Log("Tag assigned to all children with Renderer components."); // Registro de confirmación en la consola

            saveSystem.SetModel(model, url); // Llama al método SetModel del componente SaveSystem para guardar el modelo y su ruta
        }
    }

    // Método para combinar las mallas de un modelo en una sola
    private void CombineMeshes(GameObject model)
    {
        MeshFilter[] meshFilters = model.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false); // Desactiva los objetos originales de las mallas combinadas
        }

        MeshFilter combinedMeshFilter = model.AddComponent<MeshFilter>(); // Añade un MeshFilter al modelo combinado
        combinedMeshFilter.mesh = new Mesh(); // Crea una nueva malla para el modelo combinado
        combinedMeshFilter.mesh.CombineMeshes(combine, true, true); // Combina las mallas en una sola malla para el MeshFilter

        MeshRenderer combinedMeshRenderer = model.AddComponent<MeshRenderer>(); // Añade un MeshRenderer al modelo combinado
        combinedMeshRenderer.sharedMaterial = defaultMaterial; // Asigna el material por defecto al MeshRenderer

        for (int i = 0; i < meshFilters.Length; i++)
        {
            Destroy(meshFilters[i].gameObject); // Destruye los objetos originales de las mallas combinadas
        }
    }
}
