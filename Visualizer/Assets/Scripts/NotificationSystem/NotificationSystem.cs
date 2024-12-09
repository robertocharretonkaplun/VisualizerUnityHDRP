using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class NotificationSystem : MonoBehaviour
{
    // Instancia única para permitir acceso global al sistema de notificaciones.
    public static NotificationSystem instance;

    // Prefab de la notificación que se mostrará.
    [SerializeField] private GameObject notificationPrefab;

    // Contenedor donde se organizarán las notificaciones (un objeto vacío en la UI).
    [SerializeField] private Transform notificationParent;

    private void Awake()
    {
        // Establece esta clase como la instancia única si no hay otra instancia activa.
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Método para crear una nueva notificación.
    public void CreateNotification(string title, string message, float duration = 3f)
    {
        // Instancia una nueva notificación a partir del prefab y la coloca en el contenedor.
        GameObject newNotification = Instantiate(notificationPrefab, notificationParent);

        // Establece los datos de la notificación (título y mensaje).
        NotificationData notificationData = newNotification.GetComponent<NotificationData>();
        notificationData.SetNotificationData(title, message);

        // Muestra la notificación cambiando la visibilidad con CanvasGroup.
        CanvasGroup canvasGroup = newNotification.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1;  // Hace visible la notificación.
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        // Inicia la corrutina para destruir la notificación después de la duración.
        StartCoroutine(DestroyAfterDuration(newNotification, duration));
    }

    // Corrutina para destruir la notificación después de un tiempo especificado.
    private IEnumerator DestroyAfterDuration(GameObject notification, float duration)
    {
        // Espera la cantidad de segundos definida en 'duration'.
        yield return new WaitForSeconds(duration);

        // Destruye la notificación después de que pase el tiempo.
        Destroy(notification);
    }
}
