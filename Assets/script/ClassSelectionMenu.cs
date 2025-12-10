using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClassSelectionMenu : MonoBehaviour
{
    public WeaponClass[] availableClasses;
    public TextMeshProUGUI classInfoText;
    
    // Références optionnelles aux boutons (assignables dans l'Inspector)
    public Button previousButton;
    public Button nextButton;
    public Button confirmButton;
    public Button backButton;
    
    private int currentClassIndex = 0;
    
    private void Awake()
    {
        Debug.Log("=== ClassSelectionMenu Awake ===");
        
        // Créer EventSystem si manquant
        EnsureEventSystem();
        
        if (classInfoText == null)
        {
            Debug.LogWarning("[ClassSelectionMenu] classInfoText est NULL dans l'Inspector. Tentative de recherche...");
            var go = GameObject.Find("ClassInfoText");
            if (go != null)
            {
                classInfoText = go.GetComponent<TextMeshProUGUI>();
                if (classInfoText != null)
                {
                    Debug.Log("[ClassSelectionMenu] ✓ Auto-assign ClassInfoText trouvé et assigné.");
                }
                else
                {
                    Debug.LogError("[ClassSelectionMenu] ✗ GameObject 'ClassInfoText' trouvé mais n'a pas de composant TextMeshProUGUI !");
                }
            }
            else
            {
                Debug.LogError("[ClassSelectionMenu] ✗ GameObject 'ClassInfoText' introuvable dans la scène !");
            }
        }
        else
        {
            Debug.Log("[ClassSelectionMenu] ✓ classInfoText est assigné dans l'Inspector.");
        }

        if (availableClasses == null || availableClasses.Length == 0)
        {
            Debug.LogError("[ClassSelectionMenu] ✗ availableClasses est VIDE ou NULL ! Assignez des WeaponClass dans l'Inspector !");
        }
        else
        {
            Debug.Log($"[ClassSelectionMenu] ✓ {availableClasses.Length} classe(s) disponible(s).");
            for (int i = 0; i < availableClasses.Length; i++)
            {
                if (availableClasses[i] == null)
                {
                    Debug.LogError($"[ClassSelectionMenu] ✗ Element {i} de availableClasses est NULL !");
                }
                else
                {
                    Debug.Log($"[ClassSelectionMenu]   - [{i}] {availableClasses[i].className}");
                }
            }
        }
        
        // Auto-trouver et connecter les boutons
        AutoFindAndConnectButtons();
    }
    
    private void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
            Debug.Log("[ClassSelectionMenu] ✓ EventSystem créé automatiquement.");
        }
        else
        {
            Debug.Log("[ClassSelectionMenu] ✓ EventSystem déjà présent.");
        }
    }
    
    private void AutoFindAndConnectButtons()
    {
        Debug.Log("=== Auto-détection des boutons ===");
        
        // Auto-trouver les boutons par nom s'ils ne sont pas assignés
        if (previousButton == null)
        {
            GameObject go = GameObject.Find("PreviousButton");
            if (go != null) previousButton = go.GetComponent<Button>();
        }
        
        if (nextButton == null)
        {
            GameObject go = GameObject.Find("NextButton");
            if (go != null) nextButton = go.GetComponent<Button>();
        }
        
        if (confirmButton == null)
        {
            GameObject go = GameObject.Find("ConfirmButton");
            if (go != null) confirmButton = go.GetComponent<Button>();
        }
        
        if (backButton == null)
        {
            GameObject go = GameObject.Find("BackButton");
            if (go != null) backButton = go.GetComponent<Button>();
        }
        
        // Connecter les boutons aux fonctions
        if (previousButton != null)
        {
            previousButton.onClick.RemoveAllListeners();
            previousButton.onClick.AddListener(PreviousClass);
            Debug.Log("[ClassSelectionMenu] ✓ PreviousButton connecté → PreviousClass()");
        }
        else
        {
            Debug.LogWarning("[ClassSelectionMenu] ✗ PreviousButton non trouvé ! Nommez votre bouton 'PreviousButton' ou assignez-le dans l'Inspector.");
        }
        
        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(NextClass);
            Debug.Log("[ClassSelectionMenu] ✓ NextButton connecté → NextClass()");
        }
        else
        {
            Debug.LogWarning("[ClassSelectionMenu] ✗ NextButton non trouvé ! Nommez votre bouton 'NextButton' ou assignez-le dans l'Inspector.");
        }
        
        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(ConfirmClass);
            Debug.Log("[ClassSelectionMenu] ✓ ConfirmButton connecté → ConfirmClass()");
        }
        else
        {
            Debug.LogWarning("[ClassSelectionMenu] ✗ ConfirmButton non trouvé ! Nommez votre bouton 'ConfirmButton' ou assignez-le dans l'Inspector.");
        }
        
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(BackToMenu);
            Debug.Log("[ClassSelectionMenu] ✓ BackButton connecté → BackToMenu()");
        }
        else
        {
            Debug.LogWarning("[ClassSelectionMenu] ✗ BackButton non trouvé ! Nommez votre bouton 'BackButton' ou assignez-le dans l'Inspector.");
        }
    }

    private void Start()
    {
        Debug.Log("=== ClassSelectionMenu Start ===");
        if (availableClasses != null && availableClasses.Length > 0)
        {
            DisplayClassInfo(0);
        }
        else
        {
            Debug.LogError("[ClassSelectionMenu] Impossible d'afficher la classe par défaut : availableClasses vide.");
        }
    }
    
    public void NextClass()
    {
        Debug.Log("====================================");
        Debug.Log("[ClassSelectionMenu] 🎯 NextClass() APPELÉ !");
        Debug.Log("====================================");
        
        if (availableClasses == null || availableClasses.Length == 0)
        {
            Debug.LogWarning("[ClassSelectionMenu] NextClass impossible : availableClasses vide.");
            return;
        }
        
        int oldIndex = currentClassIndex;
        currentClassIndex = (currentClassIndex + 1) % availableClasses.Length;
        Debug.Log($"[ClassSelectionMenu] Index changé de {oldIndex} vers {currentClassIndex}");
        DisplayClassInfo(currentClassIndex);
    }
    
    public void PreviousClass()
    {
        Debug.Log("====================================");
        Debug.Log("[ClassSelectionMenu] 🎯 PreviousClass() APPELÉ !");
        Debug.Log("====================================");
        
        if (availableClasses == null || availableClasses.Length == 0)
        {
            Debug.LogWarning("[ClassSelectionMenu] PreviousClass impossible : availableClasses vide.");
            return;
        }
        
        int oldIndex = currentClassIndex;
        currentClassIndex--;
        if (currentClassIndex < 0) currentClassIndex = availableClasses.Length - 1;
        Debug.Log($"[ClassSelectionMenu] Index changé de {oldIndex} vers {currentClassIndex}");
        DisplayClassInfo(currentClassIndex);
    }
    
    private void DisplayClassInfo(int index)
    {
        Debug.Log($"[ClassSelectionMenu] DisplayClassInfo appelé pour index {index}");
        
        if (availableClasses == null || availableClasses.Length == 0)
        {
            Debug.LogError("[ClassSelectionMenu] DisplayClassInfo : availableClasses vide !");
            return;
        }
        
        if (index < 0 || index >= availableClasses.Length)
        {
            Debug.LogError($"[ClassSelectionMenu] Index {index} hors limites (max: {availableClasses.Length - 1})");
            return;
        }
        
        WeaponClass wc = availableClasses[index];
        if (wc == null)
        {
            Debug.LogError($"[ClassSelectionMenu] WeaponClass à l'index {index} est NULL !");
            if (classInfoText != null) 
            {
                classInfoText.text = "❌ Classe manquante";
                Debug.Log("[ClassSelectionMenu] Texte mis à jour : 'Classe manquante'");
            }
            return;
        }
        
        if (classInfoText != null)
        {
            string newText = $"<b>{wc.className}</b>\n\n" +
                            $"Dégâts: {wc.damage}\n" +
                            $"Chargeur: {wc.magazineSize}\n" +
                            $"Portée: {wc.range}m\n" +
                            $"Cadence: {wc.timeBetweenShooting}s";
            classInfoText.text = newText;
            Debug.Log($"[ClassSelectionMenu] ✓ Texte mis à jour avec : {wc.className}");
        }
        else
        {
            Debug.LogError("[ClassSelectionMenu] ✗ classInfoText est NULL — IMPOSSIBLE d'afficher les infos !");
        }
    }
    
    public void ConfirmClass()
    {
        Debug.Log("====================================");
        Debug.Log("[ClassSelectionMenu] 🎯 ConfirmClass() APPELÉ !");
        Debug.Log("====================================");
        
        if (availableClasses == null || availableClasses.Length == 0)
        {
            Debug.LogError("[ClassSelectionMenu] ConfirmClass : Aucune classe disponible !");
            return;
        }

        var chosen = availableClasses[currentClassIndex];
        if (chosen == null)
        {
            Debug.LogError($"[ClassSelectionMenu] ConfirmClass : La classe à l'index {currentClassIndex} est null !");
            return;
        }

        Debug.Log($"[ClassSelectionMenu] Classe choisie : {chosen.className}");

        ClassManager manager = FindObjectOfType<ClassManager>();
        if (manager == null)
        {
            Debug.LogWarning("[ClassSelectionMenu] ClassManager introuvable. Création dynamique...");
            GameObject go = new GameObject("ClassManager");
            manager = go.AddComponent<ClassManager>();
            Debug.Log("[ClassSelectionMenu] ClassManager créé.");
        }
        else
        {
            Debug.Log("[ClassSelectionMenu] ClassManager trouvé.");
        }
        
        manager.SelectClass(chosen);
        Debug.Log($"[ClassSelectionMenu] Classe enregistrée dans ClassManager : {chosen.className}");

        Debug.Log("[ClassSelectionMenu] Chargement de la scène Prototype Map (index 1)...");
        SceneManager.LoadScene(1);
    }

    public void BackToMenu()
    {
        Debug.Log("====================================");
        Debug.Log("[ClassSelectionMenu] 🎯 BackToMenu() APPELÉ !");
        Debug.Log("====================================");
        Debug.Log("[ClassSelectionMenu] Chargement de la scène menu (index 0)...");
        SceneManager.LoadScene(0);
    }
}

