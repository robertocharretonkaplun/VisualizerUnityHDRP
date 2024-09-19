using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class NotificationSystem : MonoBehaviour
{
    //Esta variable es para mantener una instancia de NotificationSystem.
    //Permite acceder a este sistema desde cualquier parte del codigo sintener
    // que asignarlo manualmente.
    public static NotificationSystem instance;

    //Prefab que mostrará la notificacion
    [SerializeField] private GameObject notificationPrefab;
    //Ayudara a organizar las notificaciones 
    [SerializeField] private Transform notificationParent;

    //Funcion que crea una nueva notificacion
    public void CreateNotification(string title, string message, float duration = 3f)
    {
        //Una nueva notificación usando el prefab y la coloca dentro del contenedor de
        //notificaciones.
        GameObject newNotification = Instantiate(notificationPrefab, notificationParent);
        //Establece los datos de la notificación (titulo y mensaje)
        newNotification.GetComponent<NotificationData>().SetNotificationData(title, message);
    }
}