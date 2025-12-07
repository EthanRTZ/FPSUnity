using UnityEngine;

public class move : MonoBehaviour
{
    [Header("Références")]
    public Transform orientation;

    [Header("Accroupi")]
    public float crouchspeed = 2.5f;
    public float crouchYScale = 0.5f;
    private float startYScale;
    private bool isCrouching = false;
    
    [Header("Glissade")]
    public float slideForce = 15f;
    public float slideDuration = 1.5f;
    public float slideYScale = 0.5f;
    public float slideFriction = 0.2f; // Friction appliquée pendant la glissade
    private bool isSliding = false;
    private float slideTimer = 0f;
    private Vector3 slideDirection;
    
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
    
    private Rigidbody rb;
    private Vector3 inputDir;
    private bool isSprinting;

    private bool isTouchingWall = false;
    private Vector3 wallNormal = Vector3.zero;
    private bool canWallJump = false;
    private bool hasDoubleJumped = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        if (orientation == null)
            orientation = this.transform;

        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance + 0.1f))
            canJump = true;
        hasDoubleJumped = false;

        startYScale = transform.localScale.y;
    }

    void Update()
    {
        // Input mouvement
        float h = 0f;
        if (Input.GetKey(KeyCode.D)) h = 1f;
        else if (Input.GetKey(KeyCode.A)) h = -1f;

        float v = 0f;
        if (Input.GetKey(KeyCode.W)) v = 1f;
        else if (Input.GetKey(KeyCode.S)) v = -1f;

        isSprinting = Input.GetKey(KeyCode.LeftShift);
        
        // Gestion de la glissade
        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            // Arrêter la glissade si Ctrl est relâché OU timer écoulé OU pas au sol
            if (Input.GetKeyUp(KeyCode.LeftControl) || slideTimer <= 0f || !canJump)
            {
                StopSlide();
            }
            // Pendant la glissade, on ignore les inputs normaux
            inputDir = Vector3.zero;
        }
        else
        {
            // Vitesse selon l'état (crouch ou normal)
            float currentSpeed = isCrouching ? crouchspeed : (isSprinting ? sprintspeed : speed);
            Vector3 rawDir = new Vector3(h, 0f, v);
            inputDir = rawDir.normalized * currentSpeed;
        }

        // Détection Ctrl pour crouch ou slide
        if (Input.GetKeyDown(KeyCode.LeftControl) && canJump)
        {
            if (isSprinting && (h != 0f || v != 0f))
            {
                // Sprint + mouvement + Ctrl = Glissade
                StartSlide(new Vector3(h, 0f, v).normalized);
            }
            else if (!isSliding)
            {
                // Marche + Ctrl = Accroupissement
                StartCrouch();
            }
        }

        // Désaccroupissement avec Ctrl (toggle)
        if (Input.GetKeyUp(KeyCode.LeftControl) && isCrouching)
        {
            StopCrouch();
        }

        // Saut
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (rb == null) return;

            // Annuler crouch/slide si on saute
            if (isCrouching) StopCrouch();
            if (isSliding) StopSlide();

            if (isTouchingWall && Time.time > lastWallJumpTime + wallJumpCooldown)
            {
                Vector3 away = new Vector3(wallNormal.x, 0f, wallNormal.z).normalized;
                Vector3 jumpVel = away * wallJumpHorizontalForce + Vector3.up * wallJumpUpForce;
                rb.linearVelocity = new Vector3(jumpVel.x, jumpVel.y, jumpVel.z);
                lastWallJumpTime = Time.time;
                canJump = false;
                isTouchingWall = false;
                hasDoubleJumped = false;
            }
            else if (canJump)
            {
                Vector3 vel = rb.linearVelocity;
                rb.linearVelocity = new Vector3(vel.x, jumpForce, vel.z);
                canJump = false;
                hasDoubleJumped = false;
            }
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

        bool grounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance + 0.1f);
        if (grounded && rb.linearVelocity.y <= 0.05f)
        {
            canJump = true;
            hasDoubleJumped = false;
        }

        if (isSliding)
        {
            ApplySlide();
        }
        else
        {
            MovePlayer();
        }
    }

    private void StartCrouch()
    {
        isCrouching = true;
        transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
    }

    private void StopCrouch()
    {
        isCrouching = false;
        transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
    }

    private void StartSlide(Vector3 direction)
    {
        isSliding = true;
        slideTimer = slideDuration;
        
        // Direction de la glissade dans l'espace monde
        Transform refer = (orientation != null) ? orientation : this.transform;
        slideDirection = (refer.forward * direction.z + refer.right * direction.x).normalized;
        
        // Réduire la taille du personnage
        transform.localScale = new Vector3(transform.localScale.x, slideYScale, transform.localScale.z);
        
        // Appliquer la force initiale de glissade
        rb.AddForce(slideDirection * slideForce, ForceMode.VelocityChange);
    }

    private void StopSlide()
    {
        isSliding = false;
        slideTimer = 0f;
        transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
    }

    private void ApplySlide()
    {
        // Appliquer une friction progressive pour ralentir la glissade
        Vector3 horizontalVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        Vector3 friction = -horizontalVel * slideFriction;
        rb.AddForce(friction, ForceMode.Force);
    }

    private void MovePlayer()
    {
        if (rb == null) return;

        Transform refer = (orientation != null) ? orientation : this.transform;
        Vector3 worldDir = refer.forward * inputDir.z + refer.right * inputDir.x;

        Vector3 targetVel = worldDir;
        Vector3 newVel = new Vector3(targetVel.x, rb.linearVelocity.y, targetVel.z);
        rb.linearVelocity = newVel;

        if (worldDir.sqrMagnitude > 0.001f)
        {
            Vector3 stepDirNorm = worldDir.normalized;
            float stepDist = worldDir.magnitude * Time.fixedDeltaTime;
            if (Physics.Raycast(transform.position, stepDirNorm, out RaycastHit hit, stepDist + wallCheckPadding))
            {
                if (hit.collider.CompareTag("mur"))
                {
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
        foreach (ContactPoint cp in collision.contacts)
        {
            if (cp.normal.y > 0.5f)
            {
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
