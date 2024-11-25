using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditor_Manager : MonoBehaviour
{
    // Singleton instance
    public static LevelEditor_Manager Instance { get; private set; }

    // Public variables
    public ItemController[] ItemButtons;
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

                // Registrar el objeto colocado en el reporte
                string objectName = itemPrefab.name; // Usamos el nombre del prefab como nombre del objeto
                UserInteractionTracker userInteractionTracker = FindObjectOfType<UserInteractionTracker>();
                if (userInteractionTracker != null)
                {
                    userInteractionTracker.OnObjectPlaced(CurrentButtonPressed, objectName); // Pasamos el ID y nombre del objeto
                }
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

    /// <summary>
    /// Spawnea un objeto en la posición dada basado en su itemID.
    /// </summary>
    /// <param name="itemID">El ID del item a spawnear.</param>
    /// <param name="worldPosition">La posición en el mundo donde spawnear el objeto.</param>
    public void SpawnItemAtPosition(int itemID, Vector3 worldPosition)
    {
        if (itemID < 0 || itemID >= ItemPrefabs.Length)
        {
            Debug.LogWarning("ID de item inválido: " + itemID);
            return;
        }

        // Obtener el prefab y su rotación
        var itemPrefab = ItemPrefabs[itemID];
        Quaternion spawnRotation = itemPrefab.transform.rotation;

        // Ajustar la posición en Y del objeto basado en el prefab
        worldPosition.y = itemPrefab.transform.position.y;

        // Crear el comando y ejecutarlo a través del CommandManager
        var placeItemCommand = new PlaceItemCommand(itemPrefab, worldPosition, spawnRotation);
        commandManager.ExecuteCommand(placeItemCommand);
    }
}