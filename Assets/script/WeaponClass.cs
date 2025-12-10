using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponClass", menuName = "Weapon System/Weapon Class")]
public class WeaponClass : ScriptableObject
{
    public string className;
    
    [Header("Gun Stats")]
    public int damage;
    public float timeBetweenShooting;
    public float spread;
    public float range;
    public float reloadTime;
    public float timeBetweenShots;
    public int magazineSize;
    public int bulletsPerTap;
    public bool allowButtonHold;
    
    [Header("Audio")]
    public AudioClip shootSound;
    
    [Header("Visuals")]
    public GameObject weaponPrefab;
}
