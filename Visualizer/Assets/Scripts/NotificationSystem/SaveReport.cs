using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveReport
{
    public List<NotificationEntry> notifications;  

    public SaveReport()
    {
        notifications = new List<NotificationEntry>();
    }
}

[Serializable]
public class NotificationEntry
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