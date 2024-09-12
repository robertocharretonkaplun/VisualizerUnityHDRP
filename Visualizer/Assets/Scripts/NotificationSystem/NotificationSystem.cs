using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class NotificationSystem : MonoBehaviour
{
    [SerializeField] private GameObject notificationPrefab;
    [SerializeField] private Transform notificationParent;

    public void CreateNotification(string title, string message, float duration = 3f)
    {
        
        GameObject newNotification = Instantiate(notificationPrefab, notificationParent);

        
        NotificationData notificationData = newNotification.GetComponent<NotificationData>();

       
        notificationData.SetNotificationData(title, message);
        notificationData.Initialize(duration);  
    }
}
