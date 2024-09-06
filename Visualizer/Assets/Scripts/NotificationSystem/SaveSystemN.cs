using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class SaveSystemN : MonoBehaviour
{
    private string filePath;

    private void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "NotificationReport.json");
    }

    public void SaveReport(SaveReport report)
    {
        string json = JsonUtility.ToJson(report, true);
        File.WriteAllText(filePath, json);
        UnityEngine.Debug.Log("Report saved at: " + filePath);
    }

    public SaveReport LoadReport()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<SaveReport>(json);
        }
        else
        {
            return new SaveReport();
        }
    }
}
