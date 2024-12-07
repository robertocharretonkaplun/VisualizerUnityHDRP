using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Sistema de guardado que maneja la creación y almacenamiento de reportes en formato JSON.
/// </summary>
public class SaveSystemN : MonoBehaviour
{
    // Instancia del reporte que se va a guardar
    private SaveReport saveReport;

    // Nombre del archivo y ruta donde se guardará
    private string FileName;
    private string filePath;

    /// <summary>
    /// Inicializa las variables necesarias al cargar el script.
    /// </summary>
    private void Awake()
    {
        // Se crea una nueva instancia del reporte
        saveReport = new SaveReport();
    }

    /// <summary>
    /// Agrega una notificación al reporte.
    /// </summary>
    /// <param name="title">Título de la notificación.</param>
    /// <param name="message">Mensaje de la notificación.</param>
    /// <param name="duration">Duración de la notificación en segundos.</param>
    public void AddNotificationToReport(string title, string message, float duration)
    {
        // Crea una nueva entrada de notificación y la agrega al reporte
        NotificationEntry entry = new NotificationEntry(title, message, duration);
        saveReport.Notifications.Add(entry.title + ": " + entry.message);
    }

    /// <summary>
    /// Agrega información de un objeto al reporte, incluyendo gizmos.
    /// </summary>
    /// <param name="objectInfo">Descripción del objeto, por ejemplo, su nombre o ID.</param>
    /// <param name="position">Posición del objeto en el mundo.</param>
    /// <param name="rotation">Rotación del objeto en el mundo.</param>
    /// <param name="scale">Escala del objeto en el mundo.</param>
    public void SaveObjectToReport(string objectInfo, Vector3 position, Vector3 rotation, Vector3 scale)
    {
        string gizmoData = $"Pos: {position.x}, {position.y}, {position.z}, " +
                           $"Rot: {rotation.x}, {rotation.y}, {rotation.z}, " +
                           $"Sca: {scale.x}, {scale.y}, {scale.z}";

        // Agrega la información del objeto y sus gizmos al reporte
        saveReport.Objects.Add($"{objectInfo}, Gizmos: {gizmoData}");
    }

    /// <summary>
    /// Guarda una acción realizada en el reporte.
    /// </summary>
    /// <param name="actionDescription">Descripción de la acción realizada.</param>
    public void SaveActionToReport(string actionDescription)
    {
        //saveReport.Actions.Add(actionDescription);
    }

    /// <summary>
    /// Inicializa un nuevo reporte asignándole un ID único, fecha y hora.
    /// </summary>
    public void InitializeNewReport()
    {
        // Genera un nuevo ID único para el reporte incrementando el valor almacenado
        int currentId = PlayerPrefs.GetInt("id", 0) + 1;
        saveReport.id = currentId;

        // Actualiza el ID en PlayerPrefs para la próxima ejecución
        PlayerPrefs.SetInt("id", currentId);
        PlayerPrefs.Save();

        // Asigna la fecha y hora actuales en formato UTC
        saveReport.date = DateTime.UtcNow.ToString("yyyy-MM-dd");
        saveReport.time = DateTime.UtcNow.ToString("HH:mm:ss");
    }

    /// <summary>
    /// Guarda el reporte en un archivo JSON en la carpeta "Reports".
    /// </summary>

    public void SaveReportToFile()
    {
        // Inicializa un nuevo reporte con información básica
        InitializeNewReport();

        // Genera el nombre del archivo basado en el ID y la fecha del reporte
        string newFileName = "Report_" + saveReport.id + "_" + saveReport.date + ".json";
        FileName = newFileName;

        // Define la ruta de la carpeta "Reports" dentro del directorio del proyecto
        string Folder = "/Reports/";
        string folderPath = UnityEngine.Application.dataPath + Folder;

        // Crea la carpeta si no existe
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Define la ruta completa del archivo
        filePath = folderPath + newFileName;

        // Convierte el reporte a formato JSON con un formato legible
        string json = JsonUtility.ToJson(saveReport, true);

        try
        {
            // Escribe el JSON en el archivo
            File.WriteAllText(filePath, json);
            UnityEngine.Debug.Log("Reporte guardado en: " + filePath);
        }
        catch (Exception e)
        {
            // Maneja errores al guardar el archivo
            UnityEngine.Debug.LogError("Error al guardar el reporte: " + e.Message);
        }
    }
}
