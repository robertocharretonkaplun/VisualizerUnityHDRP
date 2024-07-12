using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ItemController : MonoBehaviour
{
    public int itemID; // ID del item 
    
    
    public bool Clicked = false; // Si el item fue clickeado
    private LevelEditor_Manager levelEditor_Manager; // Manager del editor de niveles
    void Start()
    {
        // Referecia al texto de la cantidad de items y al manager del editor de niveles
       
        levelEditor_Manager = GameObject.FindGameObjectWithTag("LevelEditor_Manager").GetComponent<LevelEditor_Manager>();
    }
    public void ButtonClick()
    {
        // Si la cantidad de items es mayor a 0, se activa el click
        
        Vector3 screenPosition=new Vector3(Input.mousePosition.x,Input.mousePosition.y, Input.mousePosition.z);
        Vector3 worldPosition=Camera.main.ScreenToWorldPoint(screenPosition);    
        Clicked = true;
        
        levelEditor_Manager.CurrentButtonPressed = itemID;
        
    }
    
}
