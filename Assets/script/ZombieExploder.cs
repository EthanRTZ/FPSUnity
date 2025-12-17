using UnityEngine;

public class ZombieExploder : MonoBehaviour
{
    public float autoExplodeDistance = 2f;
    public float explosionDamage = 50f;
    public GameObject explosionEffect;
    public bool destroyAfterExplosion = true;

    private bool hasExploded = false;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

    
    }

    void Update()
    {
        if (hasExploded || player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= autoExplodeDistance)
        {
            Explode();
        }
    }

    // 🔥 Cette fonction est appelée par ReceiveDamage quand la vie tombe à 0
    void Defeated()
    {
        if (!hasExploded)
        {
            Explode();
        }
    }

    void Explode()
    {
        if (hasExploded) return; // EMPÊCHE la boucle !!!
        hasExploded = true;


        // Effet visuel
        if (explosionEffect != null)
        {
            GameObject fx = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(fx, 3f);
        }

        // Dégâts au joueur
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist < autoExplodeDistance + 1f)
        {
            ReceiveDamage dmg = player.GetComponent<ReceiveDamage>();
            if (dmg != null)
                dmg.GetDamage(explosionDamage);
        }

        // Détruire le zombie
        if (destroyAfterExplosion)
            Destroy(gameObject);
    }
}
