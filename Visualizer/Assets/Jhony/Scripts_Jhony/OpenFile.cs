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
        display = gameObject.GetComponent<Display>();
        saveSystem = gameObject.GetComponent<SaveSystem>();
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    public void OnClickOpen() {
        UploadFile(gameObject.name, "OnFileUpload", ".obj", false);
    }

    public void OnFileUpload(string url) {
        StartCoroutine(OutputRoutineOpen(url));
    }
#else
    public void OnClickOpen()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "obj", false);
        if (paths.Length > 0)
        {
            StartCoroutine(OutputRoutineOpen(new System.Uri(paths[0]).AbsoluteUri));
        }
    }
#endif

    private IEnumerator OutputRoutineOpen(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("WWW ERROR: " + www.error);
        }
        else
        {
            MemoryStream textStream = new MemoryStream(Encoding.UTF8.GetBytes(www.downloadHandler.text));
            GameObject model = new OBJLoader().Load(textStream);
            model.transform.localScale = new Vector3(1, 1, 1);

            // Combine all meshes into one
            CombineMeshes(model);

            display.SetMaterial(model, defaultMaterial);

            foreach (var renderer in model.GetComponentsInChildren<Renderer>())
            {
                renderer.gameObject.tag = "Selectable";

                if (renderer.gameObject.GetComponent<BoxCollider>() == null)
                {
                    renderer.gameObject.AddComponent<BoxCollider>();
                }
            }

            Debug.Log("Tag assigned to all children with Renderer components.");

            saveSystem.SetModel(model, url);
        }
    }

    private void CombineMeshes(GameObject model)
    {
        MeshFilter[] meshFilters = model.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }

        MeshFilter combinedMeshFilter = model.AddComponent<MeshFilter>();
        combinedMeshFilter.mesh = new Mesh();
        combinedMeshFilter.mesh.CombineMeshes(combine, true, true);

        MeshRenderer combinedMeshRenderer = model.AddComponent<MeshRenderer>();
        combinedMeshRenderer.sharedMaterial = defaultMaterial;

        for (int i = 0; i < meshFilters.Length; i++)
        {
            Destroy(meshFilters[i].gameObject);
        }
    }
}
