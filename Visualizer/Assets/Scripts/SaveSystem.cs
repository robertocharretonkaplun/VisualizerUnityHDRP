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
using UnityEngine.Networking;
using SFB;
using System.Text;
using Dummiesman;

// -------------------------------
// Clases Atributos
// -------------------------------

[System.Serializable]
public class TransformData
{
    [Header("Datos de Transformación")]
    public Vector3 position;            // Posición del objeto
    public Vector3 scale;               // Escala del objeto
    public Vector3 boxColliderCenter;   // Centro del BoxCollider
    public Vector3 boxColliderSize;     // Tamaño del BoxCollider
    public Quaternion rotation;         // Rotación del objeto
    public string tag;                  // Tag del objeto
    public bool hasBoxCollider;         // Indica si el objeto tiene un BoxCollider  
}

[System.Serializable]
public class ModelData
{
    [Header("Ruta del Modelo y Material")]
    public string modelPath;            // Ruta del modelo
    public string materialPath;         // Ruta del material
    public TransformData[] transforms;  // Datos de las transformaciones
}

[System.Serializable]
public class ModelList
{
    [Header("Lista de Modelos")]
    public List<ModelData> models = new List<ModelData>();  // Lista de ModelData
}

// -------------------------------
// Save System
// -------------------------------
public class SaveSystem : MonoBehaviour
{
    // -------------------------------
    // Variables Publicas
    // -------------------------------
    [Header("Configuración de Material y Desactivación")]
    public Material defaultMaterial;                            // Material por defecto
    public GameObject objectToDeactivate;                       // GameObject a desactivar
    // -------------------------------
    // Variables Privadas
    // -------------------------------
    [Header("Datos Internos del Sistema")]
    private List<GameObject> models = new List<GameObject>();   // Lista de modelos     
    private List<string> modelPaths = new List<string>();       //Lista de rutas de modelos
    private List<string> materialPaths = new List<string>();    // Lista de rutas de materiales
    private GameObject currentModel;                            // Modelo actual
    private SelectTransformGizmo selectTransformGizmo;          // Variable para el script SelectTransformGizmo
    /// <summary>
    /// Inicializa las referencias a componentes y objetos necesarios al iniciar el script.
    /// </summary>
    /// <remarks>
    /// - Encuentra y asigna el componente <see cref="SelectTransformGizmo"/> mediante programacion automáticamente.
    /// - Si no se encuentra el componente, se registra un error en la consola.
    /// - Encuentra y asigna el objeto con la etiqueta 'DeactivatableObject' para su posterior desactivación.
    /// - Si no se encuentra el objeto, se registra una advertencia en la consola.
    /// </remarks>
    private void Start()
    {
        // Encuentra y asigna el SelectTransformGizmo automáticamente
        selectTransformGizmo = FindObjectOfType<SelectTransformGizmo>();
        if (selectTransformGizmo == null)
        {
            Debug.LogError("No se encontró el componente SelectTransformGizmo en la escena.");
        }

        // Encuentra y asigna el objeto para desactivar automáticamente con el tag DeactivatableObject
        objectToDeactivate = GameObject.FindGameObjectWithTag("DeactivatableObject");
        if (objectToDeactivate == null)
        {
            Debug.LogWarning("No se encontró un GameObject con la etiqueta 'DeactivatableObject'.");
        }
    }

    // -------------------------------
    // Métodos
    // -------------------------------
    /// <summary>
    /// Setea el modelo actual y agrega el modelo a la lista
    /// </summary>
    /// <param name="model">El GameObject que representa el modelo que se va a agregar.</param>
    /// <param name="modelPath">La ruta del modelo como una cadena de texto.</param>
    /// <returns>Este método no retorna ningún valor.</returns>
    public void SetModel(GameObject model, string modelPath)
    {
        this.currentModel = model;
        AddModel(model, modelPath);
    }
    /// <summary>
    /// Agrega un modelo a la lista
    /// </summary>
    /// <param name="model">El GameObject que representa el modelo que se va a agregar.</param>
    /// <param name="modelPath">La ruta del modelo como una cadena de texto.</param>
    /// <returns>Este método no retorna ningún valor.</returns>
    public void AddModel(GameObject model, string modelPath)
    {
        models.Add(model);          // Agrega el modelo a la lista
        modelPaths.Add(modelPath);  // Agrega la ruta del modelo a la lista
        materialPaths.Add(null);    // Agrega una ruta de material nula a la lista 
    }
    /// <summary>
    /// Setea la ruta del material para un modelo específico
    /// </summary>
    /// <param name="model">El GameObject que representa el modelo al que se le asignará la ruta del material.</param>
    /// <param name="materialPath">La ruta del material como una cadena de texto.</param>
    /// <returns>Este método no retorna ningún valor.</returns>
    public void SetMaterialPath(GameObject model, string materialPath)
    {
        int index = models.IndexOf(model);
        if (index >= 0)
        {
            materialPaths[index] = materialPath; // Setea la ruta del material
        }
    }
    /// <summary>
    /// Guarda los datos de la escena en un archivo JSON. Este método serializa los datos de los modelos y sus transformaciones,
    /// incluyendo la ruta del modelo y el material asociado, y luego guarda esta información en un archivo JSON.
    /// </summary>
    /// <remarks>
    /// El método verifica si hay modelos en la lista antes de proceder con la guardado. Si no hay modelos, muestra un mensaje de error.
    /// La información se guarda en un archivo cuyo nombre es especificado por el usuario a través de un panel de guardado.
    /// Después de guardar, el método desactiva un GameObject específico si se ha asignado uno para ello.
    /// </remarks>
    /// <returns>Este método no retorna ningún valor.</returns>
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

            modelList.models.Add(data); // Agrega los datos del modelo a la lista
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
    /// <summary>
    /// Carga los datos de la escena desde un archivo JSON. Este método permite al usuario seleccionar un archivo JSON a través de un panel de apertura de archivos.
    /// Una vez seleccionado el archivo, el método lee el contenido del archivo, lo deserializa en un objeto <see cref="ModelList"/> y luego inicia una coroutine
    /// para procesar y cargar los modelos en la escena.
    /// Ademas en el Start de Save system obtiene la referencia de SelectTransformGizmo y en Load llamamos al metodo GetRuntimeTransformGameObject para obtenber 
    /// runtimeTransformGameObj
    /// </summary>
    /// <remarks>
    /// El método usa el "StandaloneFileBrowser" para mostrar un panel que permite al usuario seleccionar un archivo JSON.
    /// Luego, deserializa el contenido del archivo en un objeto "ModelList" utilizando "JsonUtility".
    /// Finalmente, se inicia una coroutine "LoadModelsCoroutine(ModelList)" para cargar los modelos de forma asíncrona.
    /// </remarks>
    /// <returns>Este método no retorna ningún valor.</returns>
    public void Load()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Abrir archivo", "", "json", false);
        if (paths.Length > 0)
        {
            string path = paths[0];                 // Obtiene la ruta del primer archivo seleccionado
            string json = File.ReadAllText(path);   // Lee el archivo JSON

            ModelList modelList = JsonUtility.FromJson<ModelList>(json);    // Deserializa el JSON en un Obj de tipo Model List
            StartCoroutine(LoadModelsCoroutine(modelList));                 // Inicia la corrutina para cargar los modelos delo Model List


            // Recrear/configura el GameObject dinámico y establecer sus propiedades

            // Verifica si selectTransformGizmo es nulo
            if (selectTransformGizmo != null)
            {
                // Obtiene el GameObject dinámico manejado por el SelectTransformGizmo
                GameObject runtimeTransformGameObj = selectTransformGizmo.GetRuntimeTransformGameObject();
                // Verifica si se obtuvo el GameObject dinámico
                if (runtimeTransformGameObj != null) 
                {
                    runtimeTransformGameObj.tag = "DeactivatableObject"; // Asigna la etiqueta "DeactivatableObject"
                    runtimeTransformGameObj.SetActive(false);            // Desactiva el GameObject dinámico
                                                              
                }
                else
                {
                    Debug.LogError("No se pudo obtener el GameObject dinámico.");  // Muestra un mensaje de error si no se pudo obtener el GameObject dinámico
                }
            }
            else
            {
                Debug.LogError("SelectTransformGizmo no está asignado."); // Muestra un mensaje de error si selectTransformGizmo no está asignado
            }
        }
    }
    // -------------------------------
    // Corrutinas
    // -------------------------------
    /// <summary>
    /// Corrutina para cargar los modelos de forma asíncrona
    /// </summary>
    /// <param name="modelList">Lista de datos de modelos a cargar</param>
    /// <returns>Un IEnumerator para permitir la ejecución asíncrona</returns>
    private IEnumerator LoadModelsCoroutine(ModelList modelList)
    {
        foreach (GameObject model in models)
        {
            Destroy(model); // Destruye los modelos actuales
        }
        models.Clear();
        modelPaths.Clear();
        materialPaths.Clear();

        foreach (ModelData data in modelList.models)
        {
            UnityWebRequest www = UnityWebRequest.Get(data.modelPath);
            yield return www.SendWebRequest();                          // Espera a que la solicitud se complete

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("ERROR WWW: " + www.error);
            }
            else
            {
                MemoryStream textStream = new MemoryStream(Encoding.UTF8.GetBytes(www.downloadHandler.text));
                GameObject loadedModel = new OBJLoader().Load(textStream);                                       // Carga el modelo desde el stream de texto

                Transform[] childTransforms = loadedModel.GetComponentsInChildren<Transform>();
                for (int i = 0; i < childTransforms.Length && i < data.transforms.Length; i++)
                {
                    TransformData tData = data.transforms[i];
                    Transform t = childTransforms[i];
                    t.position = tData.position;                // Setea la posición
                    t.rotation = tData.rotation;                // Setea la rotación
                    t.localScale = tData.scale;                 // Setea la escala
                    t.tag = tData.tag;                          // Setea el tag
                    if (tData.hasBoxCollider)
                    {
                        BoxCollider boxCollider = t.gameObject.AddComponent<BoxCollider>();
                        boxCollider.center = tData.boxColliderCenter;                       // Setea el centro del BoxCollider                   
                        boxCollider.size = tData.boxColliderSize;                           // Setea el tamaño del BoxCollider
                    }
                }

                if (!string.IsNullOrEmpty(data.materialPath))
                {
                    UnityWebRequest materialRequest = UnityWebRequestAssetBundle.GetAssetBundle(data.materialPath);
                    yield return materialRequest.SendWebRequest();                                                  // Espera a que la solicitud se complete

                    if (materialRequest.result == UnityWebRequest.Result.Success)
                    {
                        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(materialRequest);
                        string[] assetNames = bundle.GetAllAssetNames();
                        if (assetNames.Length > 0)
                        {
                            Material loadedMaterial = bundle.LoadAsset<Material>(assetNames[0]);
                            SetMaterial(loadedModel, loadedMaterial);                           // Setea el material                        
                        }
                        bundle.Unload(false);
                    }
                    else
                    {
                        SetMaterial(loadedModel, defaultMaterial);  // Setea el material por defecto
                    }
                }
                else
                {
                    SetMaterial(loadedModel, defaultMaterial);  // Setea el material por defecto
                }

                models.Add(loadedModel);                // Agrega el modelo a la lista
                modelPaths.Add(data.modelPath);         // Agrega la ruta del modelo a la lista
                materialPaths.Add(data.materialPath);   // Agrega la ruta del material a la lista
            }
        }
    }
    /// <summary>
    /// Setea el material a un modelo
    /// </summary>
    /// <param name="model">El GameObject al que se le asignará el material</param>
    /// <param name="material">El Material que se asignará al modelo</param>
    /// <remarks>Este método no regresa ningún valor.</remarks>
    private void SetMaterial(GameObject model, Material material)
    {
        foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>())
        {
            renderer.material = material;   // Setea el material en el renderer
        }
    }
    // -------------------------------
    // Métodos de Utilidad
    // -------------------------------
    /// <summary>
    /// Sale del juego
    /// </summary>
    /// <remarks>Este método no tiene parámetros y no regresa ningún valor. Cierra la aplicación y detiene el editor si está en modo de juego.</remarks>
    public void QuitGame()
    {
        Debug.Log("Botón de salir del juego presionado");

        Application.Quit(); // Cierra la aplicación
        // Detiene el editor si está en modo de juego
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;    
#endif
    }
}
