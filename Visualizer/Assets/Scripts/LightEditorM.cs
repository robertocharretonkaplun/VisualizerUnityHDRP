
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Controlador de items de iluminación. Maneja el spawneo de objetos de iluminación mediante los botones de la interfaz.
/// </summary>
public class LightEditorM : MonoBehaviour
{
    // Singleton instance
    public static LightEditorM Instance { get; private set; }

    // Public variables
    public Lightcontroller[] ItemButtons;
    public GameObject[] ItemPrefabs;
    public int CurrentButtonPressed;
    public float PlaneHeight = 0f; // altura del plano donde se colocan los objetos
    
    // Private variables
    [SerializeField] private CommandManager commandManager;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Implementing the singleton pattern
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
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

                // Obtener el prefab y su rotación
                var itemPrefab = ItemPrefabs[CurrentButtonPressed];
                Quaternion spawnRotation = itemPrefab.transform.rotation;

                // Ajustar la posición en Y del objeto basado en el prefab
                spawnPosition.y = itemPrefab.transform.position.y;

                // Crear el comando y ejecutarlo a través del CommandManager
                var placeItemCommand = new PlaceItemCommand(itemPrefab, spawnPosition, spawnRotation);
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
