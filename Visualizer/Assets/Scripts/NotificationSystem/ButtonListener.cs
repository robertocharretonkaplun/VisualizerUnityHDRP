using UnityEngine;
using UnityEngine.UI;

public class ButtonListener : MonoBehaviour
{
    public Button yourButton;
    public UserInteractionTracker interactionTracker;

    private void Start()
    {
        // A�ade el listener al bot�n
        yourButton.onClick.AddListener(() => interactionTracker.OnButtonClicked(yourButton.name));
    }
}
