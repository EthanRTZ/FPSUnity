using System.Collections;
using UnityEngine;

public class Stimulant : MonoBehaviour
{
    [Header("Paramètres de régénération")]
    public float healthToRegenerate = 30f; // Points de vie à régénérer
    public float regenerationDuration = 3f; // Durée de la régénération en secondes
    
    private ReceiveDamage playerHealth;
    private bool isRegenerating = false;

    void Start()
    {
        // Récupérer le composant ReceiveDamage du joueur
        playerHealth = GetComponent<ReceiveDamage>();
        
    }

    void Update()
    {
        // Vérifier si la touche Q est appuyée
        if (Input.GetKeyDown(KeyCode.Q) && !isRegenerating)
        {
            if (playerHealth != null && !playerHealth.isDead && playerHealth.health < playerHealth.maxHealth)
            {
                StartCoroutine(RegenerateHealth());
            }
        }
    }

    private IEnumerator RegenerateHealth()
    {
        isRegenerating = true;
        
        Debug.Log($"[Stimulant] Début de régénération de {healthToRegenerate} PV sur {regenerationDuration} secondes");
        
        // Réinitialiser le timer de régénération passive (empêcher les deux régénérations en même temps)
        gameObject.BroadcastMessage("OnDamageTaken", SendMessageOptions.DontRequireReceiver);
        gameObject.SendMessageUpwards("OnDamageTaken", SendMessageOptions.DontRequireReceiver);
        
        float elapsed = 0f;
        float totalHealed = 0f;
        
        while (elapsed < regenerationDuration)
        {
            // Calculer la quantité de vie à régénérer cette frame
            float healAmount = (healthToRegenerate / regenerationDuration) * Time.deltaTime;
            
            // Vérifier qu'on ne dépasse pas le maximum
            if (playerHealth.health < playerHealth.maxHealth)
            {
                playerHealth.health += healAmount;
                playerHealth.health = Mathf.Min(playerHealth.health, playerHealth.maxHealth);
                totalHealed += healAmount;
                
                // Notifier les autres composants du changement de vie
                gameObject.BroadcastMessage("OnHealthChanged", playerHealth.health, SendMessageOptions.DontRequireReceiver);
                gameObject.SendMessageUpwards("OnHealthChanged", playerHealth.health, SendMessageOptions.DontRequireReceiver);
            }
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        Debug.Log($"[Stimulant] Régénération terminée. Total régénéré: {totalHealed} PV. Vie actuelle: {playerHealth.health}/{playerHealth.maxHealth}");
        
        isRegenerating = false;
    }
}
