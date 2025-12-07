using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour
{
    public void Back()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
    }

    public void SetVolume(float volume)
    {
        Debug.Log("SetVolume appelé avec volume : " + volume);

        if (AudioManager.Instance != null)
        {
            Debug.Log("AudioManager trouvé !");
            AudioManager.Instance.SetVolume(volume);
        }
        else
        {
            Debug.LogWarning("AudioManager.Instance est NULL ! L'AudioManager n'existe pas.");
        }
    }

}
