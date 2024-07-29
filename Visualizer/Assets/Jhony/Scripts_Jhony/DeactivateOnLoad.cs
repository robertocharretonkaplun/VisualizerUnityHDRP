using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
using SFB;

public class DeactivateOnLoad : MonoBehaviour
{
    public GameObject objectToDeactivate;

    // Método para cargar el archivo JSON y desactivar el GameObject
    public void LoadAndDeactivate()
    {
        // Abrir el panel de selección de archivo para cargar el JSON
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Abrir archivo", "", "json", false);
        if (paths.Length > 0)
        {
            string path = paths[0];
            StartCoroutine(LoadJSON(path));
        }
    }

    // Corrutina para cargar el archivo JSON
    private IEnumerator LoadJSON(string path)
    {
        // Leer el archivo JSON
        UnityWebRequest www = UnityWebRequest.Get("file://" + path);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            Debug.Log("Archivo JSON cargado: " + path);

            // Desactivar el GameObject
            if (objectToDeactivate != null)
            {
                objectToDeactivate.SetActive(false);
                Debug.Log("GameObject desactivado.");
            }
            else
            {
                Debug.LogWarning("No se ha asignado ningún GameObject para desactivar.");
            }
        }
        else
        {
            Debug.LogError("Error al cargar el archivo JSON: " + www.error);
        }
    }
}
