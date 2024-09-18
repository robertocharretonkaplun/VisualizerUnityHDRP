using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NotificationData : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;  
    [SerializeField] private TMP_Text messageText; 
    [SerializeField] private float duration = 3f;

    public void SetNotificationData(string title, string message)
    {
        titleText.text = title;
        messageText.text = message;

        initialized();
    }

    public void Initialize(float customDuration = -1)
    {
        if (customDuration > 0)
        {
            duration = customDuration;
        }

        StartCoroutine(DestroyAfterDuration());
    }

    private IEnumerator DestroyAfterDuration()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);  
    }
}
