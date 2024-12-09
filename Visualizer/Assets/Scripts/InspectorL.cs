using UnityEngine;
using RuntimeInspectorNamespace; // Ensure you are using the correct namespace

public class InspectorL : MonoBehaviour
{
    // Reference to the Runtime Hierarchy
    public RuntimeHierarchy runtimeHierarchy;

    // Reference to the GameObject you want to activate
    public GameObject targetGameObject;

    private void OnEnable()
    {
        // Subscribe to the OnSelectionChanged event with the correct delegate signature
      
    }

    private void OnDisable()
    {
        // Unsubscribe from the OnSelectionChanged event
        
     
    }

    // This method is triggered when the selection changes in the hierarchy
    private void OnSelectionChanged(Object selectedObject)
    {
        // Check if the selected object is a GameObject
        if (selectedObject is GameObject)
        {
            // Activate the target GameObject when a valid GameObject is selected
            targetGameObject.SetActive(true);
        }
        else
        {
            // Deactivate the target GameObject if the selection is invalid or null
            targetGameObject.SetActive(false);
        }
    }
}