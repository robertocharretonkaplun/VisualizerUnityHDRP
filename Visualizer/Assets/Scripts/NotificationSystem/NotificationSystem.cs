using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class NotificationSystem : MonoBehaviour
{
    public GameObject notificationPrefab; 
    public Transform notificationParent;

    public void CreateNotification(NotificationData notificationData)
    {
        GameObject newNotification = Instantiate(notificationPrefab, notificationParent);

        Text titleText = newNotification.transform.Find("Titulo").GetComponent<Text>();
        Text messageText = newNotification.transform.Find("Mensaje").GetComponent<Text>();
        titleText.text = notificationData.title;
        messageText.text = notificationData.message;

        StartCoroutine(DestroyNotificationAfterDuration(newNotification, notificationData.duration));
    }

    private IEnumerator DestroyNotificationAfterDuration(GameObject notification, int duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(notification);
    }
}
