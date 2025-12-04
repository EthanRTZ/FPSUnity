using UnityEngine;
using TMPro;

namespace DefaultNamespace
{
    public class vie_zombie : MonoBehaviour
    {
        public float destroyDelay = 5f;

        Animator animator;
        Rigidbody[] ragdollBodies;
        Collider[] ragdollColliders;
        Collider mainCollider;
        bool dead = false;

        // Nouveau : référence au TextMeshProUGUI pour afficher la vie
        public TextMeshProUGUI healthText;

        // Cache du maxHealth pour afficher current / max
        private float maxHealthCached = 0f;

        void Awake()
        {
            animator = GetComponent<Animator>();
            ragdollBodies = GetComponentsInChildren<Rigidbody>();
            ragdollColliders = GetComponentsInChildren<Collider>();
            mainCollider = GetComponent<Collider>();
            DisableRagdoll();
        }

        // Nouveau : récupérer le maxHealth et initialiser l'affichage
        void Start()
        {
            var rd = GetComponentInParent<ReceiveDamage>();
            if (rd != null)
            {
                maxHealthCached = rd.maxHealth;
                UpdateHealthText(rd.health);
            }
        }

        void DisableRagdoll()
        {
            foreach (var rb in ragdollBodies)
            {
                rb.isKinematic = true;
            }
            foreach (var col in ragdollColliders)
            {
                if (col == mainCollider) continue;
                col.enabled = false;
            }
            if (mainCollider != null) mainCollider.enabled = true;
            if (animator != null) animator.enabled = true;
        }

        void EnableRagdoll()
        {
            if (animator != null) animator.enabled = false;

            foreach (var col in ragdollColliders)
            {
                col.enabled = true;
            }
            foreach (var rb in ragdollBodies)
            {
                rb.isKinematic = false;
                rb.AddForce(Vector3.up * 1.5f + Random.insideUnitSphere * 0.5f, ForceMode.Impulse);
            }
            if (mainCollider != null) mainCollider.enabled = false;
        }

        // Appelé via BroadcastMessage depuis ReceiveDamage quand on prend un dégât
        public void TakeDamage()
        {
            if (dead) return;
            if (animator != null)
            {
                animator.SetTrigger("Hit");
            }
        }

        // Appelé via BroadcastMessage depuis ReceiveDamage quand la vie <= 0
        public void Defeated()
        {
            if (dead) return;
            dead = true;
            EnableRagdoll();
            Destroy(gameObject, destroyDelay);
        }

        // Appelé via BroadcastMessage("OnHealthChanged", health) depuis ReceiveDamage
        public void OnHealthChanged(float currentHealth)
        {
            UpdateHealthText(currentHealth);
        }

        // Mets à jour le TMP si présent
        private void UpdateHealthText(float currentHealth)
        {
            if (healthText == null) return;

            if (maxHealthCached > 0f)
                healthText.SetText(Mathf.CeilToInt(currentHealth) + " / " + Mathf.CeilToInt(maxHealthCached));
            else
                healthText.SetText(Mathf.CeilToInt(currentHealth).ToString());
        }
    }
}