using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystemN : MonoBehaviour
{
    // Instancia que contiene la informaci�n del reporte que se va a generar
    private SaveReport saveReport;

    // Nombre del archivo que contendr� el reporte guardado
    private string FileName;

    // Ruta donde se almacenar� el archivo de reporte dentro del sistema de archivos del proyecto
    private string filePath;

    // M�todo que crea una nueva entrada de notificaci�n al reporte
    public void AddNotificationToReport(string title, string message, float duration)
    {
        // Crea una nueva entrada de notificaci�n usando los datos recibidos
        NotificationEntry entry = new NotificationEntry(title, message, duration);

        // Agrega la notificaci�n al reporte
        saveReport.Notifications.Add(entry.title + ": " + entry.message);
    }

    // M�todo para crear un nuevo reporte, generando un ID �nico y registrando la fecha y hora de su creaci�n
    public void CreateReport()
    {
        // Almacenar� las acciones y notificaciones
        saveReport = new SaveReport();

        // Genera un ID �nico para el reporte usando PlayerPrefs
        int currentId = PlayerPrefs.GetInt("id", 0) + 1;  // Obtiene el valor actual del ID y lo incrementa
        saveReport.id = currentId;

        // Guarda el nuevo ID en PlayerPrefs para que se use la pr�xima vez
        PlayerPrefs.SetInt("id", currentId);
        PlayerPrefs.Save();  // Aseg�rate de guardar los cambios en PlayerPrefs

        // Asigna la fecha actual en formato "yyyy-MM-dd"
        saveReport.date = System.DateTime.UtcNow.ToString("yyyy-MM-dd");

        // Asigna la hora actual en formato "HH:mm:ss"
        saveReport.time = System.DateTime.UtcNow.ToString("HH:mm:ss");
    }

    // M�todo para guardar el reporte en un archivo JSON
    public void SaveReportToFile()
    {
        // Define un nombre de archivo �nico combinando el ID del reporte y su fecha de creaci�n
        string newFileName = "Report_" + saveReport.id + "_" + saveReport.date + ".json";
        FileName = newFileName;

        // Define la carpeta donde se guardar� el archivo
        string Folder = "/Reports/";
        string folderPath = UnityEngine.Application.dataPath + Folder;  // Ruta a la carpeta "Reports"

        // Verifica si la carpeta existe, si no, la crea
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Define la ruta completa del archivo dentro de la carpeta "Reports"
        filePath = folderPath + newFileName;

        // Convierte el reporte a formato JSON
        string json = JsonUtility.ToJson(saveReport, true);

        try
        {
            // Escribe el archivo JSON en la ruta especificada
            File.WriteAllText(filePath, json);
            UnityEngine.Debug.Log("Reporte guardado en: " + filePath); // Correcci�n aqu�
        }
        catch (System.Exception e)
        {
            // Si ocurre un error durante la escritura del archivo, lo captura
            UnityEngine.Debug.LogError("Error al guardar el reporte: " + e.Message); // Correcci�n aqu�
        }
    }

    // M�todo para agregar una acci�n al reporte
    public void SaveActionToReport(string action)
    {
        // Agrega la acci�n a la lista de acciones del reporte
        saveReport.Actions.Add(action);

        // Guarda el reporte actualizado en un archivo
        SaveReportToFile();
    }
}
