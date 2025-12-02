using UnityEngine;

public class move : MonoBehaviour
{
    [Header("Références")]
    public Transform orientation; // assigner dans l'inspecteur ou fallback sur this.transform

    [Header("Accroupi")]
    public float crouchspeed;
    public float crouchYScale;
    private float startYScale;
    
    [Header("Mouvement")]
    public int speed = 5;
    public int sprintspeed = 8;
    public float rotateSpeed = 180;

    [Header("Saut")]
    public float jumpForce = 10;
    public bool canJump = false;
    public float groundCheckDistance = 0.25f;

    [Header("WallJump")]
    public float wallJumpUpForce = 8f;
    public float wallJumpHorizontalForce = 6f;
    public float wallJumpCooldown = 0.25f;
    private float lastWallJumpTime = -10f;
    public float wallCheckPadding = 0.05f;
    
    [Header("Camera")]
    float xRotation = 0f;
    float yRotation = 0f;
    public float topClamp = -90f; 
    public float bottomClamp = 90f;

    private Rigidbody rb;
    private Vector3 inputDir;
    private bool isSprinting;

    // wall contact tracking
    private bool isTouchingWall = false;
    private Vector3 wallNormal = Vector3.zero;

    private bool canWallJump = false;

    // suivi du double-saut
    private bool hasDoubleJumped = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // fallback : si orientation non assignée dans l'inspecteur, utiliser ce transform
        if (orientation == null)
            orientation = this.transform;

        // initial ground check rapide
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance + 0.1f))
            canJump = true;
        hasDoubleJumped = false;

        startYScale = transform.localScale.y;
    }

    void Update()
    {
        // Input & rotation (ne touche pas à la caméra)
        float h = 0f;
        if (Input.GetKey(KeyCode.D)) h = 1f;
        else if (Input.GetKey(KeyCode.A)) h = -1f;

        float v = 0f;
        if (Input.GetKey(KeyCode.W)) v = 1f;
        else if (Input.GetKey(KeyCode.S)) v = -1f;

        isSprinting = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isSprinting ? sprintspeed : speed;
        Vector3 rawDir = new Vector3(h, 0f, v);
        inputDir = rawDir.normalized * currentSpeed;

        float mouseX = Input.GetAxis("Mouse X")* rotateSpeed * Time.fixedDeltaTime;
        float mouseY = Input.GetAxis("Mouse Y")* rotateSpeed * Time.fixedDeltaTime;
        
        xRotation -= mouseY;
        
        xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);
        
        yRotation -= mouseX;
        
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        // Saut : wall-jump prioritaire, sinon saut au sol, sinon double-saut si disponible
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (rb == null) return;

            // 1) wall-jump prioritaire si contact mural et cooldown écoulé
            if (isTouchingWall && Time.time > lastWallJumpTime + wallJumpCooldown)
            {
                Vector3 away = new Vector3(wallNormal.x, 0f, wallNormal.z).normalized;
                Vector3 jumpVel = away * wallJumpHorizontalForce + Vector3.up * wallJumpUpForce;
                rb.linearVelocity = new Vector3(jumpVel.x, jumpVel.y, jumpVel.z);
                lastWallJumpTime = Time.time;
                canJump = false;
                isTouchingWall = false;
                // permettre le deuxième saut après wall-jump
                hasDoubleJumped = false;
            }
            // 2) saut au sol
            else if (canJump)
            {
                Vector3 vel = rb.linearVelocity;
                rb.linearVelocity = new Vector3(vel.x, jumpForce, vel.z);
                canJump = false;
                hasDoubleJumped = false;
            }
            // 3) double-saut si pas encore utilisé
            else if (!hasDoubleJumped)
            {
                Vector3 vel = rb.linearVelocity;
                rb.linearVelocity = new Vector3(vel.x, jumpForce, vel.z);
                hasDoubleJumped = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // Ground check continu
        bool grounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance + 0.1f);
        if (grounded && rb.linearVelocity.y <= 0.05f)
        {
            canJump = true;
            hasDoubleJumped = false;
        }

        MovePlayer();
    }

    private void MovePlayer()
    {
        if (rb == null) return;

        // direction monde à partir de orientation (fallback géré en Start)
        Transform refer = (orientation != null) ? orientation : this.transform;
        Vector3 worldDir = refer.forward * inputDir.z + refer.right * inputDir.x;

        // appliquer la composante horizontale via velocity tout en conservant la verticale physique
        Vector3 targetVel = worldDir;
        Vector3 newVel = new Vector3(targetVel.x, rb.linearVelocity.y, targetVel.z);
        rb.linearVelocity = newVel;

        // anti-tunneling simple : si il y a un obstacle très proche dans la direction de déplacement, annuler la composante horizontale
        if (worldDir.sqrMagnitude > 0.001f)
        {
            Vector3 stepDirNorm = worldDir.normalized;
            float stepDist = worldDir.magnitude * Time.fixedDeltaTime;
            if (Physics.Raycast(transform.position, stepDirNorm, out RaycastHit hit, stepDist + wallCheckPadding))
            {
                if (hit.collider.CompareTag("mur"))
                {
                    // stopper la vitesse horizontale vers le mur
                    Vector3 horizontalVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                    Vector3 blockedDir = Vector3.Project(horizontalVel, stepDirNorm);
                    Vector3 remaining = horizontalVel - blockedDir;
                    rb.linearVelocity = new Vector3(remaining.x, rb.linearVelocity.y, remaining.z);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Si on touche une surface, vérifier la normale principale pour déterminer sol ou mur
        foreach (ContactPoint cp in collision.contacts)
        {
            if (cp.normal.y > 0.5f)
            {
                // contact sol/plancher
                canJump = true;
                hasDoubleJumped = false;
                break;
            }
        }

        if (collision.gameObject.CompareTag("mur"))
        {
            foreach (ContactPoint cp in collision.contacts)
            {
                if (cp.normal.y < 0.5f)
                {
                    isTouchingWall = true;
                    wallNormal = cp.normal;
                    canWallJump = true;
                    break;
                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("mur"))
        {
            foreach (ContactPoint cp in collision.contacts)
            {
                if (cp.normal.y < 0.5f)
                {
                    isTouchingWall = true;
                    wallNormal = cp.normal;
                    break;
                }
            }
        }
        else
        {
            foreach (ContactPoint cp in collision.contacts)
            {
                if (cp.normal.y > 0.5f)
                {
                    canJump = true;
                    break;
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("mur"))
        {
            isTouchingWall = false;
            wallNormal = Vector3.zero;
            canWallJump = false;
        }

        if (collision.gameObject.CompareTag("sol") || collision.gameObject.CompareTag("box"))
        {
            canJump = false;
        }
    }
}
