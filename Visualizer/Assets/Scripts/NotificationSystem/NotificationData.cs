using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationData : MonoBehaviour
{
    public string title;
    public string message;
    public int duration;

    public NotificationData(string title, string message, int duration) 
    {
        this.title = title;
        this.message = message;
        this.duration = duration;
    }
}
