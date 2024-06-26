using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using SFB;
using System.Text;
using Dummiesman;

// Clase serializable que representa los datos del transform
[System.Serializable]
public class TransformData
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public string tag;
    public bool hasBoxCollider;
    public Vector3 boxColliderCenter;
    public Vector3 boxColliderSize;
}

// Clase serializable que representa los datos del modelo
[System.Serializable]
public class ModelData
{
    public string modelPath; // Ruta del archivo del modelo
    public TransformData[] transforms; // Datos de las transformaciones
    public string materialPath; // Ruta del archivo del material
}

public class SaveSystem : MonoBehaviour
{
    public Material defaultMaterial;
    private GameObject model; // Referencia al modelo cargado
    private string modelPath;
    private string materialPath;

    // Método para establecer el modelo y su ruta
    public void SetModel(GameObject model, string modelPath)
    {
        this.model = model;
        this.modelPath = modelPath;
    }

    // Método para establecer la ruta del material
    public void SetMaterialPath(string materialPath)
    {
        this.materialPath = materialPath;
    }

    // Método para guardar los datos del modelo
    public void Save()
    {
        if (model == null)
        {
            Debug.LogError("No model set to save."); // Error si no hay modelo
            return;
        }

        // Obtener las transformaciones de todos los hijos del modelo
        Transform[] childTransforms = model.GetComponentsInChildren<Transform>();
        TransformData[] transformDataArray = new TransformData[childTransforms.Length];
        for (int i = 0; i < childTransforms.Length; i++)
        {
            Transform t = childTransforms[i];
            BoxCollider boxCollider = t.GetComponent<BoxCollider>();
            transformDataArray[i] = new TransformData
            {
                position = t.position,
                rotation = t.rotation,
                scale = t.localScale,
                tag = t.tag,
                hasBoxCollider = boxCollider != null,
                boxColliderCenter = boxCollider != null ? boxCollider.center : Vector3.zero,
                boxColliderSize = boxCollider != null ? boxCollider.size : Vector3.zero
            };
        }

        // Crear el objeto ModelData con las transformaciones
        ModelData data = new ModelData
        {
            modelPath = modelPath,
            transforms = transformDataArray,
            materialPath = materialPath
        };

        // Guardar los datos en un archivo JSON
        string path = StandaloneFileBrowser.SaveFilePanel("Save File", "", "modelData", "json");
        if (!string.IsNullOrEmpty(path))
        {
            string json = JsonUtility.ToJson(data, true); // Convierte los datos en JSON
            File.WriteAllText(path, json); // Escribe el JSON en un archivo
            Debug.Log("Model saved to: " + path); // Log de confirmación
        }
    }

    // Método para cargar los datos del modelo
    public void Load()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "json", false);
        if (paths.Length > 0)
        {
            string path = paths[0];
            string json = File.ReadAllText(path); // Lee el JSON del archivo

            ModelData data = JsonUtility.FromJson<ModelData>(json); // Convierte el JSON a objeto
            StartCoroutine(LoadModelCoroutine(data)); // Inicia la corrutina para cargar el modelo
        }
    }

    // Corrutina para cargar el modelo de forma asíncrona
    private IEnumerator LoadModelCoroutine(ModelData data)
    {
        UnityWebRequest www = UnityWebRequest.Get(data.modelPath); // Solicitud para obtener el modelo
        yield return www.SendWebRequest(); // Esperar a que la solicitud se complete

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("WWW ERROR: " + www.error); // Error si la solicitud falla
        }
        else
        {
            MemoryStream textStream = new MemoryStream(Encoding.UTF8.GetBytes(www.downloadHandler.text)); // Convierte la respuesta en un stream
            GameObject loadedModel = new OBJLoader().Load(textStream); // Carga el modelo OBJ

            // Asignar las transformaciones a todos los hijos
            Transform[] childTransforms = loadedModel.GetComponentsInChildren<Transform>();
            for (int i = 0; i < childTransforms.Length && i < data.transforms.Length; i++)
            {
                TransformData tData = data.transforms[i];
                Transform t = childTransforms[i];
                t.position = tData.position;
                t.rotation = tData.rotation;
                t.localScale = tData.scale;
                t.tag = tData.tag;

                if (tData.hasBoxCollider)
                {
                    BoxCollider boxCollider = t.gameObject.AddComponent<BoxCollider>();
                    boxCollider.center = tData.boxColliderCenter;
                    boxCollider.size = tData.boxColliderSize;
                }
            }

            if (!string.IsNullOrEmpty(data.materialPath))
            {
                UnityWebRequest materialRequest = UnityWebRequestAssetBundle.GetAssetBundle(data.materialPath); // Solicitud para obtener el material
                yield return materialRequest.SendWebRequest(); // Esperar a que la solicitud se complete

                if (materialRequest.result == UnityWebRequest.Result.Success)
                {
                    AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(materialRequest); // Obtiene el bundle del material
                    string[] assetNames = bundle.GetAllAssetNames(); // Obtiene los nombres de los assets en el bundle
                    if (assetNames.Length > 0)
                    {
                        Material loadedMaterial = bundle.LoadAsset<Material>(assetNames[0]); // Carga el material
                        SetMaterial(loadedModel, loadedMaterial); // Asigna el material al modelo
                    }
                    bundle.Unload(false); // Descarga el bundle sin destruir los assets
                }
                else
                {
                    SetMaterial(loadedModel, defaultMaterial); // Asigna el material por defecto si la solicitud falla
                }
            }
            else
            {
                SetMaterial(loadedModel, defaultMaterial); // Asigna el material por defecto si la solicitud falla
            }

            // Establece el modelo cargado como el modelo actual
            model = loadedModel;
        }
    }

    // Método para asignar un material a todos los Renderers un modelo y sus hijos
    private void SetMaterial(GameObject model, Material material)
    {
        foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>())
        {
            renderer.material = material; // Asigna el material proporcionado a cada Renderer
        }
    }
}
