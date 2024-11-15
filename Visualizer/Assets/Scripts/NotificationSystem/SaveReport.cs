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
    public List<string> Actions = new List<string>();
    public List<string> Objects = new List<string>();
    public List<string> Notifications = new List<string>();
    public List<string> Errors = new List<string>();
}


// Clase para cada Prefab
public class NotificationEntry
{
    public string title;
    public string message;
    public float duration;

    // Constructor de la clase NotificationEntry
    public NotificationEntry(string notifTitle, string notifMessage, float notifDuration)
    {
        title = notifTitle;
        message = notifMessage;
        duration = notifDuration;
    }
}
