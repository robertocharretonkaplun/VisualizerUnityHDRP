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
    /// <param name="duration">Duración de la notificación.</param>
    public void AddNotificationToReport(string title, string message, float duration)
    {
        // Se crea una nueva entrada de notificación
        NotificationEntry entry = new NotificationEntry(title, message, duration);
        // Se agrega al reporte en formato "Título: Mensaje"
        saveReport.Notifications.Add(entry.title + ": " + entry.message);
    }

    /// <summary>
    /// Método reservado para guardar acciones en el reporte.
    /// </summary>
    /// <param name="action">Descripción de la acción.</param>

    public void SaveActionToReport(string action)
    {
        //saveReport.Actions.Add(action);
    }

    /// <summary>
    /// Agrega información de un objeto al reporte.
    /// </summary>
    /// <param name="objectInfo">Información del objeto (ID y nombre).</param>

    public void SaveObjectToReport(string objectInfo)
    {
        // Se agrega la información del objeto a la lista de objetos del reporte
        saveReport.Objects.Add(objectInfo); // Agregar el objeto con ID y nombre
    }

    /// <summary>
    /// Inicializa un nuevo reporte asignando un ID único y configurando fecha y hora.
    /// </summary>

    public void InitializeNewReport()
    {
        // Genera un nuevo ID incrementando el valor almacenado en PlayerPrefs
        int currentId = PlayerPrefs.GetInt("id", 0) + 1;
        saveReport.id = currentId;
        PlayerPrefs.SetInt("id", currentId); // Guarda el nuevo ID
        PlayerPrefs.Save(); // Asegura que los datos se guarden en disco

        // Asigna la fecha y hora actual en formato UTC
        saveReport.date = DateTime.UtcNow.ToString("yyyy-MM-dd");
        saveReport.time = DateTime.UtcNow.ToString("HH:mm:ss");
    }

    /// <summary>
    /// Guarda el reporte en un archivo JSON dentro de la carpeta "Reports".
    /// </summary>

    public void SaveReportToFile()
    {
        // Inicializa el reporte con información básica
        InitializeNewReport();

        // Genera el nombre del archivo basado en el ID y la fecha
        string newFileName = "Report_" + saveReport.id + "_" + saveReport.date + ".json";
        FileName = newFileName;

        // Define la ruta de la carpeta "Reports"
        string Folder = "/Reports/";
        string folderPath = UnityEngine.Application.dataPath + Folder;

        // Crea la carpeta si no existe
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Combina la ruta de la carpeta con el nombre del archivo
        filePath = folderPath + newFileName;

        // Convierte el reporte a formato JSON con formato legible
        string json = JsonUtility.ToJson(saveReport, true);

        try
        {
            // Intenta escribir el archivo JSON en la ruta especificada
            File.WriteAllText(filePath, json);
            UnityEngine.Debug.Log("Reporte guardado en: " + filePath);
        }
        catch (Exception e)
        {
            // Maneja errores al intentar guardar el archivo
            UnityEngine.Debug.LogError("Error al guardar el reporte: " + e.Message);
        }
    }
}
