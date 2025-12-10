using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject grenadePrefab;
    public Transform spawnPoint;
    public Camera fpsCam;
    public float forwardSpeed = 10f;
    public float upSpeed = 5f;
    public float explosionDelay = 3f;
    public float explosionRadius = 5f;
    public float explosionForce = 500f;
    public float bounciness = 0.5f;
    public float damageRadius = 3f; // Rayon des dégâts
    public float explosionDamage = 50f; // Dégâts infligés (50 points au lieu de tuer)
    public GameObject explosionEffect; // Effet visuel d'explosion (optionnel)

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            Launch();
    }

    void Launch()
    {
        if (grenadePrefab == null || spawnPoint == null) return;

        GameObject g = Instantiate(grenadePrefab, spawnPoint.position, Quaternion.identity);

        Rigidbody rb = g.GetComponent<Rigidbody>();
        if (rb == null) rb = g.AddComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (g.GetComponent<Collider>() == null)
        {
            SphereCollider col = g.AddComponent<SphereCollider>();
            col.radius = 0.1f;
        }

        PhysicsMaterial bounceMat = new PhysicsMaterial();
        bounceMat.bounciness = bounciness;
        bounceMat.frictionCombine = PhysicsMaterialCombine.Minimum;
        bounceMat.bounceCombine = PhysicsMaterialCombine.Maximum;
        g.GetComponent<Collider>().material = bounceMat;

        Vector3 dir = (fpsCam != null) ? fpsCam.transform.forward : transform.forward;
        rb.linearVelocity = dir.normalized * forwardSpeed + Vector3.up * upSpeed;

        GrenadeExplosion explosion = g.AddComponent<GrenadeExplosion>();
        explosion.delay = explosionDelay;
        explosion.radius = explosionRadius;
        explosion.force = explosionForce;
        explosion.damageRadius = damageRadius;
        explosion.damage = explosionDamage;
        explosion.explosionEffect = explosionEffect;
    }
}

public class GrenadeExplosion : MonoBehaviour
{
    [HideInInspector] public float delay;
    [HideInInspector] public float radius;
    [HideInInspector] public float force;
    [HideInInspector] public float damageRadius;
    [HideInInspector] public float damage;
    [HideInInspector] public GameObject explosionEffect;

    void Start()
    {
        Invoke(nameof(Explode), delay);
    }

    void Explode()
    {
        Vector3 pos = transform.position;

        // Créer l'effet d'explosion si défini
        if (explosionEffect != null)
        {
            Debug.Log($"[Grenade] Création de l'effet d'explosion à {pos}");
            GameObject effect = Instantiate(explosionEffect, pos, Quaternion.identity);
            // Détruire l'effet après 5 secondes pour éviter l'accumulation
            Destroy(effect, 5f);
        }
        else
        {
            Debug.LogWarning("[Grenade] Aucun effet d'explosion assigné dans l'inspecteur!");
        }

        // Force physique
        Collider[] hits = Physics.OverlapSphere(pos, radius);
        foreach (Collider c in hits)
        {
            Rigidbody rb = c.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddExplosionForce(force, pos, radius);
        }

        // Dégâts aux zombies
        Collider[] damageHits = Physics.OverlapSphere(pos, damageRadius);
        foreach (Collider c in damageHits)
        {
            // Chercher le composant ReceiveDamage sur l'objet ou ses parents
            ReceiveDamage health = c.GetComponent<ReceiveDamage>();
            if (health == null)
                health = c.GetComponentInParent<ReceiveDamage>();
            
            if (health != null)
            {
                health.GetDamage(damage);
                Debug.Log($"[Grenade] Dégâts infligés à {c.gameObject.name}: {damage}");
            }
        }

        Destroy(gameObject);
    }
}
