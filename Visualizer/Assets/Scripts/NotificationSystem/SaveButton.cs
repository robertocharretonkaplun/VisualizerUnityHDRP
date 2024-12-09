using UnityEngine;
using UnityEngine.UI;

public class SaveButton : MonoBehaviour
{
    [SerializeField] private Button saveButton;
    [SerializeField] private NotificationSystem notificationSystem;
    [SerializeField] private SaveSystemN saveSystem;

    private void Start()
    {
        saveButton.onClick.AddListener(OnSaveButtonClick);
    }

    private void OnSaveButtonClick()
    {
        saveSystem.SaveReportToFile();
        notificationSystem.CreateNotification("Reporte guardado", "Se ha guardado el reporte correctamente.", 3.0f);
    }
}
