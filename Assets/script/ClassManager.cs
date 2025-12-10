using UnityEngine;

public class ClassManager : MonoBehaviour
{
    public static ClassManager Instance { get; private set; }
    
    public WeaponClass selectedClass;
    
    private void Awake()
    {
        Debug.Log("=== ClassManager Awake ===");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[ClassManager] Instance créée et marquée DontDestroyOnLoad.");
        }
        else
        {
            Debug.Log("[ClassManager] Instance déjà existante. Destruction du doublon.");
            Destroy(gameObject);
        }
    }
    
    public void SelectClass(WeaponClass weaponClass)
    {
        if (weaponClass == null)
        {
            Debug.LogError("[ClassManager] SelectClass : weaponClass est NULL !");
            return;
        }
        
        selectedClass = weaponClass;
        Debug.Log($"[ClassManager] ✓ Classe sélectionnée : {weaponClass.className}");
        Debug.Log($"  - Dégâts : {weaponClass.damage}");
        Debug.Log($"  - Chargeur : {weaponClass.magazineSize}");
    }
}
