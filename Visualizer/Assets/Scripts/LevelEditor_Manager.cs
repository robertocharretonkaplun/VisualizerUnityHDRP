using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditor_Manager : MonoBehaviour
{
    public ItemController[] ItemButtons;
    public GameObject[] ItemPrefabs;
    public int CurrentButtonPressed;
    public float PlaneHeight = 0f; // altura del plano donde se colocan los objetos
    [SerializeField] private CommandManager commandManager;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && ItemButtons[CurrentButtonPressed].Clicked)
        {
            ItemButtons[CurrentButtonPressed].Clicked = false;

            // Crear un rayo desde la camara hasta la posicion del mouse
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Crear un plano en la altura deseada
            Plane plane = new Plane(Vector3.up, new Vector3(0, PlaneHeight, 0));
            float distance;
            if (plane.Raycast(ray, out distance))
            {
                // Calcular la posicion donde se va a instanciar el objeto
                Vector3 spawnPosition = ray.GetPoint(distance);

                // Crear el comando y ejecutarlo a través del CommandManager
                var itemPrefab = ItemPrefabs[CurrentButtonPressed];
                var placeItemCommand = new PlaceItemCommand(itemPrefab, spawnPosition, Quaternion.identity);
                commandManager.ExecuteCommand(placeItemCommand);
            }
        }

        // Check for undo/redo inputs (example: Ctrl+Z for undo, Ctrl+Y for redo)
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
        {
            commandManager.UndoCommand();
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Y))
        {
            commandManager.RedoCommand();
        }
    }
}