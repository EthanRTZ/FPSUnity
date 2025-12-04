using UnityEngine;

public class ReceiveDamage : MonoBehaviour
{
    // Maximum de points de vie
    public float maxHealth = 100f;
    
    // Points de vie actuels
    public float health = 0f;
    
    // Après avoir reçu un dégât :
    // La créature est invulnérable quelques instants
    public bool isInvulnerable;

    // Indique si l'entité est déjà morte
    private bool isDead = false;
    
    // Temps d'invulnérabilité
    public float invulnerabilityTime;
    
    // Temps depuis le dernier dégât
    private float timeSinceLastHit = 0.0f;
    
    void Start()
    {
        // Au début : Points de vie actuels = Maximum de points de vie
        health = maxHealth;
        
        isInvulnerable = false;
        isDead = false;
    }
    
    void Update()
    {
        if (isInvulnerable)
        {
            timeSinceLastHit += Time.deltaTime;
            
            if (timeSinceLastHit > invulnerabilityTime)
            {
                timeSinceLastHit = 0.0f;
                isInvulnerable = false;
            }
        }
    }
    
    // Surcharge pour compatibilité si d'autres scripts envoient un int
    public void GetDamage(int damage)
    {
        GetDamage((float)damage);
    }

    // Permet de recevoir des dommages (float pour points de vie)
    public void GetDamage(float damage)
    {
        // Ne rien faire si déjà mort
        if (isDead) return;

        if (isInvulnerable)
            return;
        
        isInvulnerable = true;

        // Réinitialise le timer d'invulnérabilité au moment du hit
        timeSinceLastHit = 0.0f;
        
        // Applique les dommages aux points de vie actuels
        health -= damage;

        // S'assurer que la vie ne tombe pas sous zéro
        health = Mathf.Max(0f, health);

        // Informer les composants (enfant/parent) du changement de vie
        // On envoie vers les enfants ET vers les parents pour couvrir toutes les configurations
        gameObject.BroadcastMessage("OnHealthChanged", health, SendMessageOptions.DontRequireReceiver);
        gameObject.SendMessageUpwards("OnHealthChanged", health, SendMessageOptions.DontRequireReceiver);
        
        // S'il reste des points de vie
        if (health > 0f)
        {
            // Notify components on this GameObject AND children, et aussi parents
            gameObject.BroadcastMessage("TakeDamage", SendMessageOptions.DontRequireReceiver);
            gameObject.SendMessageUpwards("TakeDamage", SendMessageOptions.DontRequireReceiver);
        }
        // Sinon
        else
        {
            // Marque comme mort pour éviter double traitement
            isDead = true;
            // Notify components on this GameObject AND children, et aussi parents
            gameObject.BroadcastMessage("Defeated", SendMessageOptions.DontRequireReceiver);
            gameObject.SendMessageUpwards("Defeated", SendMessageOptions.DontRequireReceiver);
        }
    }
}
