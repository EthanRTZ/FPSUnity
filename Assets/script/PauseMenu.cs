using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject pauseMenuUI;
    public Slider SensibiliteSliderX;
    public Slider SensibiliteSliderY;

    [Header("Settings")]
    public float SensibiliteBase = 100f;

    private bool isPaused = false;

    void Start()
    {
        // Charger les sensibilités sauvegardées
        float savedSensibiliteX = PlayerPrefs.GetFloat("MouseSensitivityX", SensibiliteBase);
        float savedSensibiliteY = PlayerPrefs.GetFloat("MouseSensitivityY", SensibiliteBase);

        if (SensibiliteSliderX != null)
        {
            SensibiliteSliderX.value = savedSensibiliteX;
        }

        if (SensibiliteSliderY != null)
        {
            SensibiliteSliderY.value = savedSensibiliteY;
        }

        // S'assurer que le menu est caché au départ
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
    }

    void Update()
    {
        // Appuyer sur Tab pour ouvrir/fermer le menu
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        Time.timeScale = 1f; // Remettre le temps normal
        isPaused = false;

        // Verrouiller le curseur
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Pause()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }
        Time.timeScale = 0f; // Mettre le jeu en pause
        isPaused = true;

        // Déverrouiller le curseur pour utiliser le menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SetSensibiliteX(float SensibiliteX)
    {
        // Sauvegarder la sensibilité X
        PlayerPrefs.SetFloat("MouseSensitivityX", SensibiliteX);
        PlayerPrefs.Save();

        Debug.Log("Sensibilité X changée à : " + SensibiliteX);

        // Mettre à jour la sensibilité de la caméra en temps réel
        UpdateCameraSensibiliteX(SensibiliteX);
    }

    public void SetSensibiliteY(float SensibiliteY)
    {
        // Sauvegarder la sensibilité Y
        PlayerPrefs.SetFloat("MouseSensitivityY", SensibiliteY);
        PlayerPrefs.Save();

        Debug.Log("Sensibilité Y changée à : " + SensibiliteY);

        // Mettre à jour la sensibilité de la caméra en temps réel
        UpdateCameraSensibiliteY(SensibiliteY);
    }

    void UpdateCameraSensibiliteX(float SensibiliteX)
    {
        // Chercher le script de caméra et mettre à jour la sensibilité X
        PlayerCamera playerCamera = FindObjectOfType<PlayerCamera>();
        if (playerCamera != null)
        {
            playerCamera.UpdateSensibiliteX(SensibiliteX);
        }
    }

    void UpdateCameraSensibiliteY(float SensibiliteY)
    {
        // Chercher le script de caméra et mettre à jour la sensibilité Y
        PlayerCamera playerCamera = FindObjectOfType<PlayerCamera>();
        if (playerCamera != null)
        {
            playerCamera.UpdateSensibiliteY(SensibiliteY);
        }
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; // Remettre le temps normal avant de changer de scène
        SceneManager.LoadScene(0); // Charger la scène du menu principal (index 0)
    }
}
