using UnityEngine;

public class UserInteractionTracker : MonoBehaviour
{
    private SaveSystemN saveSystem;

    private void Start()
    {
        //Busca una instacia del tipo SaveSystemN en la escena y la asigna saveSystem.
        //Esto permite acceder a los metodos de SaveSystemN desde este script.
        saveSystem = FindObjectOfType<SaveSystemN>();
    }

    public void OnButtonClicked(string buttonName)
    {
        // Asegurarse de que el nombre del bot?n se env?a correctamente
        if (!string.IsNullOrEmpty(buttonName))
        {
            //Si el nombre es valido, guarda la acci?n en el reporte llamanso al m?todo SaveActionToReport.
            //en saveSystem, indicando que el bot?n fue seleccionado.
            saveSystem.SaveActionToReport("Botón seleccionado: " + buttonName);
        }
        else
        {
            // Si el nombre es nulo o vac?o, muestra una advertencia en la consola.
            UnityEngine.Debug.LogWarning("El nombre del botón está vacío o es nulo.");
        }
    }

    // Método para registrar los objetos colocados en el visualizador
    public void OnObjectPlaced(int itemID, string objectName)
    {
        // Agregar el objeto con su ID y nombre al reporte
        string objectInfo = $"ID: {itemID}, Nombre: {objectName}";
        saveSystem.SaveObjectToReport(objectInfo);
    }

    public void OnInputFieldChanged(string inputFieldName, string value)
    {
        saveSystem.SaveActionToReport("Input modificado: " + inputFieldName + " con valor: " + value);
    }
}


