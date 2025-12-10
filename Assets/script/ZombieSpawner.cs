using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Paramètres de Spawn")]
    public GameObject zombiePrefab;
    public int numberOfZombies = 5;
    public float spawnRadius = 30f;
    public float minDistanceBetweenZombies = 5f;

    [Header("Zone de Spawn")]
    public Vector3 spawnCenterOffset = Vector3.zero;
    public bool showSpawnZone = true;

    private int zombiesSpawned = 0;

    // Méthode publique pour spawner un nombre spécifique de zombies
    public void SpawnerZombies(int nombre)
    {
        zombiesSpawned = 0;

        for (int i = 0; i < nombre; i++)
        {
            SpawnZombie();
        }
    }

    public void SpawnAllZombies()
    {
        zombiesSpawned = 0;

        for (int i = 0; i < numberOfZombies; i++) SpawnZombie();

    }

    void SpawnZombie()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
        zombiesSpawned++;
    }

    Vector3 GetRandomSpawnPosition()
    {
        // Position du spawner + offset configuré
        Vector3 centerPosition = transform.position + spawnCenterOffset;

        // Générer une position aléatoire dans un rayon
        Vector3 randomDirection = Random.insideUnitSphere;
        randomDirection.y = 0; // Garder la même hauteur
        randomDirection = randomDirection.normalized;

        float randomDistance = Random.Range(0f, spawnRadius);
        Vector3 spawnPosition = centerPosition + (randomDirection * randomDistance);

        return spawnPosition;
    }

    // Fonction pour respawner les zombies 
    public void RespawnAllZombies()
    {
        // Détruire tous les zombies existants
        MonsterController[] existingZombies = FindObjectsOfType<MonsterController>();
        foreach (MonsterController zombie in existingZombies)
        {
            Destroy(zombie.gameObject);
        }

        // Respawner
        SpawnAllZombies();
    }
}
