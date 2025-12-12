using UnityEngine;
using UnityEngine.SceneManagement;


public class WinMenu : MonoBehaviour
{
    void Start()
    {
        // Déverrouiller et afficher le curseur
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // bouton rejouer
    public void Rejouer()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    // bouton retour à l'accueil
    public void Menu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 6);
    }

}
