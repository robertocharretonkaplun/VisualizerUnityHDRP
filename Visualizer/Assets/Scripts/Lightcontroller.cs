using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Controlador de items de iluminación. Maneja el spawneo de objetos de iluminación mediante los botones de la interfaz.
/// </summary>
public class Lightcontroller : MonoBehaviour
{
    public int itemID; // ID del item 
     public int itemAmount; // Cantidad de items
    public TextMeshProUGUI itemAmountText; // Texto de la cantidad de items
    public bool Clicked = false; // Si el item fue clickeado
    private LightEditorM LightEditor_Manager; // Manager del editor de niveles
    /// <summary>
    /// Inicializa referencias 
    /// </summary>
    void Start()
    {
        itemAmountText.text = itemAmount.ToString();
        // Referecia al manager del editor de niveles
        LightEditor_Manager = LightEditorM.Instance;
    }
    /// <summary>
    /// Maneja la lógica cuando se clickea el botón del item.
    /// </summary>
    public void ButtonClick()
    {
        if(itemAmount > 0)
        {
        Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z); // Posición del mouse
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);  // Posición del mouse en el mundo
        Clicked = true; // El item fue clickeado
        itemAmount--;
        itemAmountText.text = itemAmount.ToString();
        LightEditor_Manager.CurrentButtonPressed = itemID; // Setea el item actual
        }
    }
}
