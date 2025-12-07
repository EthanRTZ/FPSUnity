using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    public GameObject player;
    
    // Agent de Navigation
    NavMeshAgent navMeshAgent;
    
    // Actions possibles
    const string STAND_STATE = "Stand";
    const string TAKE_DAMAGE_STATE = "Damage";
    public const string DEFEATED_STATE = "Defeated";
    public const string WALK_STATE = "Walk";
    const string FALLING_STATE = "Falling";
    
    // Mémorise l'action actuelle
    public string currentAction;
    
    // Détection de vide et chute
    public float cliffCheckDistance = 2f;
    public float cliffCheckHeight = 5f;
    public float fallHeightThreshold = 2f; // Hauteur minimum pour autoriser la chute
    public float maxFallDistance = 10f; // Distance maximum de chute autorisée
    
    // Détection de blocage
    private Vector3 lastPosition;
    private float stuckTimer = 0f;
    private float stuckThreshold = 2f;
    private float minMovementDistance = 0.2f;
    
    // Physique pour la chute
    private Rigidbody rb;
    private bool isFalling = false;
    
    private void Awake()
    {
        currentAction = STAND_STATE;
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<move>().gameObject;
        
        // Ajouter ou récupérer Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        // Configuration Rigidbody
        rb.isKinematic = true; // Kinematic par défaut, désactivé pendant la chute
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        
        // Configuration NavMeshAgent
        if (navMeshAgent != null)
        {
            navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            navMeshAgent.autoBraking = true;
        }
        
        lastPosition = transform.position;
    }
    
    private void Update()
    {
        // si la créature est défaite
        if (currentAction == DEFEATED_STATE)
        {
            if (navMeshAgent.enabled)
                navMeshAgent.ResetPath();
            return;
        }
        
        // Si la créature reçoit des dommages
        if (currentAction == TAKE_DAMAGE_STATE)
        {
            if (navMeshAgent.enabled)
                navMeshAgent.ResetPath();
            TakingDamage();
            return;
        }
        
        // Si en train de tomber
        if (currentAction == FALLING_STATE)
        {
            CheckLanding();
            return;
        }
        
        if (player != null && navMeshAgent.isOnNavMesh)
        {
            CheckIfStuck();
            
            if (MovingToTarget())
            {
                return;
            }
        }
    }
    
    // La créature attend
    private void Stand()
    {
        currentAction = STAND_STATE;
    }
    
    public void TakeDamage()
    {
        currentAction = TAKE_DAMAGE_STATE;
    }
    
    public void Defeated()
    {
        currentAction = DEFEATED_STATE;
    }
    
    // Permet de surveiller l'animation lorsque l'on prend un dégât
    private void TakingDamage()
    {
        // Logique simplifiée sans animation
        Stand();
    }
    
    private void CheckIfStuck()
    {
        // Si le zombie se déplace
        if (currentAction == WALK_STATE && navMeshAgent.hasPath)
        {
            float distanceMoved = Vector3.Distance(transform.position, lastPosition);
            
            // Si le zombie n'a presque pas bougé
            if (distanceMoved < minMovementDistance)
            {
                stuckTimer += Time.deltaTime;
                
                // Si bloqué et que le joueur est en contrebas, essayer de tomber
                if (stuckTimer >= stuckThreshold)
                {
                    if (ShouldFallToPlayer())
                    {
                        StartFalling();
                    }
                    else
                    {
                        navMeshAgent.ResetPath();
                    }
                    stuckTimer = 0f;
                }
            }
            else
            {
                stuckTimer = 0f;
            }
        }
        
        lastPosition = transform.position;
    }
    
    private bool ShouldFallToPlayer()
    {
        if (player == null) return false;
        
        float heightDifference = transform.position.y - player.transform.position.y;
        
        // Le joueur est en contrebas et la hauteur est raisonnable
        if (heightDifference > fallHeightThreshold && heightDifference < maxFallDistance)
        {
            // Vérifier qu'il y a du vide devant (pas de NavMesh)
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            Vector3 checkPosition = transform.position + directionToPlayer * cliffCheckDistance;
            
            NavMeshHit hit;
            if (!NavMesh.SamplePosition(checkPosition, out hit, 2f, NavMesh.AllAreas))
            {
                // Vérifier qu'il y a un sol en bas
                if (Physics.Raycast(checkPosition + Vector3.up * 0.5f, Vector3.down, out RaycastHit groundHit, heightDifference + 2f))
                {
                    return true;
                }
            }
        }
        
        return false;
    }
    
    private void StartFalling()
    {
        currentAction = FALLING_STATE;
        isFalling = true;
        
        // Désactiver NavMeshAgent et activer la physique
        navMeshAgent.enabled = false;
        rb.isKinematic = false;
        
        // Donner une petite impulsion vers le joueur
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        Vector3 pushForce = new Vector3(directionToPlayer.x, 0, directionToPlayer.z) * 2f;
        rb.AddForce(pushForce, ForceMode.Impulse);
    }
    
    private void CheckLanding()
    {
        // Vérifier si le zombie a atterri
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.3f))
        {
            // Atterri !
            StopFalling();
        }
    }
    
    private void StopFalling()
    {
        isFalling = false;
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        
        // Repositionner sur le NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            navMeshAgent.enabled = true;
            Stand();
        }
        else
        {
            // Pas de NavMesh trouvé, réessayer dans un instant
            Invoke("StopFalling", 0.2f);
        }
    }
    
    private bool IsCliffAhead()
    {
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        Vector3 checkPosition = transform.position + directionToPlayer * cliffCheckDistance;
        
        // Vérifier la hauteur du joueur par rapport au zombie
        float heightDifference = transform.position.y - player.transform.position.y;
        
        // Si le joueur est significativement en contrebas, autoriser la chute
        if (heightDifference > fallHeightThreshold && heightDifference < maxFallDistance)
        {
            NavMeshHit hit;
            if (!NavMesh.SamplePosition(checkPosition, out hit, 2f, NavMesh.AllAreas))
            {
                // Pas de NavMesh devant et joueur en contrebas = autoriser la chute
                if (Physics.Raycast(checkPosition + Vector3.up * 0.5f, Vector3.down, out RaycastHit groundHit, heightDifference + 2f))
                {
                    StartFalling();
                    return false; // Ne pas bloquer le mouvement
                }
            }
        }
        
        // Vérifier s'il y a du sol devant (chute mortelle)
        if (!Physics.Raycast(checkPosition + Vector3.up * 0.5f, Vector3.down, cliffCheckHeight))
        {
            // Pas de sol et pas de condition de chute valide = bloquer
            return true;
        }
        
        // Vérifier le NavMesh
        NavMeshHit navHit;
        if (!NavMesh.SamplePosition(checkPosition, out navHit, 2f, NavMesh.AllAreas))
        {
            // Pas de NavMesh mais pas dans les conditions de chute autorisée
            if (heightDifference <= fallHeightThreshold)
            {
                return true;
            }
        }
        
        return false;
    }
    
    private bool MovingToTarget()
    {
        if (!navMeshAgent.isOnNavMesh)
            return false;
        
        // Vérifier s'il y a un précipice avant de continuer
        if (IsCliffAhead())
        {
            navMeshAgent.ResetPath();
            Stand();
            return false;
        }
        
        // Calculer le chemin vers le joueur
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(transform.position, player.transform.position, NavMesh.AllAreas, path))
        {
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                navMeshAgent.SetPath(path);
            }
            else
            {
                // Pas de chemin complet, vérifier si on peut tomber
                if (ShouldFallToPlayer())
                {
                    StartFalling();
                    return true;
                }
                
                Stand();
                return false;
            }
        }
        
        if (navMeshAgent.pathPending)
            return true;
            
        if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        {
            if (currentAction != WALK_STATE)
                Walk();
        }
        else
        {
            // Si arrivé à bonne distance, regarde vers le joueur
            RotateToTarget(player.transform);
            return false;
        }
        
        return true;
    }
    
    // Walk = Marcher
    private void Walk()
    {
        currentAction = WALK_STATE;
    }
    
    // Permet de tout le temps regarder en direction de la cible
    private void RotateToTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3f);
    }
}
