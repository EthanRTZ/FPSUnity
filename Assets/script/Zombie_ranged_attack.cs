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
    private Transform playerCapsule;     // Transform de la "Capsule" du player à viser

    void Start()
    {
        // Cherche le joueur
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null)
        {
            player = playerGO.transform;

            // On cherche l'enfant nommé "Capsule" directement
            Transform capsule = playerGO.transform.Find("Capsule");

            // Si non trouvé directement, on cherche dans tous les enfants
            if (capsule == null)
            {
                foreach (Transform t in playerGO.GetComponentsInChildren<Transform>())
                {
                    if (t.name == "Capsule")
                    {
                        capsule = t;
                        break;
                    }
                }
            }

            if (capsule != null)
            {
                playerCapsule = capsule;
                Debug.Log("ZombieRangedAttack : cible = " + playerCapsule.name);
            }
            else
            {
                Debug.LogWarning("ZombieRangedAttack : aucun enfant 'Capsule' trouvé sur Player, le zombie visera player.position (souvent aux pieds).");
            }
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

        // Calcule la position à viser :
        // - si on a trouvé l'enfant "Capsule", on vise sa position (au milieu du joueur)
        // - sinon, on vise player.position (souvent aux pieds)
        Vector3 targetPos = player.position;
        if (playerCapsule != null)
            {
                // Légèrement au-dessus du centre de la capsule si besoin
            targetPos = playerCapsule.position + Vector3.up * 0.3f;
        }

        // Calcule la direction vers la cible
        Vector3 direction = (targetPos - shootPoint.position).normalized;

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
