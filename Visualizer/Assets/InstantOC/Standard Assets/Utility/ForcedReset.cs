using System;
using UnityEngine;
using UnityEngine.SceneManagement; // Agrega esta línea para el manejo de escenas
using UnityStandardAssets.CrossPlatformInput;

public class ForcedReset : MonoBehaviour
{
    private void Update()
    {
        // si forzamos un reinicio ...
        if (CrossPlatformInputManager.GetButtonDown("ResetObject"))
        {
            //... recargar la escena
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name); // Método actualizado
        }
    }
}
