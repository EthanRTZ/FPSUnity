using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    // Singleton pour persister entre les scènes
    public static DifficultyManager Instance { get; private set; }

    // Énumération des difficultés
    public enum Difficulty
    {
        Facile,
        Normal,
        Difficile
    }

    // Difficulté actuelle (par défaut : Normal)
    public Difficulty difficulteActuelle = Difficulty.Normal;

    // Paramètres par difficulté
    [System.Serializable]
    public class DifficultySettings
    {
        public int zombiesParManche = 5;
        public float delaiEntreManches = 2f;
        public float degatsZombie = 10f;
    }

    [Header("Paramètres des Difficultés")]
    public DifficultySettings facile = new DifficultySettings
    {
        zombiesParManche = 3,
        delaiEntreManches = 2f,
        degatsZombie = 5f
    };

    public DifficultySettings normal = new DifficultySettings
    {
        zombiesParManche = 5,
        delaiEntreManches = 2f,
        degatsZombie = 10f
    };

    public DifficultySettings difficile = new DifficultySettings
    {
        zombiesParManche = 8,
        delaiEntreManches = 2f,
        degatsZombie = 15f
    };

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Charger la difficulté sauvegardée
            ChargerDifficulte();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Définir la difficulté
    public void DefinirDifficulte(Difficulty nouvelleDifficulte)
    {
        difficulteActuelle = nouvelleDifficulte;
        SauvegarderDifficulte();
    }

    // Obtenir les paramètres de la difficulté actuelle
    public DifficultySettings ObtenirParametresActuels()
    {
        switch (difficulteActuelle)
        {
            case Difficulty.Facile:
                return facile;
            case Difficulty.Normal:
                return normal;
            case Difficulty.Difficile:
                return difficile;
            default:
                return normal;
        }
    }

    // Sauvegarder la difficulté
    void SauvegarderDifficulte()
    {
        PlayerPrefs.SetInt("Difficulty", (int)difficulteActuelle);
        PlayerPrefs.Save();
    }

    // Charger la difficulté
    void ChargerDifficulte()
    {
        int savedDifficulty = PlayerPrefs.GetInt("Difficulty", (int)Difficulty.Normal);
        difficulteActuelle = (Difficulty)savedDifficulty;
    }
}
