using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class NotificationSystem : MonoBehaviour
{
    // Instancia �nica para permitir acceso global al sistema de notificaciones.
    public static NotificationSystem instance;

    // Prefab de la notificaci�n que se mostrar�.
    [SerializeField] private GameObject notificationPrefab;

    // Contenedor donde se organizar�n las notificaciones (un objeto vac�o en la UI).
    [SerializeField] private Transform notificationParent;

    private void Awake()
    {
        // Establece esta clase como la instancia �nica si no hay otra instancia activa.
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // M�todo para crear una nueva notificaci�n.
    public void CreateNotification(string title, string message, float duration = 3f)
    {
        // Instancia una nueva notificaci�n a partir del prefab y la coloca en el contenedor.
        GameObject newNotification = Instantiate(notificationPrefab, notificationParent);

        // Establece los datos de la notificaci�n (t�tulo y mensaje).
        NotificationData notificationData = newNotification.GetComponent<NotificationData>();
        notificationData.SetNotificationData(title, message);

        // Muestra la notificaci�n cambiando la visibilidad con CanvasGroup.
        CanvasGroup canvasGroup = newNotification.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1;  // Hace visible la notificaci�n.
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        // Inicia la corrutina para destruir la notificaci�n despu�s de la duraci�n.
        StartCoroutine(DestroyAfterDuration(newNotification, duration));
    }

    // Corrutina para destruir la notificaci�n despu�s de un tiempo especificado.
    private IEnumerator DestroyAfterDuration(GameObject notification, float duration)
    {
        // Espera la cantidad de segundos definida en 'duration'.
        yield return new WaitForSeconds(duration);

        // Destruye la notificaci�n despu�s de que pase el tiempo.
        Destroy(notification);
    }
}
