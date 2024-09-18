using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveReport
{
    public int id;
    public string date;
    public string time;
    public List<string> Actions;
    public List<string> Notifications; 
    public List<string> Errors;
}

// CLASE PARA CADA PREFAB
public class NotificationScript : MonoBehavior
{
    public string title;    
    public string message;  
    public float duration;  

    
    public NotificationEntry(string notifTitle, string notifMessage, float notifDuration)
    {
        title = notifTitle;
        message = notifMessage;
        duration = notifDuration;
    }
}