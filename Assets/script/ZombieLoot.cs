using UnityEngine;

/// <summary>
/// Gère le système de loot des zombies
/// Attaché aux prefabs de zombies
/// </summary>
public class ZombieLoot : MonoBehaviour
{
    [Header("Prefabs de loot")]
    public GameObject ammoPrefab;
    public GameObject grenadePrefab;
    public GameObject stimulantPrefab;

    [Header("Chance globale de drop")]
    [Range(0, 100)]
    [Tooltip("Chance qu'un zombie drop quelque chose (33% = 1 chance sur 3)")]
    public float globalDropChance = 33f; // 1 chance sur 3 de dropper quelque chose

    [Header("Probabilités de drop (%)")]
    [Range(0, 100)]
    public float ammoDropChance = 60f; // 60% de chance
    [Range(0, 100)]
    public float grenadeDropChance = 25f; // 25% de chance
    [Range(0, 100)]
    public float stimulantDropChance = 15f; // 15% de chance (rare)

    [Header("Quantités")]
    public int ammoAmountMin = 5;  // Réduit de 15 à 5
    public int ammoAmountMax = 15; // Réduit de 30 à 15
    public int grenadeAmount = 1;
    public int stimulantAmount = 1;

    [Header("Drop settings")]
    public float dropHeight = 0.5f; // Hauteur au-dessus du sol pour le drop
    public bool dropOnDeath = true;

    /// <summary>
    /// Appelé quand le zombie meurt (depuis vie_zombie.cs)
    /// </summary>
    public void DropLoot()
    {
        if (!dropOnDeath) return;

        // Vérifier d'abord si le zombie drop quelque chose (chance globale)
        float globalRoll = Random.Range(0f, 100f);
        if (globalRoll > globalDropChance)
        {
            return;
        }

        // Position du drop (légèrement au-dessus du zombie)
        Vector3 dropPosition = transform.position + Vector3.up * dropHeight;

        // Tirer un nombre aléatoire pour déterminer quel loot dropper
        float randomValue = Random.Range(0f, 100f);

        GameObject lootToDrop = null;
        LootType lootType = LootType.Ammo;
        int amount = 1;

        // Déterminer quel loot dropper selon les probabilités
        if (randomValue < stimulantDropChance && stimulantPrefab != null)
        {
            // Stimulant 
            lootToDrop = stimulantPrefab;
            lootType = LootType.Stimulant;
            amount = stimulantAmount;
        }
        else if (randomValue < stimulantDropChance + grenadeDropChance && grenadePrefab != null)
        {
            // Grenade 
            lootToDrop = grenadePrefab;
            lootType = LootType.Grenade;
            amount = grenadeAmount;
        }
        else if (randomValue < stimulantDropChance + grenadeDropChance + ammoDropChance && ammoPrefab != null)
        {
            // Munitions 
            lootToDrop = ammoPrefab;
            lootType = LootType.Ammo;
            amount = Random.Range(ammoAmountMin, ammoAmountMax + 1);
        }
        else
        {
            return;
        }

        // Instancier le loot
        if (lootToDrop != null)
        {
            GameObject loot = Instantiate(lootToDrop, dropPosition, Quaternion.identity);

            // Configurer le LootItem
            LootItem lootItem = loot.GetComponent<LootItem>();
            if (lootItem == null)
            {
                lootItem = loot.AddComponent<LootItem>();
            }

            lootItem.lootType = lootType;

            if (lootType == LootType.Ammo)
            {
                lootItem.ammoAmount = amount;
            }
            else
            {
                lootItem.amount = amount;
            }
        }
    }
}
