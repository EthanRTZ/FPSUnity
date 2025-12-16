using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [Header("Paramètres de l'attaque")]
    public int damage = 34;
    public float range = 2.5f;
    public float attackCooldown = 0.5f;

    [Header("Détection")]
    public LayerMask whatIsEnemy;
    public Camera fpsCam;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip meleeSound;
    public AudioClip meleeHitSound;

    [Header("Effets visuels")]
    public bool useScreenShake = true;
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.1f;

    private bool readyToAttack = true;
    private float cooldownTimer = 0f;
    private Vector3 originalCameraPosition;
    private bool isShaking = false;
    private float shakeTimer = 0f;

    void Start()
    {
        if (fpsCam == null)
        {
            fpsCam = Camera.main;
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        if (fpsCam != null)
        {
            originalCameraPosition = fpsCam.transform.localPosition;
        }
    }

    void Update()
    {
        if (!readyToAttack)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= attackCooldown)
            {
                readyToAttack = true;
                cooldownTimer = 0f;
            }
        }

        if (isShaking)
        {
            shakeTimer += Time.deltaTime;
            if (shakeTimer >= shakeDuration)
            {
                isShaking = false;
                shakeTimer = 0f;
                if (fpsCam != null)
                {
                    fpsCam.transform.localPosition = originalCameraPosition;
                }
            }
            else
            {
                if (fpsCam != null)
                {
                    fpsCam.transform.localPosition = originalCameraPosition + Random.insideUnitSphere * shakeIntensity;
                }
            }
        }

        if (Input.GetMouseButtonDown(1) && readyToAttack)
        {
            PerformMeleeAttack();
        }
    }

    void PerformMeleeAttack()
    {
        readyToAttack = false;

        if (audioSource != null && meleeSound != null)
        {
            audioSource.PlayOneShot(meleeSound);
        }

        bool hitSomething = false;
        RaycastHit hit;

        if (fpsCam != null && Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range, whatIsEnemy))
        {
            int hitLayer = hit.collider.gameObject.layer;
            if ((whatIsEnemy.value & (1 << hitLayer)) != 0)
            {
                ReceiveDamage receiveDamage = hit.collider.GetComponentInParent<ReceiveDamage>();
                if (receiveDamage != null)
                {
                    receiveDamage.GetDamage(damage);
                    hitSomething = true;

                    if (audioSource != null && meleeHitSound != null)
                    {
                        audioSource.PlayOneShot(meleeHitSound);
                    }
                }
            }
        }

        if (hitSomething && useScreenShake)
        {
            StartScreenShake();
        }
    }

    void StartScreenShake()
    {
        if (fpsCam != null)
        {
            isShaking = true;
            shakeTimer = 0f;
        }
    }
}
