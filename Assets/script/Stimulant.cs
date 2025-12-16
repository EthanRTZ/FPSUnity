using System.Collections;
using UnityEngine;
using TMPro;

public class Stimulant : MonoBehaviour
{
    [Header("Paramètres de régénération")]
    public float healthToRegenerate = 30f; // Points de vie à régénérer
    public float regenerationDuration = 3f; // Durée de la régénération en secondes

    [Header("Stimulants system")]
    public int startingStimulants = 2; // Nombre de stimulants au départ
    public TextMeshProUGUI stimulantText; // Texte UI pour afficher le nombre de stimulants
    public TextMeshProUGUI noStimulantText; // Texte UI pour le message "pas de stimulant"

    private ReceiveDamage playerHealth;
    private bool isRegenerating = false;
    private int currentStimulants; // Nombre actuel de stimulants
    private float noStimulantMessageTimer; // Timer pour le message
    private bool showingNoStimulantMessage = false;

    private void Awake()
    {
        currentStimulants = startingStimulants;
        noStimulantMessageTimer = 0f;
        UpdateStimulantText();
        if (noStimulantText != null)
            noStimulantText.gameObject.SetActive(false);
    }

    void Start()
    {
        playerHealth = GetComponent<ReceiveDamage>();
        if (playerHealth == null)
        {
            playerHealth = GetComponentInChildren<ReceiveDamage>();
        }
        if (playerHealth == null)
        {
            playerHealth = GetComponentInParent<ReceiveDamage>();
        }
    }

    void Update()
    {
        if (showingNoStimulantMessage)
        {
            noStimulantMessageTimer += Time.deltaTime;
            if (noStimulantMessageTimer >= 3f)
            {
                showingNoStimulantMessage = false;
                if (noStimulantText != null)
                    noStimulantText.gameObject.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && !isRegenerating)
        {
            UseStimulant();
        }
    }

    void UseStimulant()
    {
        if (currentStimulants <= 0)
        {
            if (noStimulantText != null)
            {
                noStimulantText.SetText("Plus de stimulant!");
                noStimulantText.gameObject.SetActive(true);
                showingNoStimulantMessage = true;
                noStimulantMessageTimer = 0f;
            }
            return;
        }

        if (playerHealth == null || playerHealth.isDead)
        {
            return;
        }

        if (playerHealth.health >= playerHealth.maxHealth)
        {
            return;
        }

        currentStimulants--;
        UpdateStimulantText();
        StartCoroutine(RegenerateHealth());
    }

    private IEnumerator RegenerateHealth()
    {
        isRegenerating = true;

        gameObject.BroadcastMessage("OnDamageTaken", SendMessageOptions.DontRequireReceiver);
        gameObject.SendMessageUpwards("OnDamageTaken", SendMessageOptions.DontRequireReceiver);

        float elapsed = 0f;

        while (elapsed < regenerationDuration)
        {
            float healAmount = (healthToRegenerate / regenerationDuration) * Time.deltaTime;

            if (playerHealth.health < playerHealth.maxHealth)
            {
                playerHealth.health += healAmount;
                playerHealth.health = Mathf.Min(playerHealth.health, playerHealth.maxHealth);

                gameObject.BroadcastMessage("OnHealthChanged", playerHealth.health, SendMessageOptions.DontRequireReceiver);
                gameObject.SendMessageUpwards("OnHealthChanged", playerHealth.health, SendMessageOptions.DontRequireReceiver);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        isRegenerating = false;
    }

    private void UpdateStimulantText()
    {
        if (stimulantText != null)
        {
            stimulantText.SetText($"Stimulants: {currentStimulants}");
        }
    }

    public void AddStimulants(int amount)
    {
        currentStimulants += amount;
        UpdateStimulantText();
    }

    public int GetStimulantCount()
    {
        return currentStimulants;
    }
}
