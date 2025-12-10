using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    public float damage = 10f;
    public float lifetime = 5f;
    public string targetTag = "Player";

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform t = other.transform;

        // Remonte la hiérarchie pour vérifier le tag
        while (t != null)
        {
            if (t.CompareTag(targetTag))
            {
                ReceiveDamage receiveDamage = t.GetComponent<ReceiveDamage>();
                if (receiveDamage != null)
                {
                    receiveDamage.GetDamage(damage);
                }
                break;
            }
            t = t.parent;
        }

        Destroy(gameObject);
    }

}
