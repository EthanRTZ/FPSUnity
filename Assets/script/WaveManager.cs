using UnityEngine;
using System.Collections.Generic;
using DefaultNamespace;

public class WaveManager : MonoBehaviour
{
    // Singleton pour éviter plusieurs instances
    public static WaveManager Instance { get; private set; }

    [Header("Paramètres des Vagues")]
    public ZombieSpawner zombieSpawner; // Référence au ZombieSpawner
    public int nombreMaxManches = 10;

    [Header("Affichage")]
    public UnityEngine.UI.Text texteVague;
    public UnityEngine.UI.Text zombieRestant;

    private int mancheActuelle = 0;
    private List<MonsterController> zombiesActuels = new List<MonsterController>();
    private DifficultyManager.DifficultySettings parametresDifficulte;
    private float timerDelaiManche = 0f;
    private bool enDelai = false;
    private bool mancheEnCours = false; // Flag pour savoir si une manche est active

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // S'assurer que le DifficultyManager existe, sinon le créer
        if (DifficultyManager.Instance == null)
        {
            GameObject managerObj = new GameObject("DifficultyManager");
            managerObj.AddComponent<DifficultyManager>();
        }

        // Récupérer les paramètres de difficulté
        parametresDifficulte = DifficultyManager.Instance.ObtenirParametresActuels();
        DemarrerManche();
    }

    void Update()
    {
        // Mettre à jour le texte des zombies restants
        if(zombieRestant != null){
            int valZombieRes = GetNombreZombiesRestants();
            zombieRestant.text = $"Zombies : {valZombieRes}";
        }
        
        // Nettoyer la liste des zombies morts
        zombiesActuels.RemoveAll(z => z == null);

        // Si on est en délai entre les manches
        if (enDelai)
        {
            timerDelaiManche -= Time.deltaTime;

            if (timerDelaiManche <= 0f)
            {
                enDelai = false;
                Debug.Log($"[WaveManager] Délai écoulé, démarrage de la prochaine manche...");

                // Après le délai, démarrer la prochaine manche
                if (mancheActuelle < nombreMaxManches)
                {
                    DemarrerProchaineManche();
                }
            }
        }
        // Vérifier si tous les zombies sont morts ET qu'une manche est en cours ET qu'on n'est PAS en délai
        else if (mancheEnCours && zombiesActuels.Count == 0 && !enDelai && mancheActuelle < nombreMaxManches)
        {
            // Tous les zombies sont morts, on arrête la manche et on lance le délai
            mancheEnCours = false;
            enDelai = true;
            timerDelaiManche = parametresDifficulte.delaiEntreManches;
        }


    }

    void DemarrerManche()
    {
        // Cette fonction est appelée au Start pour la première manche
        mancheActuelle = 1;
        SpawnerZombiesManche();
        mancheEnCours = true; // Activer APRÈS le spawn
    }

    void DemarrerProchaineManche()
    {
        // Cette fonction est appelée après le délai pour les manches suivantes
        mancheActuelle++;
        SpawnerZombiesManche();
        mancheEnCours = true; // Activer APRÈS le spawn
    }

    void SpawnerZombiesManche()
    {
        if (mancheActuelle > nombreMaxManches)
        {
            return;
        }

        // Nombre de zombies = zombiesParManche + (zombiesParManche * (mancheActuelle - 1))
        int zombiesASpawner = parametresDifficulte.zombiesParManche +
                              (parametresDifficulte.zombiesParManche * (mancheActuelle - 1));

        // Mettre à jour le texte d'affichage
        if (texteVague != null)
        {
            texteVague.text = $"Manche : {mancheActuelle}/{nombreMaxManches}";
        }

        // Utiliser le ZombieSpawner pour spawner les zombies
        if (zombieSpawner != null)
        {
            // Compter les zombies AVANT le spawn
            MonsterController[] zombiesAvant = FindObjectsOfType<MonsterController>();
            int nombreAvant = zombiesAvant.Length;

            // Spawner les nouveaux zombies
            zombieSpawner.SpawnerZombies(zombiesASpawner);

            // Récupérer tous les zombies APRÈS le spawn
            MonsterController[] tousLesZombies = FindObjectsOfType<MonsterController>();
            zombiesActuels.Clear();

            // Ajouter SEULEMENT les nouveaux zombies (ceux créés après la ligne 96)
            for (int i = nombreAvant; i < tousLesZombies.Length; i++)
            {
                MonsterController zombie = tousLesZombies[i];

                // Appliquer les dégâts selon la difficulté SEULEMENT aux nouveaux
                zombie.DefinirDegats(parametresDifficulte.degatsZombie);

                // Appliquer aussi les dégâts au script zombie_melee_attack
                DefaultNamespace.zombie_melee_attack meleeAttack = zombie.GetComponent<DefaultNamespace.zombie_melee_attack>();
                if (meleeAttack != null)
                {
                    meleeAttack.attackDamage = parametresDifficulte.degatsZombie;
                }

                zombiesActuels.Add(zombie);
            }


        }
    }

    // Getter pour afficher le numéro de manche actuelle
    public int GetMancheActuelle()
    {
        return mancheActuelle;
    }

    // Getter pour afficher le nombre de zombies restants
    public int GetNombreZombiesRestants()
    {
        return zombiesActuels.Count;
    }
}
