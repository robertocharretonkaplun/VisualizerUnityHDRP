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

// Clase serializable que representa la lista de modelos
[System.Serializable]
public class ModelList
{
    public List<ModelData> models = new List<ModelData>();
}

public class SaveSystem : MonoBehaviour
{
    public Material defaultMaterial;
    private List<GameObject> models = new List<GameObject>(); // Referencia a los modelos cargados
    private List<string> modelPaths = new List<string>();
    private List<string> materialPaths = new List<string>();
    private GameObject currentModel;

    // Método para establecer el modelo y su ruta (compatibilidad)
    public void SetModel(GameObject model, string modelPath)
    {
        this.currentModel = model;
        AddModel(model, modelPath);
    }

    // Método para agregar un modelo y su ruta
    public void AddModel(GameObject model, string modelPath)
    {
        models.Add(model);
        modelPaths.Add(modelPath);
        materialPaths.Add(null); // Inicialmente sin ruta de material
    }

    // Método para establecer la ruta del material para un modelo
    public void SetMaterialPath(GameObject model, string materialPath)
    {
        int index = models.IndexOf(model);
        if (index >= 0)
        {
            materialPaths[index] = materialPath;
        }
    }

    // Método para guardar los datos de los modelos y la escena
    public void Save()
    {
        if (models.Count == 0)
        {
            Debug.LogError("No models set to save."); // Error si no hay modelos
            return;
        }

        // Crear una lista de ModelData
        ModelList modelList = new ModelList();

        for (int j = 0; j < models.Count; j++)
        {
            GameObject model = models[j];
            string modelPath = modelPaths[j];
            string materialPath = materialPaths[j];

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

            modelList.models.Add(data);
        }

        // Guardar los datos en un archivo JSON
        string path = StandaloneFileBrowser.SaveFilePanel("Save File", "", "sceneData", "json");
        if (!string.IsNullOrEmpty(path))
        {
            string json = JsonUtility.ToJson(modelList, true); // Convierte los datos en JSON
            File.WriteAllText(path, json); // Escribe el JSON en un archivo
            Debug.Log("Scene saved to: " + path); // Log de confirmación
        }
    }

    // Método para cargar los datos de los modelos y la escena
    public void Load()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "json", false);
        if (paths.Length > 0)
        {
            string path = paths[0];
            string json = File.ReadAllText(path); // Lee el JSON del archivo

            ModelList modelList = JsonUtility.FromJson<ModelList>(json); // Convierte el JSON a objeto
            StartCoroutine(LoadModelsCoroutine(modelList)); // Inicia la corrutina para cargar los modelos
        }
    }

    // Corrutina para cargar los modelos de forma asíncrona
    private IEnumerator LoadModelsCoroutine(ModelList modelList)
    {
        // Limpiar modelos actuales antes de cargar nuevos
        foreach (GameObject model in models)
        {
            Destroy(model);
        }
        models.Clear();
        modelPaths.Clear();
        materialPaths.Clear();

        foreach (ModelData data in modelList.models)
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

                // Agregar el modelo cargado a la lista de modelos actuales
                models.Add(loadedModel);
                modelPaths.Add(data.modelPath);
                materialPaths.Add(data.materialPath);
            }
        }
    }

    // Método para asignar un material a todos los Renderers de un modelo y sus hijos
    private void SetMaterial(GameObject model, Material material)
    {
        foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>())
        {
            renderer.material = material; // Asigna el material proporcionado a cada Renderer
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game button pressed");

        Application.Quit();

        // Si estamos en el editor de Unity, detener la reproducción
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
