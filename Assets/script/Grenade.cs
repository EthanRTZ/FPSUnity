using UnityEngine;
using TMPro;

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
    public AudioClip launchSound; // Son du lancer (optionnel)
    public AudioClip explosionSound; // Son de l'explosion (optionnel)
    public float soundVolume = 1f; // Volume des sons

    // Grenades system
    public int startingGrenades = 3; // Nombre de grenades au départ
    public TextMeshProUGUI grenadeText; // Texte UI pour afficher le nombre de grenades
    public TextMeshProUGUI noGrenadeText; // Texte UI pour le message "pas de grenade"

    private int currentGrenades; // Nombre actuel de grenades
    private float noGrenadeMessageTimer; // Timer pour le message "pas de grenade"
    private bool showingNoGrenadeMessage = false; // Flag pour savoir si on affiche le message

    private void Awake()
    {
        currentGrenades = startingGrenades;
        noGrenadeMessageTimer = 0f;
        UpdateGrenadeText();
        // Ne pas afficher le message au démarrage
        if (noGrenadeText != null)
            noGrenadeText.gameObject.SetActive(false);
    }

    void Update()
    {
        // Gestion du timer du message "Plus de grenade!"
        if (showingNoGrenadeMessage)
        {
            noGrenadeMessageTimer += Time.deltaTime;
            if (noGrenadeMessageTimer >= 3f)
            {
                showingNoGrenadeMessage = false;
                if (noGrenadeText != null)
                    noGrenadeText.gameObject.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
            Launch();
    }

    void Launch()
    {
        // Vérifier s'il y a des grenades disponibles
        if (currentGrenades <= 0)
        {
            // Afficher le message "Plus de grenade!" pendant 5 secondes
            if (noGrenadeText != null)
            {
                noGrenadeText.SetText("Plus de grenade!");
                noGrenadeText.gameObject.SetActive(true);
                showingNoGrenadeMessage = true;
                noGrenadeMessageTimer = 0f;
            }
            return;
        }

        if (grenadePrefab == null || spawnPoint == null) return;

        // Jouer le son de lancer (optionnel)
        if (launchSound != null)
        {
            AudioSource.PlayClipAtPoint(launchSound, spawnPoint.position, soundVolume);
        }

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
        explosion.explosionSound = explosionSound;
        explosion.soundVolume = soundVolume;

        // Décrémenter le nombre de grenades et mettre à jour l'affichage
        currentGrenades--;
        UpdateGrenadeText();
    }

    // Méthode pour mettre à jour l'affichage du nombre de grenades
    private void UpdateGrenadeText()
    {
        if (grenadeText == null) return;
        grenadeText.SetText($"Grenades: {currentGrenades}");
    }


    public void AddGrenades(int amount)
    {
        currentGrenades += amount;
        UpdateGrenadeText();
        Debug.Log($"[Grenade] +{amount} grenades. Total: {currentGrenades}");
    }


    public int GetGrenadeCount()
    {
        return currentGrenades;
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
    [HideInInspector] public AudioClip explosionSound;
    [HideInInspector] public float soundVolume;

    private bool hasExploded = false; // Flag pour éviter les explosions multiples

    void Start()
    {
        Invoke(nameof(Explode), delay);
    }

    void Explode()
    {
        // S'assurer que l'explosion n'est exécutée qu'une seule fois
        if (hasExploded)
            return;

        hasExploded = true;

        // Annuler tous les Invoke en attente sur ce script
        CancelInvoke();

        Vector3 pos = transform.position;

        // Jouer le son d'explosion
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, pos, soundVolume);
        }

        // Créer l'effet d'explosion si défini
        if (explosionEffect != null)
        {
            GameObject effect = Instantiate(explosionEffect, pos, Quaternion.identity);
            // Ne pas forcer la destruction - l'effet gère lui-même son cycle de vie
        }

        // Force physique
        Collider[] hits = Physics.OverlapSphere(pos, radius);
        foreach (Collider c in hits)
        {
            Rigidbody rb = c.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddExplosionForce(force, pos, radius);
        }

        // Dégâts aux zombies ET au joueur
        Collider[] damageHits = Physics.OverlapSphere(pos, damageRadius);

        // Utiliser un HashSet pour s'assurer qu'on n'applique les dégâts qu'une seule fois par GameObject
        System.Collections.Generic.HashSet<GameObject> damagedObjects = new System.Collections.Generic.HashSet<GameObject>();

        foreach (Collider c in damageHits)
        {
            // Chercher le composant ReceiveDamage sur l'objet ou ses parents
            ReceiveDamage health = c.GetComponent<ReceiveDamage>();
            if (health == null)
                health = c.GetComponentInParent<ReceiveDamage>();

            if (health != null && !damagedObjects.Contains(health.gameObject))
            {
                damagedObjects.Add(health.gameObject);
                health.GetDamage(damage);
            }
        }

        // Désactiver le GameObject avant destruction pour éviter tout déclenchement supplémentaire
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
