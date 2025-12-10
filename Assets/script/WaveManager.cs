using UnityEngine;
using System.Collections.Generic;
using DefaultNamespace;

public class WaveManager : MonoBehaviour
{
    [Header("Paramètres des Vagues")]
    public ZombieSpawner zombieSpawner; // Référence au ZombieSpawner
    public int nombreMaxManches = 10;

    [Header("Affichage")]
    public UnityEngine.UI.Text texteVague;

    private int mancheActuelle = 0;
    private List<MonsterController> zombiesActuels = new List<MonsterController>();
    private DifficultyManager.DifficultySettings parametresDifficulte;
    private float timerDelaiManche = 0f;
    private bool enDelai = false;
    private bool peutSpawner = true;
    private bool delaiEnCoursDeMarrage = false; // Flag pour éviter de relancer le délai plusieurs fois

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

        // Démarrer la première manche automatiquement
        DemarrerManche();
    }

    void Update()
    {
        // Si on est en délai entre les manches
        if (enDelai)
        {
            timerDelaiManche -= Time.deltaTime;

            if (timerDelaiManche <= 0f)
            {
                enDelai = false;
                peutSpawner = true;

                if (mancheActuelle < nombreMaxManches)
                {
                    DemarrerManche();
                }
            }
        }

        // Vérifier si tous les zombies sont morts (une seule fois)
        if (peutSpawner == false && zombiesActuels.Count == 0 && !delaiEnCoursDeMarrage && mancheActuelle < nombreMaxManches)
        {
            delaiEnCoursDeMarrage = true; // Marquer que le délai est en cours
            enDelai = true;
            timerDelaiManche = parametresDifficulte.delaiEntreManches;
        }

        // Nettoyer la liste des zombies morts
        zombiesActuels.RemoveAll(z => z == null);
    }

    void DemarrerManche()
    {
        mancheActuelle++;
        peutSpawner = false;
        delaiEnCoursDeMarrage = false; // Réinitialiser le flag pour la prochaine fois

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

            // Ajouter les anciens zombies toujours vivants
            for (int i = 0; i < nombreAvant; i++)
            {
                if (zombiesAvant[i] != null)
                {
                    zombiesActuels.Add(zombiesAvant[i]);
                }
            }
        }

        peutSpawner = false; // Empêcher de respawner avant que tous soient morts
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
