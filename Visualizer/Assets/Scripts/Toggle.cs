using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Toggle: MonoBehaviour
{
    // Assign these in the Inspector
    public GameObject object1;
    public GameObject object2;
    public TMP_Text statusText;
    public Button toggleButton;

    private bool isObject1Active = true;

    void Start()
    {
        // Actualizar objetos y texto
        UpdateGameObjects();
        UpdateStatusText();

        // Asignar el método OnToggleButtonClick al evento onClick del botón
        toggleButton.onClick.AddListener(OnToggleButtonClick);
    }

    void OnToggleButtonClick()
    {
        //  Cambiar el estado de los objetos
        isObject1Active = !isObject1Active;

        // Actualizar objetos y texto
        UpdateGameObjects();
        UpdateStatusText();
    }

    void UpdateGameObjects()
    {
        object1.SetActive(isObject1Active);
        object2.SetActive(!isObject1Active);
    }

    void UpdateStatusText()
    {
        if (isObject1Active)
        {
            statusText.text = "Gizmo";
        }
        else
        {
            statusText.text = "UI";
        }
    }
}
