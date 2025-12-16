// csharp
// Fichier: 'Assets/script/ZombieAudio.cs'
using UnityEngine;

public class ZombieAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip groanClip;
    [SerializeField] private AudioClip attackClip;

    [Header("Volumes")]
    [Range(0f, 1f)] [SerializeField] private float sourceVolume = 1f;   // Volume de l'AudioSource
    [Range(0f, 2f)] [SerializeField] private float groanVolume = 1f;    // Volume du gémissement
    [Range(0f, 2f)] [SerializeField] private float attackVolume = 1f;   // Volume de l'attaque

    [Header("Intervalles")]
    [SerializeField] private float minGroanInterval = 4f;
    [SerializeField] private float maxGroanInterval = 9f;

    private float nextGroanTime;

    private void Awake()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f; // 3D
            audioSource.volume = Mathf.Clamp01(sourceVolume);
        }
        ScheduleNextGroan();
    }

    private void Update()
    {
        if (groanClip != null && audioSource != null && Time.time >= nextGroanTime)
        {
            audioSource.PlayOneShot(groanClip, Mathf.Clamp(groanVolume, 0f, 2f));
            ScheduleNextGroan();
        }
    }

    private void ScheduleNextGroan()
    {
        nextGroanTime = Time.time + Random.Range(minGroanInterval, maxGroanInterval);
    }

    public void OnPlayerSpotted()
    {
        if (groanClip != null && audioSource != null)
            audioSource.PlayOneShot(groanClip, Mathf.Clamp(groanVolume, 0f, 2f));
    }

    public void AnimAttackEvent()
    {
        if (attackClip != null && audioSource != null)
            audioSource.PlayOneShot(attackClip, Mathf.Clamp(attackVolume, 0f, 2f));
    }
}