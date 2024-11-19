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
        saveReport = new SaveReport();
    }

    public void AddNotificationToReport(string title, string message, float duration)
    {
        NotificationEntry entry = new NotificationEntry(title, message, duration);
        saveReport.Notifications.Add(entry.title + ": " + entry.message);
    }

    public void SaveActionToReport(string action)
    {
        //saveReport.Actions.Add(action);
    }

    public void SaveObjectToReport(string objectInfo)
    {
        saveReport.Objects.Add(objectInfo); // Agregar el objeto con ID y nombre
    }

    public void InitializeNewReport()
    {
        int currentId = PlayerPrefs.GetInt("id", 0) + 1;
        saveReport.id = currentId;
        PlayerPrefs.SetInt("id", currentId);
        PlayerPrefs.Save();

        saveReport.date = DateTime.UtcNow.ToString("yyyy-MM-dd");
        saveReport.time = DateTime.UtcNow.ToString("HH:mm:ss");
    }

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
