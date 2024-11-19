using UnityEngine;

public class UserInteractionTracker : MonoBehaviour
{
    //Declara Una variable para almacenar una referencia al sistema de guardado.
    private SaveSystemN saveSystem;

    //Método que se llama cuando el script inicia.
    private void Start()
    {
        //Busca una instacia del tipo SaveSystemN en la escena y la asigna saveSystem.
        //Esto permite acceder a los metodos de SaveSystemN desde este script.
        saveSystem = FindObjectOfType<SaveSystemN>();
    }

    public void OnButtonClicked(string buttonName)
    {
        // Asegurarse de que el nombre del botón se envía correctamente
        if (!string.IsNullOrEmpty(buttonName))
        {
            //Si el nombre es valido, guarda la acción en el reporte llamanso al método SaveActionToReport.
            //en saveSystem, indicando que el botón fue seleccionado.
            saveSystem.SaveActionToReport("Botón seleccionado: " + buttonName);
        }
        else
        {
            // Si el nombre es nulo o vacío, muestra una advertencia en la consola.
            UnityEngine.Debug.LogWarning("El nombre del botón está vacío o es nulo.");
        }
    }

    // Método que se llama cuando se coloca un objeto en la escena.
    public void OnObjectPlaced(string objectName, Vector3 position)
    {
        // Guarda la acción en el reporte indicando que se ha colocado un objeto,
        // incluyendo el nombre del objeto y su posición.
        saveSystem.SaveActionToReport("Objeto colocado: " + objectName + " en posición: " + position);
    }

    // Método que se llama cuando el usuario cambia el valor en un campo de entrada.
    public void OnInputFieldChanged(string inputFieldName, string value)
    {
        // Guarda la acción en el reporte indicando que se ha modificado un campo de entrada,
        // incluyendo el nombre del campo y el valor nuevo ingresado.
        saveSystem.SaveActionToReport("Input modificado: " + inputFieldName + " con valor: " + value);
    }
}


