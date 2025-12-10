using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsDefeatMenu : MonoBehaviour
{
    public void Back()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void SetVolume(float volume)
    {
        AudioManager.Instance.SetVolume(volume);
    }

}
