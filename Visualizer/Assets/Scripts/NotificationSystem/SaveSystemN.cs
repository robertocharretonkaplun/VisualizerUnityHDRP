using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystemN : MonoBehaviour
{
    private SaveReport saveReport;

    private string FileName;
    private string filePath;

    private void Awake()
    {
        // Crear una instancia vacía de SaveReport en el inicio
        saveReport = new SaveReport();
    }

    // Método para agregar una notificación a la lista en memoria
    public void AddNotificationToReport(string title, string message, float duration)
    {
        NotificationEntry entry = new NotificationEntry(title, message, duration);
        saveReport.Notifications.Add(entry.title + ": " + entry.message);
    }

    // Método para agregar una acción a la lista en memoria
    public void SaveActionToReport(string action)
    {
        saveReport.Actions.Add(action);
    }

    // Método para crear un nuevo reporte temporal en memoria
    public void InitializeNewReport()
    {
        int currentId = PlayerPrefs.GetInt("id", 0) + 1;
        saveReport.id = currentId;
        PlayerPrefs.SetInt("id", currentId);
        PlayerPrefs.Save();

        saveReport.date = DateTime.UtcNow.ToString("yyyy-MM-dd");
        saveReport.time = DateTime.UtcNow.ToString("HH:mm:ss");
    }

    // Método para guardar el reporte en un archivo JSON al hacer clic en el botón de guardar
    public void SaveReportToFile()
    {
        InitializeNewReport();

        string newFileName = "Report_" + saveReport.id + "_" + saveReport.date + ".json";
        FileName = newFileName;

        string Folder = "/Reports/";
        string folderPath = UnityEngine.Application.dataPath + Folder;

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        filePath = folderPath + newFileName;

        string json = JsonUtility.ToJson(saveReport, true);

        try
        {
            File.WriteAllText(filePath, json);
            UnityEngine.Debug.Log("Reporte guardado en: " + filePath);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Error al guardar el reporte: " + e.Message);
        }
    }
}
