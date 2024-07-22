using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using SFB;
using System.Text;
using Dummiesman;

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

[System.Serializable]
public class ModelData
{
    public string modelPath;
    public TransformData[] transforms;
    public string materialPath;
}

[System.Serializable]
public class ModelList
{
    public List<ModelData> models = new List<ModelData>();
}

public class SaveSystem : MonoBehaviour
{
    public Material defaultMaterial;
    private List<GameObject> models = new List<GameObject>();
    private List<string> modelPaths = new List<string>();
    private List<string> materialPaths = new List<string>();
    private GameObject currentModel;

    public GameObject objectToDeactivate; // GameObject a desactivar

    public void SetModel(GameObject model, string modelPath)
    {
        this.currentModel = model;
        AddModel(model, modelPath);
    }

    public void AddModel(GameObject model, string modelPath)
    {
        models.Add(model);
        modelPaths.Add(modelPath);
        materialPaths.Add(null);
    }

    public void SetMaterialPath(GameObject model, string materialPath)
    {
        int index = models.IndexOf(model);
        if (index >= 0)
        {
            materialPaths[index] = materialPath;
        }
    }

    public void Save()
    {
        if (models.Count == 0)
        {
            Debug.LogError("No models set to save.");
            return;
        }

        ModelList modelList = new ModelList();

        for (int j = 0; j < models.Count; j++)
        {
            GameObject model = models[j];
            string modelPath = modelPaths[j];
            string materialPath = materialPaths[j];

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

            ModelData data = new ModelData
            {
                modelPath = modelPath,
                transforms = transformDataArray,
                materialPath = materialPath
            };

            modelList.models.Add(data);
        }

        string path = StandaloneFileBrowser.SaveFilePanel("Guardar archivo", "", "sceneData", "json");
        if (!string.IsNullOrEmpty(path))
        {
            string json = JsonUtility.ToJson(modelList, true);
            File.WriteAllText(path, json);
            Debug.Log("Escena guardada en: " + path);

            // Desactivar el GameObject después de guardar
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
    }

    public void Load()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Abrir archivo", "", "json", false);
        if (paths.Length > 0)
        {
            string path = paths[0];
            string json = File.ReadAllText(path);

            ModelList modelList = JsonUtility.FromJson<ModelList>(json);
            StartCoroutine(LoadModelsCoroutine(modelList));
        }
    }

    private IEnumerator LoadModelsCoroutine(ModelList modelList)
    {
        foreach (GameObject model in models)
        {
            Destroy(model);
        }
        models.Clear();
        modelPaths.Clear();
        materialPaths.Clear();

        foreach (ModelData data in modelList.models)
        {
            UnityWebRequest www = UnityWebRequest.Get(data.modelPath);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("ERROR WWW: " + www.error);
            }
            else
            {
                MemoryStream textStream = new MemoryStream(Encoding.UTF8.GetBytes(www.downloadHandler.text));
                GameObject loadedModel = new OBJLoader().Load(textStream);

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
                    UnityWebRequest materialRequest = UnityWebRequestAssetBundle.GetAssetBundle(data.materialPath);
                    yield return materialRequest.SendWebRequest();

                    if (materialRequest.result == UnityWebRequest.Result.Success)
                    {
                        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(materialRequest);
                        string[] assetNames = bundle.GetAllAssetNames();
                        if (assetNames.Length > 0)
                        {
                            Material loadedMaterial = bundle.LoadAsset<Material>(assetNames[0]);
                            SetMaterial(loadedModel, loadedMaterial);
                        }
                        bundle.Unload(false);
                    }
                    else
                    {
                        SetMaterial(loadedModel, defaultMaterial);
                    }
                }
                else
                {
                    SetMaterial(loadedModel, defaultMaterial);
                }

                models.Add(loadedModel);
                modelPaths.Add(data.modelPath);
                materialPaths.Add(data.materialPath);
            }
        }
    }

    private void SetMaterial(GameObject model, Material material)
    {
        foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>())
        {
            renderer.material = material;
        }
    }

    public void QuitGame()
    {
        Debug.Log("Botón de salir del juego presionado");

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
