using UnityEngine;


public enum LootType
{
    Ammo,
    Grenade,
    Stimulant
}

public class LootItem : MonoBehaviour
{
    [Header("Type de loot")]
    public LootType lootType = LootType.Ammo;

    [Header("Quantité")]
    public int amount = 1; 
    [Header("Munitions spécifiques")]
    [Tooltip("Pour les munitions: quantité de balles à donner")]
    public int ammoAmount = 30;

    [Header("Visuals")]
    public float rotationSpeed = 50f; 
    public float bobSpeed = 1f; 
    public float bobHeight = 0.3f; 

    private Vector3 startPosition;
    private float bobTimer = 0f;

    void Start()
    {
        startPosition = transform.position;

        // S'assurer qu'il y a un collider en trigger
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            col = gameObject.AddComponent<SphereCollider>();
            ((SphereCollider)col).radius = 0.5f;
        }
        col.isTrigger = true;
    }

    void Update()
    {
        // Rotation
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Oscillation verticale (bob)
        bobTimer += Time.deltaTime * bobSpeed;
        float newY = startPosition.y + Mathf.Sin(bobTimer) * bobHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    void OnTriggerEnter(Collider other)
    {
        // Vérifier si c'est le joueur
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            PickUp(other.gameObject);
        }
    }

    void PickUp(GameObject player)
    {
        bool pickedUp = false;
        GameObject root = player.transform.root.gameObject;

        switch (lootType)
        {
            case LootType.Ammo:
                if (AmmoInventory.Instance != null)
                {
                    AmmoInventory.Instance.AddAmmo(ammoAmount);
                    pickedUp = true;
                }
                break;

            case LootType.Grenade:
                Grenade grenadeScript = root.GetComponent<Grenade>();
                if (grenadeScript == null)
                    grenadeScript = root.GetComponentInChildren<Grenade>();

                if (grenadeScript != null)
                {
                    grenadeScript.AddGrenades(amount);
                    pickedUp = true;
                }
                break;

            case LootType.Stimulant:
                Stimulant stimulantScript = root.GetComponent<Stimulant>();
                if (stimulantScript == null)
                    stimulantScript = root.GetComponentInChildren<Stimulant>();

                if (stimulantScript != null)
                {
                    stimulantScript.AddStimulants(amount);
                    pickedUp = true;
                }
                break;
        }

        if (pickedUp)
        {
            Destroy(gameObject);
        }
    }
}
