using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NotificationData : MonoBehaviour
{
    // Referencias a los elementos de texto que mostrarán el título y el mensaje de la notificación.
    // Se utilizan los componentes de TextMeshPro para mayor control sobre la calidad del texto.
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text messageText;

    / Duración predeterminada de la notificación(en segundos).
    // Define cuánto tiempo se mostrará la notificación antes de desaparecer.
    [SerializeField] private float duration = 3f;

    // Método que establece los datos de la notificación: el título y el mensaje que se mostrarán.
    // Este método es llamado justo después de que se crea una nueva instancia de notificación.
    public void SetNotificationData(string title, string message)
    {
        //Asigna los valores del titulo y mensaje a los elementos de texto
        titleText.text = title;
        messageText.text = message;

        // Llama a Initialize para que comience el ciclo de tiempo de la notificación
        // (incluyendo su eventual destrucción).
        Initialize();
    }

    // Método que inicializa la duración de la notificación y comienza
    // la corrutina que la destruirá después del tiempo indicado.
 
    public void Initialize(float customDuration = -1)
    {
        // Si se especifica un valor positivo en customDuration, actualiza
        // la duración de la notificación.
        if (customDuration > 0)
        {
            duration = customDuration;
        }

        // Inicia la corrutina que espera el tiempo determinado antes de
        // destruir la notificación.
        StartCoroutine(DestroyAfterDuration());
    }

    // Corrutina que espera la cantidad de segundos indicada antes de destruir el GameObject
    // de la notificación.
    private IEnumerator DestroyAfterDuration()
    {
        // Espera la cantidad de segundos especificada en la variable 'duration'.
        yield return new WaitForSeconds(duration);
        // Destruye el GameObject de la notificación, removiéndolo tanto de la jerarquía de la
        // escena como de la memoria.
        Destroy(gameObject);
    }
}