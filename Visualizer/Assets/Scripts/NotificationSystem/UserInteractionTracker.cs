using UnityEngine;

public class UserInteractionTracker : MonoBehaviour
{
    //Declara Una variable para almacenar una referencia al sistema de guardado.
    private SaveSystemN saveSystem;

    //M�todo que se llama cuando el script inicia.
    private void Start()
    {
        //Busca una instacia del tipo SaveSystemN en la escena y la asigna saveSystem.
        //Esto permite acceder a los metodos de SaveSystemN desde este script.
        saveSystem = FindObjectOfType<SaveSystemN>();
    }

    public void OnButtonClicked(string buttonName)
    {
        // Asegurarse de que el nombre del bot�n se env�a correctamente
        if (!string.IsNullOrEmpty(buttonName))
        {
            //Si el nombre es valido, guarda la acci�n en el reporte llamanso al m�todo SaveActionToReport.
            //en saveSystem, indicando que el bot�n fue seleccionado.
            saveSystem.SaveActionToReport("Bot�n seleccionado: " + buttonName);
        }
        else
        {
            // Si el nombre es nulo o vac�o, muestra una advertencia en la consola.
            UnityEngine.Debug.LogWarning("El nombre del bot�n est� vac�o o es nulo.");
        }
    }

    // M�todo que se llama cuando se coloca un objeto en la escena.
    public void OnObjectPlaced(string objectName, Vector3 position)
    {
        // Guarda la acci�n en el reporte indicando que se ha colocado un objeto,
        // incluyendo el nombre del objeto y su posici�n.
        saveSystem.SaveActionToReport("Objeto colocado: " + objectName + " en posici�n: " + position);
    }

    // M�todo que se llama cuando el usuario cambia el valor en un campo de entrada.
    public void OnInputFieldChanged(string inputFieldName, string value)
    {
        // Guarda la acci�n en el reporte indicando que se ha modificado un campo de entrada,
        // incluyendo el nombre del campo y el valor nuevo ingresado.
        saveSystem.SaveActionToReport("Input modificado: " + inputFieldName + " con valor: " + value);
    }
}


