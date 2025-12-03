using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float mouseSensitivity = 200f;
    public Transform playerBody;

    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // On récupère la souris une seule fois
        Vector2 mouse = new Vector2(
            Input.GetAxis("Mouse X"),
            Input.GetAxis("Mouse Y")
        ) * mouseSensitivity * Time.deltaTime;

        // --- Rotation verticale de la caméra ---
        xRotation -= mouse.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // --- Rotation horizontale du joueur ---
        playerBody.Rotate(Vector3.up * mouse.x);
    }
}