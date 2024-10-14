using UnityEngine;
using Unity.Formats.USD;

public class USD : MonoBehaviour
{
    public string usdFilePath;
    public bool forceRebuild = true; // Forzar la reconstrucción

    void Start()
    {
        if (!string.IsNullOrEmpty(usdFilePath))
        {
            // Crear un nuevo GameObject para cargar el archivo USD
            GameObject usdRoot = new GameObject("USD GameObject");

            // Añadir el componente UsdAsset para cargar el archivo USD
            var usdAsset = usdRoot.AddComponent<UsdAsset>();
            usdAsset.usdFullPath = usdFilePath;

            // Importar el archivo USD
            usdAsset.Reload(forceRebuild);

            // Ajustar la posición del objeto importado
            usdRoot.transform.SetParent(transform);
            Debug.Log("Archivo USD importado con éxito.");
        }
        else
        {
            Debug.LogError("Ruta de archivo USD no válida o archivo no encontrado.");
        }
    }
}
