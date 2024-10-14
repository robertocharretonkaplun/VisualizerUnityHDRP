using UnityEngine;
using Unity.Formats.USD;

public class USD : MonoBehaviour
{
    public string usdFilePath;
    public bool forceRebuild = true; // Forzar la reconstrucci�n

    void Start()
    {
        if (!string.IsNullOrEmpty(usdFilePath))
        {
            // Crear un nuevo GameObject para cargar el archivo USD
            GameObject usdRoot = new GameObject("USD GameObject");

            // A�adir el componente UsdAsset para cargar el archivo USD
            var usdAsset = usdRoot.AddComponent<UsdAsset>();
            usdAsset.usdFullPath = usdFilePath;

            // Importar el archivo USD
            usdAsset.Reload(forceRebuild);

            // Ajustar la posici�n del objeto importado
            usdRoot.transform.SetParent(transform);
            Debug.Log("Archivo USD importado con �xito.");
        }
        else
        {
            Debug.LogError("Ruta de archivo USD no v�lida o archivo no encontrado.");
        }
    }
}
