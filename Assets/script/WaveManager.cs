using UnityEngine;
using System.Collections.Generic;

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

    void Start()
    {
        Debug.Log("🎬 WaveManager : Start()");

        // S'assurer que le DifficultyManager existe, sinon le créer
        if (DifficultyManager.Instance == null)
        {
            Debug.LogWarning("⚠️ WaveManager : DifficultyManager introuvable, création automatique...");
            GameObject managerObj = new GameObject("DifficultyManager");
            managerObj.AddComponent<DifficultyManager>();
        }

        Debug.Log("✅ DifficultyManager trouvé !");

        // Récupérer les paramètres de difficulté
        parametresDifficulte = DifficultyManager.Instance.ObtenirParametresActuels();
        Debug.Log($"📊 Paramètres : {parametresDifficulte.zombiesParManche} zombies par manche");

        if (zombieSpawner == null)
        {
            Debug.LogError("❌ WaveManager : ZombieSpawner non assigné !");
            return;
        }

        Debug.Log("✅ ZombieSpawner assigné !");

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
                else
                {
                    Debug.Log("✅ Toutes les vagues sont terminées !");
                }
            }
        }

        // Vérifier si tous les zombies sont morts
        if (peutSpawner == false && zombiesActuels.Count == 0 && mancheActuelle < nombreMaxManches)
        {
            Debug.Log($"💀 Tous les zombies de la manche {mancheActuelle} sont morts !");
            peutSpawner = false;
            enDelai = true;
            timerDelaiManche = parametresDifficulte.delaiEntreManches;

            Debug.Log($"⏱️ Délai de {parametresDifficulte.delaiEntreManches}s avant la manche suivante...");
        }

        // Nettoyer la liste des zombies morts
        zombiesActuels.RemoveAll(z => z == null);
    }

    void DemarrerManche()
    {
        mancheActuelle++;
        peutSpawner = true;

        if (mancheActuelle > nombreMaxManches)
        {
            Debug.Log("✅ Toutes les vagues sont terminées !");
            return;
        }

        // Nombre de zombies = zombiesParManche + (zombiesParManche * (mancheActuelle - 1))
        // Exemple en Facile (zombiesParManche = 3) :
        // Manche 1 = 3 + (3 * 0) = 3 zombies
        // Manche 2 = 3 + (3 * 1) = 6 zombies
        // Manche 3 = 3 + (3 * 2) = 9 zombies
        int zombiesASpawner = parametresDifficulte.zombiesParManche +
                              (parametresDifficulte.zombiesParManche * (mancheActuelle - 1));

        Debug.Log($"🌊 ===== MANCHE {mancheActuelle}/{nombreMaxManches} =====");
        Debug.Log($"🧟 Spawning {zombiesASpawner} zombies");
        Debug.Log($"💢 Dégâts : {parametresDifficulte.degatsZombie}");

        // Mettre à jour le texte d'affichage
        if (texteVague != null)
        {
            texteVague.text = $"Manche : {mancheActuelle}/{nombreMaxManches}";
        }

        // Utiliser le ZombieSpawner pour spawner les zombies
        if (zombieSpawner != null)
        {
            zombieSpawner.SpawnerZombies(zombiesASpawner);

            // Récupérer tous les zombies spawnés
            MonsterController[] tousLesZombies = FindObjectsOfType<MonsterController>();
            zombiesActuels.Clear();
            zombiesActuels.AddRange(tousLesZombies);
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
