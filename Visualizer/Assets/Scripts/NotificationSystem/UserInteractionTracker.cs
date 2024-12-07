using UnityEngine;

/// <summary>
/// Clase que rastrea las interacciones del usuario en la aplicaci�n.
/// Permite registrar acciones realizadas como clics en botones, colocaci�n de objetos,
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
    /// Registra una interacci�n cuando se hace clic en un bot�n.
    /// </summary>
    /// <param name="buttonName">El nombre del bot�n que fue seleccionado.</param>
    public void OnButtonClicked(string buttonName)
    {
        if (!string.IsNullOrEmpty(buttonName))
        {
            // Registra el bot�n seleccionado en el reporte
            saveSystem.SaveActionToReport("Bot�n seleccionado: " + buttonName);
        }
        else
        {
            // Muestra una advertencia si el nombre del bot�n es inv�lido
            UnityEngine.Debug.LogWarning("El nombre del bot�n est� vac�o o es nulo.");
        }
    }

    /// <summary>
    /// Registra una interacci�n cuando se coloca un objeto en el visualizador.
    /// </summary>
    /// <param name="itemID">El identificador �nico del objeto colocado.</param>
    /// <param name="objectName">El nombre del objeto colocado.</param>
    /// <param name="position">La posici�n del objeto en el mundo.</param>
    /// <param name="rotation">La rotaci�n del objeto en el mundo.</param>
    /// <param name="scale">La escala del objeto en el mundo.</param>
    public void OnObjectPlaced(int itemID, string objectName, Vector3 position, Vector3 rotation, Vector3 scale)
    {
        // Construye la informaci�n del objeto
        string objectInfo = $"ID: {itemID}, Nombre: {objectName}";

        // Registra la informaci�n del objeto con gizmos en el reporte
        saveSystem.SaveObjectToReport(objectInfo, position, rotation, scale);
    }

    /// <summary>
    /// Registra una interacci�n cuando se modifica el valor de un campo de entrada.
    /// </summary>
    /// <param name="inputFieldName">El nombre del campo de entrada.</param>
    /// <param name="value">El nuevo valor ingresado en el campo de entrada.</param>
    public void OnInputFieldChanged(string inputFieldName, string value)
    {
        saveSystem.SaveActionToReport("Input modificado: " + inputFieldName + " con valor: " + value);
    }
}


