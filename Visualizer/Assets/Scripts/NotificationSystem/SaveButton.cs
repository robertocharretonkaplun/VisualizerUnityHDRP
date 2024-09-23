using UnityEngine;
using UnityEngine.UI;

public class SaveButton : MonoBehaviour
{
    [SerializeField] private Button saveButton;
    [SerializeField] private NotificationSystem notificationSystem;  
    [SerializeField] private SaveSystemN saveSystem; 

    private void Start()
    {
        // Agregar listener para que solo cuando se haga click se muestre la notificación
        saveButton.onClick.AddListener(OnSaveButtonClick);
    }

    private void OnSaveButtonClick()
    {
        saveSystem.CreateReport(); // Crear el reporte
        saveSystem.SaveReportToFile(); // Guardar el reporte
        notificationSystem.CreateNotification("Reporte guardado", "Se ha guardado el reporte correctamente.", 3.0f);  // Mostrar notificación al guardar
    }
}
