using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void PlayGame()
    {
        // Charger la scène ClassSelection (index 5)
        SceneManager.LoadScene(5); // ou SceneManager.LoadScene("ClassSelection");
    }

    public void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void Options()
    {
        // Charger la scène Options (index 2)
        SceneManager.LoadScene(2);
    }
}
