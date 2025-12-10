using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Vie: MonoBehaviour
{
    // Référence au GameObject du joueur (à assigner dans l'inspecteur)
    public GameObject player;
    
    public float vieMax = 100f;
    public float vie = 100f;
    
    public Image vieBar;
    public TextMeshProUGUI vieText;
    
    [Header("Régénération Passive")]
    public float passiveRegenDelay = 10f; // Délai avant la régénération (en secondes)
    public float passiveRegenRate = 1f; // Points de vie régénérés par seconde
    
    private ReceiveDamage receiveDamage;
    private float timeSinceLastDamage = 0f;
    private float lastKnownHealth = 0f;
    
    void Start()
    {
        Debug.Log($"[Vie] Start appelé sur {gameObject.name}");
        
        // Si player n'est pas assigné, essayer de trouver le GameObject "Player"
        if (player == null)
        {
            player = GameObject.Find("Player");
            if (player == null)
            {
                Debug.LogError("[Vie] GameObject Player introuvable ! Assignez-le dans l'inspecteur.");
                return;
            }
        }
        
        // Récupère le composant ReceiveDamage du joueur
        receiveDamage = player.GetComponent<ReceiveDamage>();
        if (receiveDamage == null)
        {
            receiveDamage = player.GetComponentInChildren<ReceiveDamage>();
        }
        
        if (receiveDamage != null)
        {
            vieMax = receiveDamage.maxHealth;
            vie = receiveDamage.health;
            lastKnownHealth = vie;
            timeSinceLastDamage = 0f;
            Debug.Log($"[Vie] ReceiveDamage trouvé sur {player.name} - Vie: {vie}/{vieMax}");
        }
        else
        {
            Debug.LogError($"[Vie] ReceiveDamage non trouvé sur {player.name} !");
        }
        
        // Vérifier les références UI
        if (vieBar == null)
        {
            Debug.LogError($"[Vie] vieBar (Image) n'est pas assignée dans l'inspecteur sur {gameObject.name} !");
        }
        if (vieText == null)
        {
            Debug.LogWarning($"[Vie] vieText (TextMeshProUGUI) n'est pas assignée dans l'inspecteur sur {gameObject.name} !");
        }
        
        UpdateUI();
    }
    
    void Update()
    {
        // Surveille en continu la vie du joueur
        if (receiveDamage != null)
        {
            float newHealth = receiveDamage.health;
            
            // Détection de changement de vie
            if (newHealth != vie)
            {
                // Si la vie a diminué, c'est un dégât
                if (newHealth < vie)
                {
                    timeSinceLastDamage = 0f;
                    Debug.Log($"[Vie] Dégât détecté - Timer de régénération réinitialisé");
                }
                
                Debug.Log($"[Vie] Changement détecté - Ancienne vie: {vie} - Nouvelle vie: {newHealth}");
                vie = newHealth;
                vieMax = receiveDamage.maxHealth;
                lastKnownHealth = vie;
                UpdateUI();
            }
            
            // Régénération passive
            if (vie < vieMax && !receiveDamage.isDead)
            {
                timeSinceLastDamage += Time.deltaTime;
                
                // Si le délai de régénération est écoulé
                if (timeSinceLastDamage >= passiveRegenDelay)
                {
                    // Régénérer progressivement
                    float regenAmount = passiveRegenRate * Time.deltaTime;
                    receiveDamage.health += regenAmount;
                    receiveDamage.health = Mathf.Min(receiveDamage.health, receiveDamage.maxHealth);
                    
                    vie = receiveDamage.health;
                    
                    // Notifier les autres composants du changement de vie
                    if (player != null)
                    {
                        player.BroadcastMessage("OnHealthChanged", receiveDamage.health, SendMessageOptions.DontRequireReceiver);
                        player.SendMessageUpwards("OnHealthChanged", receiveDamage.health, SendMessageOptions.DontRequireReceiver);
                    }
                    
                    UpdateUI();
                }
            }
        }
    }
    
    // Appelé par ReceiveDamage quand des dégâts sont reçus
    void OnDamageTaken()
    {
        timeSinceLastDamage = 0f;
        Debug.Log($"[Vie] OnDamageTaken - Timer de régénération réinitialisé et régénération arrêtée");
    }

    void UpdateUI()
    {
        if (vieBar != null)
        {
            float fillAmount = vieMax > 0 ? vie / vieMax : 0f;
            vieBar.fillAmount = fillAmount;
            Debug.Log($"[Vie] vieBar.fillAmount mis à {fillAmount} ({vie}/{vieMax})");
        }
        else
        {
            Debug.LogError("[Vie] vieBar est null, impossible de mettre à jour l'UI !");
        }
        
        if (vieText != null)
        {
            vieText.text = vie.ToString("F0") + " / " + vieMax.ToString("F0");
        }
    }
}
