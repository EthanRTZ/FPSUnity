using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public class DefeatManager : MonoBehaviour
    {
        // Référence au joueur
        public GameObject player;
        
        // Le jeu est-il terminé ?
        private bool isDefeated = false;
        
        private ReceiveDamage playerHealth;
        
        void Start()
        {
            // Trouver le joueur si pas assigné
            if (player == null)
            {
                player = GameObject.Find("Player");
                if (player == null)
                {
                    return;
                }
            }
            
            // Récupérer le composant ReceiveDamage
            playerHealth = player.GetComponent<ReceiveDamage>();
            if (playerHealth == null)
            {
                playerHealth = player.GetComponentInChildren<ReceiveDamage>();
            }
        }
        
        void Update()
        {
            // Vérifier si le joueur est mort
            if (!isDefeated && playerHealth != null && playerHealth.health <= 0f)
            {
                Defeat();
            }
        }
        
        void Defeat()
        {
            isDefeated = true;
            
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            // Ordre des scènes: 0=Menu, 1=Prototype Map, 2=Options, 3=Defeat
            // Depuis le jeu (1) on doit aller à Defeat (3) => +2
            int targetIndex = SceneManager.GetActiveScene().buildIndex + 2;
            SceneManager.LoadScene(targetIndex);
        }
    }
}