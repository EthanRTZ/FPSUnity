using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Vie: MonoBehaviour
{
    public float vieMax = 100f;
    public float vie = 100f;
    
    public Image vieBar;
    public TextMeshProUGUI vieText;
    
    private ReceiveDamage receiveDamage;
    
    void Start()
    {
        // Récupère le composant ReceiveDamage sur ce GameObject ou ses parents
        receiveDamage = GetComponentInParent<ReceiveDamage>();
        if (receiveDamage == null)
        {
            receiveDamage = GetComponent<ReceiveDamage>();
        }
        
        if (receiveDamage != null)
        {
            vieMax = receiveDamage.maxHealth;
            vie = receiveDamage.health;
        }
        
        UpdateUI();
    }
    
    void Update()
    {
        UpdateUI();
    }
    
    // Appelé par ReceiveDamage via BroadcastMessage/SendMessageUpwards
    void OnHealthChanged(float newHealth)
    {
        vie = newHealth;
        if (receiveDamage != null)
        {
            vieMax = receiveDamage.maxHealth;
        }
        UpdateUI();
    }
    
    void UpdateUI()
    {
        if (vieBar != null)
        {
            vieBar.fillAmount = vieMax > 0 ? vie / vieMax : 0f;
        }
        if (vieText != null)
        {
            vieText.text = vie.ToString("F0") + " / " + vieMax.ToString("F0");
        }
    }
}
