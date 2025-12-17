using UnityEngine;

namespace DefaultNamespace
{
    public class zombie_melee_attack : MonoBehaviour
    {
        // Dégâts infligés par l'attaque de mêlée
        public float attackDamage = 10f;
        // Temps entre deux attaques (en secondes)
        public float attackCooldown = 1.0f;
        // Rayon de détection des cibles
        public float attackRadius = 2.0f;
        // Layer des cibles (ex : Player)
        public LayerMask targetLayer;
        // Point d'origine de l'attaque (si null, utilise transform)
        public Transform attackPoint;
        // Noms possibles des méthodes de réception de dégâts (SendMessage compatible)
        public string[] damageMethodNames = new string[] { "GetDamage", "TakeDamage", "ApplyDamage" };
        // Animator et nom du trigger d'attaque (optionnel)
        public Animator animator;
        public string attackTrigger = "Attack";
        // Option : n'attaquer qu'une fois par root si plusieurs colliders touchent la zone
        public bool uniquePerRoot = true;
      

        float cooldownTimer = -1f; // Changé de 0f à -1f pour permettre attaque immédiate

        // Référence au composant ReceiveDamage du zombie
        private ReceiveDamage receiveDamage;

        void Reset()
        {
            attackPoint = transform;
        }
        
        void Start()
        {
            // Permettre une attaque immédiate au démarrage
            cooldownTimer = -1f;
            
            // Récupérer le composant ReceiveDamage du zombie
            receiveDamage = GetComponent<ReceiveDamage>();
            if (receiveDamage == null)
            {
                receiveDamage = GetComponentInParent<ReceiveDamage>();
            }
            
          
        }

        void Update()
        {
            // Ne rien faire si le zombie est mort
            if (receiveDamage != null && receiveDamage.health <= 0f)
            {
                
                return;
            }
            
            // Décrémenter le timer
            if (cooldownTimer > 0f)
            {
                cooldownTimer -= Time.deltaTime;
            }
            
            // Vérifier en permanence s'il y a des cibles dans la zone
            Vector3 origin = attackPoint != null ? attackPoint.position : transform.position;
            Collider[] hits = Physics.OverlapSphere(origin, attackRadius, targetLayer);
            
           
    
            // Si des cibles sont détectées ET que le cooldown est écoulé
            if (hits.Length > 0 && cooldownTimer <= 0f)
            {
                Attack(hits);
            }
        }

        void Attack(Collider[] hits)
        {
            
            
            if (animator != null && !string.IsNullOrEmpty(attackTrigger))
            {
                animator.SetTrigger(attackTrigger);
            }

            // Réinitialiser le cooldown pour la prochaine attaque
            cooldownTimer = attackCooldown;

            if (uniquePerRoot)
            {
                var hitRoots = new System.Collections.Generic.HashSet<Transform>();
                foreach (var col in hits)
                {
                    Transform root = col.transform.root;
                    if (hitRoots.Contains(root)) continue;
                    hitRoots.Add(root);
                    
                  
                    ApplyDamageToTarget(root.gameObject);
                }
            }
            else
            {
                foreach (var col in hits)
                {
                   
                    ApplyDamageToTarget(col.gameObject);
                }
            }
        }

        void ApplyDamageToTarget(GameObject target)
        {
            foreach (var method in damageMethodNames)
            {
                target.SendMessageUpwards(method, attackDamage, SendMessageOptions.DontRequireReceiver);
                target.SendMessage(method, attackDamage, SendMessageOptions.DontRequireReceiver);
            }
        }

        void OnDrawGizmosSelected()
        {
            Vector3 origin = attackPoint != null ? attackPoint.position : transform.position;
            Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
            Gizmos.DrawSphere(origin, attackRadius);
        }
    }
}