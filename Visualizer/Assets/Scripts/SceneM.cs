using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Clase responsable de manejar la carga de escenas en el juego que puede ser llamada de diferentes formas.
/// </summary>
public class SceneM : MonoBehaviour
{
    /// <summary>
    /// El nombre de la escena que se desea cargar.
    /// </summary>
    public string gameSceneName = "GameScene";
    /// <summary>
    /// Función que se llama cuando se hace clic en el botón de Jugar.
    /// </summary>
    public void OnPlayButtonClick()
    {
        // Cargar la escena especificada
        SceneManager.LoadScene(gameSceneName);
    }
}
