using UnityEngine;

public class UserInteractionTracker : MonoBehaviour
{
    private SaveSystemN saveSystem;

    private void Start()
    {
        saveSystem = FindObjectOfType<SaveSystemN>();
    }

    public void OnButtonClicked(string buttonName)
    {
        saveSystem.SaveActionToReport("Bot�n seleccionado: " + buttonName);
    }

    public void OnObjectPlaced(string objectName, Vector3 position)
    {
        saveSystem.SaveActionToReport("Objeto colocado: " + objectName + " en posici�n: " + position);
    }

    public void OnInputFieldChanged(string inputFieldName, string value)
    {
        saveSystem.SaveActionToReport("Input modificado: " + inputFieldName + " con valor: " + value);
    }
}
