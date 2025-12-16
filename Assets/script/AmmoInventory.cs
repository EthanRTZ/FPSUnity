using UnityEngine;
using TMPro;

/// <summary>
/// Gère l'inventaire des munitions du joueur
/// Singleton pour être accessible depuis n'importe où
/// </summary>
public class AmmoInventory : MonoBehaviour
{
    public static AmmoInventory Instance { get; private set; }

    [Header("Munitions de départ")]
    public int startingAmmo = 120; // Munitions totales au départ
    public int startingMagazine = 30; // Munitions dans le chargeur au départ

    [Header("UI")]
    public TextMeshProUGUI ammoCounterText; // Affichage: "30 / 120"

    private int currentAmmo; // Munitions en réserve
    private int currentMagazine; // Munitions dans le chargeur actuel

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Initialiser les munitions
        currentAmmo = startingAmmo;
        currentMagazine = startingMagazine;

        UpdateUI();
    }


    public int GetAmmo()
    {
        return currentAmmo;
    }
    public int GetMagazineAmmo()
    {
        return currentMagazine;
    }

    public bool UseBullet()
    {
        if (currentMagazine > 0)
        {
            currentMagazine--;
            UpdateUI();
            return true;
        }
        return false; 
    }


    public bool Reload(int magazineSize)
    {
        // Si le chargeur est déjà plein, pas besoin de recharger
        if (currentMagazine >= magazineSize)
            return false;

        // Si on n'a plus de munitions en réserve
        if (currentAmmo <= 0)
            return false;

        // Calculer combien de munitions on peut prendre de la réserve
        int bulletsNeeded = magazineSize - currentMagazine;
        int bulletsToTake = Mathf.Min(bulletsNeeded, currentAmmo);

        // Transférer les munitions
        currentAmmo -= bulletsToTake;
        currentMagazine += bulletsToTake;

        UpdateUI();
        return true;
    }


    public void AddAmmo(int amount)
    {
        currentAmmo += amount;
        UpdateUI();
    }


    public void SetMagazineAmmo(int amount)
    {
        currentMagazine = amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (ammoCounterText != null)
        {
            int totalAmmo = currentMagazine + currentAmmo;
            ammoCounterText.text = $"{totalAmmo}";
        }
    }
}
