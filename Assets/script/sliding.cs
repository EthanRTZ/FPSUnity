using UnityEngine;

public class sliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerObj;
    private Rigidbody rb;
    private move pm;
    
    [Header("Sliding")]
    public float maxSlideTime;
    private float slideTimer;
    public float slideForce;
    
    public float slideYScale;
    private float startYScale;
    
    private bool Sliding;
    
    [Header("Input")]
    public KeyCode slideKey = KeyCode.LeftControl;
    private float horizontalInput;
    private float verticalInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<move>();
        // Si playerObj n'a pas été assigné dans l'inspecteur, on utilise ce transform comme fallback
        if (playerObj == null)
            playerObj = this.transform;
        // si orientation n'est pas assignée, on la remplace aussi par ce transform pour éviter l'exception
        if (orientation == null)
            orientation = this.transform;
        startYScale = playerObj.localScale.y;
    }
    
    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        
        if (Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0))
            startSlide();
        
        if (Input.GetKeyUp(slideKey) && Sliding)
            stopSlide();
    }
    
    private void FixedUpdate()
    {
        if (Sliding)
            SlidingMovement();
    }
    
    private void startSlide()
    {
         Sliding = true ;
         playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
         rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
         
         slideTimer = maxSlideTime;
    }
    
    private void SlidingMovement()
    {
        // utiliser une reference sûre : préférer orientation s'il est assigné, sinon fallback sur transform
        Transform refer = (orientation != null) ? orientation : this.transform;
        Vector3 inputDirection = refer.forward * verticalInput + refer.right * horizontalInput;
        rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);
        slideTimer -= Time.fixedDeltaTime;
        if (slideTimer <= 0)
        {
            Sliding = false;
            stopSlide();
        }
    }
    
    private void stopSlide()
    {
        Sliding = false ;
        
        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }
    
}
