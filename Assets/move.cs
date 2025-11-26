using UnityEngine;

public class rotate : MonoBehaviour
{

    public int speed = 5;
    public int sprintspeed = 8;
    public float rotateSpeed = 180;
    public float jumpForce = 10;
    public bool canJump = false;
    public float groundCheckDistance = 0.25f;

    private Rigidbody rb;
    private Vector3 inputDir;
    private bool isSprinting;
    public float wallCheckPadding = 0.05f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
        // Initialiser canJump si déjà au sol au démarrage
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance + 0.1f))
            canJump = true;
    }

    // Update is called once per frame
    void Update()
    {
        /**if(Input.GetKey(KeyCode.A)){
            transform.Translate(-speed * Time.deltaTime,0,0);
        }**/
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

        transform.Rotate(0, rotateSpeed * Input.GetAxis("Mouse X"), 0);



        if (Input.GetKey(KeyCode.Space) && canJump)
        {
            // FIX: utiliser rb.velocity au lieu de rb.linearVelocity
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            canJump = false;
        }
    }

    void FixedUpdate()
    {
        // Ground check continu (évite blocage si OnCollisionEnter non appelé au start)
        bool grounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance + 0.1f);
        if (grounded && rb.linearVelocity.y <= 0.05f) canJump = true;

        if (rb == null) return;
        if (inputDir.sqrMagnitude > 0f)
        {
            Vector3 worldDir = transform.TransformDirection(inputDir);
            float stepDist = worldDir.magnitude * Time.fixedDeltaTime;
            Vector3 stepDirNorm = worldDir.normalized;

            // Raycast anti-tunneling mur
            if (Physics.Raycast(transform.position, stepDirNorm, out RaycastHit hit, stepDist + wallCheckPadding))
            {
                if (hit.collider.CompareTag("mur"))
                {
                    float allowed = Mathf.Max(0f, hit.distance - wallCheckPadding);
                    rb.MovePosition(rb.position + stepDirNorm * allowed);
                    return;
                }
            }

            rb.MovePosition(rb.position + worldDir * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("sol") ||
            collision.gameObject.CompareTag("box") ||
            collision.gameObject.CompareTag("mur"))
        {
            if (collision.GetContact(0).normal.y > 0.5f)
            {
                canJump = true;
            }
        }
    }
}
