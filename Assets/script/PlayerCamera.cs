using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float mouseSensibiliteX = 200f;
    public float mouseSensibiliteY = 200f;
    public Transform playerBody;

    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Charger les sensibilités sauvegardées
        mouseSensibiliteX = PlayerPrefs.GetFloat("MouseSensitivityX", mouseSensibiliteX);
        mouseSensibiliteY = PlayerPrefs.GetFloat("MouseSensitivityY", mouseSensibiliteY);
    }

    void Update()
    {
        // On récupère la souris avec des sensibilités séparées
        float mouseX = Input.GetAxis("Mouse X") * mouseSensibiliteX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensibiliteY * Time.deltaTime;

        // --- Rotation verticale de la caméra ---
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // --- Rotation horizontale du joueur ---
        playerBody.Rotate(Vector3.up * mouseX);
    }

    // Méthodes pour mettre à jour les sensibilités en temps réel
    public void UpdateSensibiliteX(float newSensibiliteX)
    {
        mouseSensibiliteX = newSensibiliteX;
        PlayerPrefs.SetFloat("MouseSensitivityX", newSensibiliteX);
    }

    public void UpdateSensibiliteY(float newSensibiliteY)
    {
        mouseSensibiliteY = newSensibiliteY;
        PlayerPrefs.SetFloat("MouseSensitivityY", newSensibiliteY);
    }
}