using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneM : MonoBehaviour
{
    // The name of the scene you want to load
    public string gameSceneName = "GameScene";

    // Function to be called when the Play button is clicked
    public void OnPlayButtonClick()
    {
        // Load the specified scene
        SceneManager.LoadScene(gameSceneName);
    }
}
