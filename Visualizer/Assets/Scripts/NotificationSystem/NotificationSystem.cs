using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class NotificationSystem : MonoBehaviour
{
    public static NotificationSystem instance;
    
    [SerializeField] private GameObject notificationPrefab;
    [SerializeField] private Transform notificationParent;

    public void CreateNotification(string title, string message, float duration = 3f)
    {
        GameObject newNotification = Instantiate(notificationPrefab, notificationParent);
        newNotification.GetComponent<NotificationData>().SetNotificationData(title, message);
    }
}
