using UnityEngine;

/// <summary>
/// Clase que rastrea las interacciones del usuario en la aplicación.
/// Permite registrar acciones realizadas como clics en botones, colocación de objetos,
/// y cambios en campos de entrada en el reporte generado.
/// </summary>

public class UserInteractionTracker : MonoBehaviour
{
    // Referencia al sistema de guardado para registrar interacciones
    private SaveSystemN saveSystem;

    /// <summary>
    /// Inicializa el script buscando el sistema de guardado en la escena.
    /// </summary>
    private void Start()
    {
        saveSystem = FindObjectOfType<SaveSystemN>();
    }

    /// <summary>
    /// Registra una interacción cuando se hace clic en un botón.
    /// </summary>
    /// <param name="buttonName">El nombre del botón que fue seleccionado.</param>
    public void OnButtonClicked(string buttonName)
    {
        if (!string.IsNullOrEmpty(buttonName))
        {
            // Registra el botón seleccionado en el reporte
            saveSystem.SaveActionToReport("Botón seleccionado: " + buttonName);
        }
        else
        {
            // Muestra una advertencia si el nombre del botón es inválido
            UnityEngine.Debug.LogWarning("El nombre del botón está vacío o es nulo.");
        }
    }

    /// <summary>
    /// Registra una interacción cuando se coloca un objeto en el visualizador.
    /// </summary>
    /// <param name="itemID">El identificador único del objeto colocado.</param>
    /// <param name="objectName">El nombre del objeto colocado.</param>
    /// <param name="position">La posición del objeto en el mundo.</param>
    /// <param name="rotation">La rotación del objeto en el mundo.</param>
    /// <param name="scale">La escala del objeto en el mundo.</param>
    public void OnObjectPlaced(int itemID, string objectName, Vector3 position, Vector3 rotation, Vector3 scale)
    {
        // Construye la información del objeto
        string objectInfo = $"ID: {itemID}, Nombre: {objectName}";

        // Registra la información del objeto con gizmos en el reporte
        saveSystem.SaveObjectToReport(objectInfo, position, rotation, scale);
    }

    /// <summary>
    /// Registra una interacción cuando se modifica el valor de un campo de entrada.
    /// </summary>
    /// <param name="inputFieldName">El nombre del campo de entrada.</param>
    /// <param name="value">El nuevo valor ingresado en el campo de entrada.</param>
    public void OnInputFieldChanged(string inputFieldName, string value)
    {
        saveSystem.SaveActionToReport("Input modificado: " + inputFieldName + " con valor: " + value);
    }
}


