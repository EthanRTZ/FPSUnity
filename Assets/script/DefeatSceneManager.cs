using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public class DefeatSceneManager : MonoBehaviour
    {
        void Start()
        {
            // S'assurer que le temps est normal
            Time.timeScale = 1f;
            
            // Déverrouiller le curseur pour pouvoir cliquer sur les boutons
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        // Méthode à appeler depuis le bouton "Rejouer"
        // Rejouer: Defeat(3) -> Prototype Map(1) => -2
        public void Rejouer()
        {
            // Réinitialiser le temps
            Time.timeScale = 1f;
            
            // Reverrouiller le curseur pour le gameplay
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            int targetIndex = SceneManager.GetActiveScene().buildIndex - 2;
            SceneManager.LoadScene(targetIndex);
        }
        
        // Méthode pour aller aux options
        // Options: Defeat(3) -> Options(2) => -1 (ou charger index 2 directement)
        public void Options()
        {
            // Réinitialiser le temps
            Time.timeScale = 1f;

            int targetIndex = SceneManager.GetActiveScene().buildIndex + 1; // 3 -> 2
            SceneManager.LoadScene(targetIndex);
        }
        
        // Méthode pour retourner au menu principal
        public void RetournerAuMenu()
        {
            // Réinitialiser le temps
            Time.timeScale = 1f;
            
            // Déverrouiller le curseur
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            // Charger le menu (toujours build index 0)
            SceneManager.LoadScene(0);
        }
        
        // Méthode pour quitter le jeu
        public void QuitterJeu()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}
