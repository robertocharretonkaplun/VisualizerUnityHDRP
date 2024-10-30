using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystemN : MonoBehaviour
{
    private SaveReport saveReport; // Variable para almacenar el reporte en la memoria antes de guardarlo.

    private string FileName; //Nombre del archivo que se generara al guardar el reporte.

    private string filePath; //Ruta completa donde se guardará el archivo del reporte.

    private void Awake()
    {
        // Crear una instancia vacía de SaveReport en el inicio
        saveReport = new SaveReport();
    }

    // Método para agregar una notificación a la lista en memoria
    public void AddNotificationToReport(string title, string message, float duration)
    {
        // Crea una entrada de notificación con título, mensaje y duración.
        NotificationEntry entry = new NotificationEntry(title, message, duration);
        //Agrega la notificación al listado de notificaciones en el reporte.
        saveReport.Notifications.Add(entry.title + ": " + entry.message);
    }

    // Método para agregar una acción a la lista en memoria
    public void SaveActionToReport(string action)
    {
        //Agrega laación al listado de acciones en el reporte.
        saveReport.Actions.Add(action);
    }

    // Método para crear un nuevo reporte temporal en memoria
    public void InitializeNewReport()
    {
        //Obtiene el ultimo id almacenado en PlayerPrefs y le suma 1.
        int currentId = PlayerPrefs.GetInt("id", 0) + 1;
        //Asigna el nuevo ID al reporte.
        saveReport.id = currentId;
        //Almacena el nuevo IDen PlayerPrefs para persistirlo
        PlayerPrefs.SetInt("id", currentId);
        PlayerPrefs.Save(); //Guarda PlayerPrefs en disco.

        //Asigna la fecha actual al reporte.
        saveReport.date = DateTime.UtcNow.ToString("yyyy-MM-dd");
        //Asigna la hora actual al reporte.
        saveReport.time = DateTime.UtcNow.ToString("HH:mm:ss");
    }

    // Método para guardar el reporte en un archivo JSON al hacer clic en el botón de guardar
    public void SaveReportToFile()
    {
        //Inicializa el reporte con un nuevo ID, fecha y hora antes de guardar el reporte.
        InitializeNewReport();
        //Genera el nombre del archivo usando el ID y la fecha del reporte.
        string newFileName = "Report_" + saveReport.id + "_" + saveReport.date + ".json";
        FileName = newFileName; //Almacena el nombre del archivo.
        //Define la carpeta donde se guardarán los reportes.
        string Folder = "/Reports/";
        //Define la ruta completa de la carpeta donde se almacenmará el archivo.
        string folderPath = UnityEngine.Application.dataPath + Folder;
        // Verifica si la carpeta de reportes existe; si no, la crea.
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        //Combina la ruta de la carpeta con el nombre del archivo para obtener la ruta completa.
        filePath = folderPath + newFileName;
        //Convierte el objeto saveReport a formato JASON con formato legible.
        string json = JsonUtility.ToJson(saveReport, true);

        //Intenta escribir el Json en la ruta especificada.
        try
        {
            File.WriteAllText(filePath, json);
            // Informa en la consola de Unity que el reporte se guardó correctamente.
            UnityEngine.Debug.Log("Reporte guardado en: " + filePath);
        }
        catch (Exception e)
        {
            // Muestra un error en la consola si ocurre un problema al guardar el archivo.
            UnityEngine.Debug.LogError("Error al guardar el reporte: " + e.Message);
        }
    }
}
