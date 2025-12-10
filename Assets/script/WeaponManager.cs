using UnityEngine;
using TMPro;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Holder")]
    public Transform weaponHolder;
    
    [Header("References")]
    public Camera fpsCamera;
    public TextMeshProUGUI ammoText;
    public AudioSource weaponAudioSource;
    
    private GameObject currentWeapon;
    private GunSystem currentGunSystem;
    
    private void Start()
    {
        Debug.Log("=== WeaponManager Start ===");
        
        // IMPORTANT : Détruire toute arme existante dans weaponHolder
        DestroyExistingWeapons();
        
        if (ClassManager.Instance != null && ClassManager.Instance.selectedClass != null)
        {
            Debug.Log($"[WeaponManager] Classe détectée : {ClassManager.Instance.selectedClass.className}");
            LoadWeaponFromClass(ClassManager.Instance.selectedClass);
        }
        else
        {
            Debug.LogWarning("[WeaponManager] ⚠️ Aucune classe sélectionnée ! Passez par le menu de sélection.");
        }
    }
    
    // NOUVELLE MÉTHODE : Détruire toutes les armes enfants existantes
    private void DestroyExistingWeapons()
    {
        if (weaponHolder == null)
        {
            Debug.LogError("[WeaponManager] weaponHolder est NULL ! Assignez-le dans l'Inspector.");
            return;
        }
        
        int childCount = weaponHolder.childCount;
        if (childCount > 0)
        {
            Debug.Log($"[WeaponManager] Destruction de {childCount} arme(s) existante(s)...");
            
            // Détruire tous les enfants
            for (int i = childCount - 1; i >= 0; i--)
            {
                Transform child = weaponHolder.GetChild(i);
                Debug.Log($"  - Suppression : {child.name}");
                Destroy(child.gameObject);
            }
        }
        else
        {
            Debug.Log("[WeaponManager] Aucune arme existante à détruire.");
        }
    }
    
    public void LoadWeaponFromClass(WeaponClass weaponClass)
    {
        if (weaponClass == null)
        {
            Debug.LogError("[WeaponManager] weaponClass est null !");
            return;
        }
        
        // Détruire l'arme actuelle si elle existe
        if (currentWeapon != null)
        {
            Debug.Log($"[WeaponManager] Destruction de l'arme actuelle : {currentWeapon.name}");
            Destroy(currentWeapon);
            currentWeapon = null;
            currentGunSystem = null;
        }
        
        // Vérifier que le prefab existe
        if (weaponClass.weaponPrefab == null)
        {
            Debug.LogError($"[WeaponManager] ✗ '{weaponClass.className}' n'a pas de weaponPrefab assigné !");
            Debug.LogError("[WeaponManager] Assignez le prefab dans le ScriptableObject WeaponClass.");
            return;
        }
        
        Debug.Log($"[WeaponManager] Chargement du prefab : {weaponClass.weaponPrefab.name}");
        
        // Instancier le nouveau prefab
        currentWeapon = Instantiate(weaponClass.weaponPrefab, weaponHolder);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;
        
        Debug.Log($"[WeaponManager] ✓ Prefab instancié : {currentWeapon.name}");
        
        // Récupérer le GunSystem
        currentGunSystem = currentWeapon.GetComponentInChildren<GunSystem>();
        if (currentGunSystem != null)
        {
            Debug.Log("[WeaponManager] ✓ GunSystem trouvé sur le prefab.");
            
            // Assigner les références de la scène au GunSystem
            if (fpsCamera != null)
            {
                currentGunSystem.fpsCam = fpsCamera;
                Debug.Log("[WeaponManager]   - Camera assignée");
            }
            
            if (ammoText != null)
            {
                currentGunSystem.text = ammoText;
                Debug.Log("[WeaponManager]   - AmmoText assigné");
            }
            
            if (weaponAudioSource != null)
            {
                currentGunSystem.audioSource = weaponAudioSource;
                Debug.Log("[WeaponManager]   - AudioSource assigné");
            }
            
            // Initialiser avec les stats de la classe
            currentGunSystem.InitializeFromClass(weaponClass);
            Debug.Log($"[WeaponManager] ✓✓✓ Arme '{weaponClass.className}' chargée et initialisée avec succès !");
        }
        else
        {
            Debug.LogError($"[WeaponManager] ✗ Aucun GunSystem trouvé sur {weaponClass.weaponPrefab.name}");
            Debug.LogError("[WeaponManager] Vérifiez que le prefab contient un composant GunSystem.");
        }
    }
    
    public GunSystem GetCurrentGunSystem()
    {
        return currentGunSystem;
    }
}
