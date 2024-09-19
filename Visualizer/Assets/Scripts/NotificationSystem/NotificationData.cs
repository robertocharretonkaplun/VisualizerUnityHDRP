using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NotificationData : MonoBehaviour
{
    // Referencias a los elementos de texto que mostrar�n el t�tulo y el mensaje de la notificaci�n.
    // Se utilizan los componentes de TextMeshPro para mayor control sobre la calidad del texto.
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text messageText;

    / Duraci�n predeterminada de la notificaci�n(en segundos).
    // Define cu�nto tiempo se mostrar� la notificaci�n antes de desaparecer.
    [SerializeField] private float duration = 3f;

    // M�todo que establece los datos de la notificaci�n: el t�tulo y el mensaje que se mostrar�n.
    // Este m�todo es llamado justo despu�s de que se crea una nueva instancia de notificaci�n.
    public void SetNotificationData(string title, string message)
    {
        //Asigna los valores del titulo y mensaje a los elementos de texto
        titleText.text = title;
        messageText.text = message;

        // Llama a Initialize para que comience el ciclo de tiempo de la notificaci�n
        // (incluyendo su eventual destrucci�n).
        Initialize();
    }

    // M�todo que inicializa la duraci�n de la notificaci�n y comienza
    // la corrutina que la destruir� despu�s del tiempo indicado.
 
    public void Initialize(float customDuration = -1)
    {
        // Si se especifica un valor positivo en customDuration, actualiza
        // la duraci�n de la notificaci�n.
        if (customDuration > 0)
        {
            duration = customDuration;
        }

        // Inicia la corrutina que espera el tiempo determinado antes de
        // destruir la notificaci�n.
        StartCoroutine(DestroyAfterDuration());
    }

    // Corrutina que espera la cantidad de segundos indicada antes de destruir el GameObject
    // de la notificaci�n.
    private IEnumerator DestroyAfterDuration()
    {
        // Espera la cantidad de segundos especificada en la variable 'duration'.
        yield return new WaitForSeconds(duration);
        // Destruye el GameObject de la notificaci�n, removi�ndolo tanto de la jerarqu�a de la
        // escena como de la memoria.
        Destroy(gameObject);
    }
}