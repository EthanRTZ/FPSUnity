using System.Collections.Generic;
using UnityEngine;

public class DamageSphere : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damagePerSecond = 25;
    public float sphereRadius = 5f;
    public LayerMask whatIsEnemy;

    [Header("Lifetime")]
    public float duration = 10f;

    private float lifetimeTimer = 0f;
    private Dictionary<GameObject, float> enemyDamageTimers = new Dictionary<GameObject, float>();
    private SphereCollider sphereCollider;

    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider == null)
        {
            sphereCollider = gameObject.AddComponent<SphereCollider>();
        }

        sphereCollider.isTrigger = true;
        sphereCollider.radius = sphereRadius;
    }

    void Update()
    {
        lifetimeTimer += Time.deltaTime;

        if (lifetimeTimer >= duration)
        {
            Destroy(gameObject);
            return;
        }

        List<GameObject> enemiesToRemove = new List<GameObject>();
        foreach (var kvp in enemyDamageTimers)
        {
            if (kvp.Key == null)
            {
                enemiesToRemove.Add(kvp.Key);
            }
        }

        foreach (var enemy in enemiesToRemove)
        {
            enemyDamageTimers.Remove(enemy);
        }
    }

    void OnTriggerStay(Collider other)
    {
        int hitLayer = other.gameObject.layer;
        if ((whatIsEnemy.value & (1 << hitLayer)) == 0)
            return;

        ReceiveDamage receiveDamage = other.GetComponentInParent<ReceiveDamage>();
        if (receiveDamage == null || receiveDamage.isDead)
            return;

        GameObject enemyRoot = receiveDamage.gameObject;

        if (!enemyDamageTimers.ContainsKey(enemyRoot))
        {
            enemyDamageTimers[enemyRoot] = 0f;
        }

        enemyDamageTimers[enemyRoot] += Time.deltaTime;

        if (enemyDamageTimers[enemyRoot] >= 1f)
        {
            receiveDamage.GetDamage(damagePerSecond);
            enemyDamageTimers[enemyRoot] = 0f;
        }
    }

    void OnTriggerExit(Collider other)
    {
        ReceiveDamage receiveDamage = other.GetComponentInParent<ReceiveDamage>();
        if (receiveDamage != null)
        {
            GameObject enemyRoot = receiveDamage.gameObject;
            if (enemyDamageTimers.ContainsKey(enemyRoot))
            {
                enemyDamageTimers.Remove(enemyRoot);
            }
        }
    }
}
