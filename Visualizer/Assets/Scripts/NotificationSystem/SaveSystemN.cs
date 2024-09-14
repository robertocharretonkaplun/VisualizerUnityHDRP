using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class SaveSystemN : MonoBehaviour
{
    private SaveReport saveReport = new SaveReport(); 

  
    public void AddNotificationToReport(string title, string message, float duration)
    {
        NotificationEntry entry = new NotificationEntry(title, message, duration);
        saveReport.notifications.Add(entry);  
    }

    public void SaveReportToFile(string filePath)
    {
        string json = JsonUtility.ToJson(saveReport, true);  

        try
        {
            File.WriteAllText(filePath, json);  
            UnityEngine.Debug.Log("Reporte guardado en: " + filePath);
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("Error al guardar el reporte: " + e.Message);
        }

    }
}
