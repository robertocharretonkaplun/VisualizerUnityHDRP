using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; // Archivos y rutas
using System.Text; // Manejar texto en el c�digo y conversiones
using System.Runtime.InteropServices; // Trabajar directo con memoria
using UnityEngine.UI; // UI unity
using SFB; // Sistema de nombres personalizados por UnityStandaOneFileBrowser
using UnityEngine.Networking; // Crea y administra conexiones de red enviando y recibiendo datos
using Dummiesman;

public class OpenFile : MonoBehaviour
{
    public Material defaultMaterial; // Material Default
    private Display display; //Referenicia al componente Display 
    private SaveSystem saveSystem; //Referencia al componente SaveSystem

    //Obtenemos los componentes del GameObject
    private void Start()
    {
        display = gameObject.GetComponent<Display>();
        saveSystem = gameObject.GetComponent<SaveSystem>();
    }

#if UNITY_WEBGL && !UNITY_EDITOR
// WebGL 
[DllImport("__Internal")] 
private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

public void OnClickOpen() {
    UploadFile(gameObject.name, "OnFileUpload", ".obj", false);
}

// Called from browser
public void OnFileUpload(string url) {
    StartCoroutine(OutputRoutineOpen(url));
}
#else
    // Standalone platforms & editor
    public void OnClickOpen()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "obj", false); // Abre el panel para seleccionar el Obj
        if (paths.Length > 0)
        {
            StartCoroutine(OutputRoutineOpen(new System.Uri(paths[0]).AbsoluteUri)); // URI Object: Devuelve la URL completa cuando abre el archivo
        }
    }
#endif

    private IEnumerator OutputRoutineOpen(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url); // Crea una solicitud para obtener el archivo
        yield return www.SendWebRequest(); // Esperar a que la solicitud se complete
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("WWW ERROR: " + www.error); // Mostrar error en caso de fallo en la solicitud
        }
        else
        {
            // Cargar el Modelo Obj
            MemoryStream textStream = new MemoryStream(Encoding.UTF8.GetBytes(www.downloadHandler.text)); // Flujo de memoria a partir del texto descargado
            GameObject model = new OBJLoader().Load(textStream);
            model.transform.localScale = new Vector3(1, 1, 1); // OBJLoader establece la escala x como -1 de forma predeterminada, la cambiamos a 1

            // Asignamos un material predeterminado
            display.SetMaterial(model, defaultMaterial);

            // Asignamos el tag "Selectable" a todos los hijos con el componente Renderer
            foreach (var renderer in model.GetComponentsInChildren<Renderer>())
            {
                renderer.gameObject.tag = "Selectable";

                // A�adir BoxCollider a los hijos si no tienen uno
                if (renderer.gameObject.GetComponent<BoxCollider>() == null)
                {
                    renderer.gameObject.AddComponent<BoxCollider>();
                }
            }

            // Debug para verificar si se asign� correctamente el tag
            Debug.Log("Tag assigned to all children with Renderer components.");

            // Pasar la informaci�n del modelo al SaveSystem
            saveSystem.SetModel(model, url);
        }
    }
}
