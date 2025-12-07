using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    void Awake()
    {
        // Singleton : Une seule instance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persiste entre les scènes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Changer le volume
    public void SetVolume(float volume)
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.volume = volume;
            Debug.Log("Volume changé à : " + volume);
        }
        else
        {
            Debug.LogError("Aucun AudioSource trouvé sur l'AudioManager !");
        }
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }
}
