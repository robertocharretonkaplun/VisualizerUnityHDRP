using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class SaveSystemN : MonoBehaviour
{
    private SaveReport saveReport; 
    string FileName;
    private string filePath;
  
    public void AddNotificationToReport(string title, string message, float duration)
    {
        NotificationEntry entry = new NotificationEntry(title, message, duration);
        saveReport.notifications.Add(entry);  
    }

    public void CreateReport() {
        SaveReport = new SaveReport();

    }

    // Esta funcion esta encarga de guardar reportes
    public void SaveReportToFile()
    {
        string newFileName =  "Report " + SaveReport.id + " " + SaveReport.date;
        FileName = newFileName;
        string Folder = "/Reports/";
        filePath = Application.dataPath + Folder + newFileName;
        
        string json = JsonUtility.ToJson(saveReport, true);  
        SaveReport = new SaveReport{
            id = PlayerPref.GetInt("id") + 1;
            date = DateTime.now;

        }
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

    public void SaveActionToReport(Action action) {
        SaveReport.Actions.Add(action.data);
        saveReport();
    }
}
