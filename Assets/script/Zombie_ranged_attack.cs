using UnityEngine;

public class ZombieRangedAttack : MonoBehaviour
{
    public GameObject projectilePrefab;  // Ton prefab de projectile
    public Transform shootPoint;         // Point d'où part le tir
    public float projectileSpeed = 20f;  // Vitesse du projectile
    public float attackCooldown = 1f;    // Temps entre chaque tir
    public float attackRange = 50f;      // Distance max pour tirer

    private float nextShotTime = 0f;
    private Transform player;

    void Start()
    {
        // Cherche le joueur
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null)
        {
            player = playerGO.transform;
        }
        else
        {
            Debug.LogError("Player non trouvé ! Vérifie le tag 'Player'");
        }

        if (projectilePrefab == null)
            Debug.LogError("ProjectilePrefab non assigné !");
        if (shootPoint == null)
            Debug.LogError("ShootPoint non assigné !");
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Vérifie la distance et le cooldown
        if (distance <= attackRange && Time.time >= nextShotTime)
        {
            ShootAtPlayer();
            nextShotTime = Time.time + attackCooldown;
        }
    }

    void ShootAtPlayer()
    {
        if (projectilePrefab == null || shootPoint == null) return;

        // Instancie le projectile
        GameObject proj = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

        // Calcule la direction vers le joueur
        Vector3 direction = (player.position - shootPoint.position).normalized;

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false; // Pour qu'il ne tombe pas
            rb.linearVelocity = direction * projectileSpeed;
        }

        // Optionnel : décaler légèrement le projectile devant le zombie
        proj.transform.position += direction * 0.5f;

        // Ignore collisions avec le zombie
        Collider zombieCollider = GetComponent<Collider>();
        Collider projCollider = proj.GetComponent<Collider>();
        if (zombieCollider != null && projCollider != null)
        {
            Physics.IgnoreCollision(zombieCollider, projCollider, true);
        }
    }
}
