using UnityEngine;

public class UserInteractionTracker : MonoBehaviour
{
    private SaveSystemN saveSystem;

    private void Start()
    {
        // Inicializa SaveSystemN
        saveSystem = FindObjectOfType<SaveSystemN>();

        // Crea un nuevo reporte al iniciar
        saveSystem.CreateReport();
    }

    public void OnButtonClicked(string buttonName)
    {
        // Registra la acci�n del bot�n presionado en el reporte
        saveSystem.SaveActionToReport("Bot�n seleccionado: " + buttonName);
    }

    public void OnObjectPlaced(string objectName, Vector3 position)
    {
        // Registra la colocaci�n del objeto y su posici�n
        saveSystem.SaveActionToReport("Objeto colocado: " + objectName + " en posici�n: " + position);
    }

    public void OnInputFieldChanged(string inputFieldName, string value)
    {
        // Registra el cambio en el campo de texto
        saveSystem.SaveActionToReport("Input modificado: " + inputFieldName + " con valor: " + value);
    }
}
