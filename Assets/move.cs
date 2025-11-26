using UnityEngine;

public class rotate : MonoBehaviour
{

    public int speed = 5;
    public int sprintspeed = 8;
    public float rotateSpeed = 180;
    public float jumpForce = 10;
    public bool canJump = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        /**if(Input.GetKey(KeyCode.A)){
            transform.Translate(-speed * Time.deltaTime,0,0);
        }**/
        if(Input.GetKey(KeyCode.D)){
            transform.Translate(+speed * Time.deltaTime,0,0);
        }
        if(Input.GetKey(KeyCode.W)){
            transform.Translate(0,0,+speed * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.S)){
            transform.Translate(0,0,-speed * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.Q)){
            transform.Rotate(0,-rotateSpeed*Time.deltaTime,0);
        }
        if(Input.GetKey(KeyCode.LeftShift)){
            transform.Translate(0,0,+sprintspeed * Time.deltaTime);
        }
        /**if(Input.GetKey(KeyCode.E)){
            transform.Rotate(0,rotateSpeed*Time.deltaTime,0);
        }**/

        transform.Rotate(0,rotateSpeed*Input.GetAxis("Mouse X"),0);



        if (Input.GetKey(KeyCode.Space) && canJump )
        {
            GetComponent<Rigidbody>().linearVelocity = Vector3.up*jumpForce;
            canJump = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "sol") 
        {
            canJump = true;
        }if (collision.gameObject.tag == "box")
        {
            canJump = true;
        }

        if (collision.gameObject.tag == "mur")
        {
            canJump = true;
        }

    }

    public class ColorCycler : MonoBehaviour
    {
        public float colorSpeed = 0.2f; // vitesse de rotation des couleurs
        private Renderer _rd;

        void Start()
        {
            _rd = GetComponent<Renderer>();
        }

        void Update()
        {
            float h = (Time.time * colorSpeed) % 1f; // teinte entre 0 et 1
            Color c = Color.HSVToRGB(h, 1f, 1f);
            _rd.material.color = c;
        }
    }
}
