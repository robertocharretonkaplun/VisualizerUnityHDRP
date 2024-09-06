using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveReport : MonoBehaviour
{
    public List<NotificationData> notificationHistory = new List<NotificationData>();

    public void AddNotification(NotificationData notification)
    {
        notificationHistory.Add(notification);
    }
}
