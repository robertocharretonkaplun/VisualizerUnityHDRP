using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Controlador de items. Maneja el spawneo de objetos mediante los botones de la interfaz.
/// </summary>
public class ItemController : MonoBehaviour
{
    public int itemID; // ID del item 
    public bool Clicked = false; // Si el item fue clickeado
    private LevelEditor_Manager levelEditor_Manager; // Manager del editor de niveles
    /// <summary>
    /// Inicializa referencias 
    /// </summary>
    void Start()
    {
        // Referecia al manager del editor de niveles
        levelEditor_Manager = LevelEditor_Manager.Instance;
    }
    /// <summary>
    /// Maneja la lógica cuando se clickea el botón del item.
    /// </summary>
    public void ButtonClick()
    {
        Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z); // Posición del mouse
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);  // Posición del mouse en el mundo
        Clicked = true; // El item fue clickeado
        
        levelEditor_Manager.CurrentButtonPressed = itemID; // Setea el item actual
    }
}
