using UnityEngine;
using UnityEngine.UI;

public class ButtonListener : MonoBehaviour
{
    public Button yourButton;
    public UserInteractionTracker interactionTracker;

    private void Start()
    {
        // Añade el listener al botón
        yourButton.onClick.AddListener(() => interactionTracker.OnButtonClicked(yourButton.name));
    }
}
