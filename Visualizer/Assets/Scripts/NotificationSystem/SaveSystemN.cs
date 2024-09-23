using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystemN : MonoBehaviour
{
    //Instancia que contiene la informacion del reporte que se va a generar
    //Esta clase actúa como un contenedor para las acciones y notificaciones que serán registradas en el reporte.
    private SaveReport saveReport;

    // Nombre del archivo que contendrá el reporte guardado.
    private string FileName;

    // Ruta donde se almacenará el archivo de reporte dentro del sistema de archivos del proyecto.
    private string filePath;

    // Método que crea una nueva entrada de notificación al reporte
    public void AddNotificationToReport(string title, string message, float duration)
    {
        // Crea una nueva entrada de notificación usando los datos recibidos.
        NotificationEntry entry = new NotificationEntry(title, message, duration);

        // Agrega la notificación al reporte, almacenándola como una cadena de texto en la lista Notifications.
        saveReport.Notifications.Add(entry.title + ": " + entry.message);
    }

    // Método para crear un nuevo reporte, generando un ID único y registrando la fecha y hora de su creación.
    public void CreateReport()
    {
        //Almacenará las acciones y notificaciones.
        saveReport = new SaveReport();

        //Genera un ID único para el reporte 
        saveReport.id = PlayerPrefs.GetInt("id", 0) + 1;

        // Asigna la fecha actual al campo 'date'
        saveReport.date = System.DateTime.Now.ToString("yyyy-MM-dd");

        // Asigna la hora actual al campo 'time'
        saveReport.time = System.DateTime.Now.ToString("HH:mm:ss");
    }

    // Método para guardar el reporte en un archivo JSON
    //Convierte el reporte en un formato de texto legible para ser almacenado localmente.
    public void SaveReportToFile()
    {
        // Define un nombre de archivo único combinando el ID del reporte y su fecha de creación.
        string newFileName = "Report_" + saveReport.id + "_" + saveReport.date + ".json";
        FileName = newFileName;

        // Define la carpeta donde se guardará el archivo, dentro de la carpeta "Reports" en la ruta de datos del juego.
        string Folder = "/Reports/";
        string folderPath = UnityEngine.Application.dataPath + Folder;  // Ruta a la carpeta "Reports"

        // Verifica si la carpeta existe, si no, la crea.
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Define la ruta completa del archivo dentro de la carpeta "Reports".
        filePath = folderPath + newFileName;

        // Convierte el reporte a formato JSON para facilitar su almacenamiento.
        string json = JsonUtility.ToJson(saveReport, true);

        try
        {
            // Escribe el archivo JSON en la ruta especificada.
            File.WriteAllText(filePath, json);
            UnityEngine.Debug.Log("Reporte guardado en: " + filePath);
        }
        catch (System.Exception e)
        {
            // Si ocurre un error durante la escritura del archivo, lo captura y muestra un mensaje de error en la consola de Unity.
            UnityEngine.Debug.LogError("Error al guardar el reporte: " + e.Message);
        }
    }

    // Método para agregar una acción al reporte
    public void SaveActionToReport(string action)
    {
        // Agrega la acción a la lista de acciones del reporte.
        saveReport.Actions.Add(action);

        // Guarda el reporte actualizado en un archivo.
        SaveReportToFile();
    }
}
