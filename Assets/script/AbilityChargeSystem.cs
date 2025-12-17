using UnityEngine;
using TMPro;

public class AbilityChargeSystem : MonoBehaviour
{
    public static AbilityChargeSystem Instance { get; private set; }

    [Header("Charge Settings")]
    public int maxCharge = 500;
    public int currentCharge = 0;

    [Header("UI")]
    public TextMeshProUGUI chargeText;
    public Color normalColor = Color.white;
    public Color readyColor = Color.green;

    private bool isReady = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentCharge = 0;
        UpdateUI();
    }

    public void AddCharge(int amount)
    {
        if (currentCharge >= maxCharge)
            return;

        currentCharge += amount;

        if (currentCharge > maxCharge)
        {
            currentCharge = maxCharge;
        }

        if (currentCharge >= maxCharge && !isReady)
        {
            isReady = true;
        }

        UpdateUI();
    }

    public bool IsReady()
    {
        return currentCharge >= maxCharge;
    }

    public void UseAbility()
    {
        if (!IsReady())
            return;

        currentCharge = 0;
        isReady = false;
        UpdateUI();
    }

    public int GetCurrentCharge()
    {
        return currentCharge;
    }

    private void UpdateUI()
    {
        if (chargeText == null)
            return;

        chargeText.SetText($"Capacité: {currentCharge}/{maxCharge}");

        if (isReady)
        {
            chargeText.color = readyColor;
        }
        else
        {
            chargeText.color = normalColor;
        }
    }
}
