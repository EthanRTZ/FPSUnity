using UnityEngine;

public class AbilityDeployment : MonoBehaviour
{
    [Header("Ability Settings")]
    public GameObject damageSpherePrefa;
    public float maxPlacementRange = 50f;
    public KeyCode activationKey = KeyCode.C;

    [Header("Camera")]
    public Camera fpsCam;

    void Start()
    {
        if (fpsCam == null)
        {
            fpsCam = Camera.main;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(activationKey))
        {
            if (AbilityChargeSystem.Instance != null && AbilityChargeSystem.Instance.IsReady())
            {
                TryDeployAbility();
            }
        }
    }

    void TryDeployAbility()
    {
        if (AbilityChargeSystem.Instance == null || !AbilityChargeSystem.Instance.IsReady())
        {
            return;
        }

        if (damageSpherePrefa == null)
        {
            return;
        }

        RaycastHit hit;
        if (fpsCam != null && Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, maxPlacementRange))
        {
            Vector3 spawnPosition = hit.point;
            GameObject sphere = Instantiate(damageSpherePrefa, spawnPosition, Quaternion.identity);
            AbilityChargeSystem.Instance.UseAbility();
        }
    }
}
