using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DifficultyMenu : MonoBehaviour
{
    [Header("Indicateur Visuel (Optionnel)")]
    public Text texteDifficulteActuelle;
    public Color couleurSelectionnee = Color.green;
    public Color couleurNormale = Color.white;

    

    void Start()
    {
        // S'assurer que le DifficultyManager existe
        if (DifficultyManager.Instance == null)
        {
            GameObject managerObj = new GameObject("DifficultyManager");
            managerObj.AddComponent<DifficultyManager>();
        }

        // Afficher la difficulté actuelle
        MettreAJourAffichage();
    }

    // Méthode pour difficulté Facile (à assigner au bouton Facile)
    public void Facile()
    {
        SelectionnerDifficulte(DifficultyManager.Difficulty.Facile);
    }

    // Méthode pour difficulté Normal (à assigner au bouton Normal)
    public void Normal()
    {
        SelectionnerDifficulte(DifficultyManager.Difficulty.Normal);
    }

    // Méthode pour difficulté Difficile (à assigner au bouton Difficile)
    public void Difficile()
    {
        SelectionnerDifficulte(DifficultyManager.Difficulty.Difficile);
    }

    // Méthode privée pour gérer la sélection
    void SelectionnerDifficulte(DifficultyManager.Difficulty difficulte)
    {
        if (DifficultyManager.Instance != null)
        {
            DifficultyManager.Instance.DefinirDifficulte(difficulte);
            MettreAJourAffichage();
        }
    }

    void MettreAJourAffichage()
    {
        if (DifficultyManager.Instance == null)
            return;

        DifficultyManager.Difficulty difficulteActuelle = DifficultyManager.Instance.difficulteActuelle;

        // Mettre à jour le texte
        if (texteDifficulteActuelle != null)
        {
            texteDifficulteActuelle.text = $"Difficulté : {difficulteActuelle}";
        }
    }

    // Fonction pour démarrer le jeu (appelée par le bouton "Jouer")
    public void DemarrerJeu()
    {
        // Charger la scène de jeu (ajuste l'index selon ta config)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}

