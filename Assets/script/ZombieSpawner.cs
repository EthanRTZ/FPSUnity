using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Paramètres de Spawn")]
    public GameObject zombie;
    public GameObject zombieRanged;
    public GameObject zombieExplosif;
    public GameObject zombieEnorme;




    public float chanceZombieNormal = 40f; 

    public float chanceZombieRanged = 20f; 

    public float chanceZombieExplosif = 20f; 

    public float chanceZombieEnorme = 20f;

    public int numberOfZombies = 5;
    public float spawnRadius = 10f;
    public float minDistanceBetweenZombies = 0f;

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

        GameObject zombieToSpawn = ChoisirTypeZombie();

        
        Instantiate(zombieToSpawn, spawnPosition, Quaternion.identity);
        zombiesSpawned++;
       
    }

    GameObject ChoisirTypeZombie()
    {
        float randomValue = Random.Range(0f, 100f);

        if (randomValue < chanceZombieNormal)
        {
            return zombie; 
        }
        else if (randomValue < chanceZombieNormal + chanceZombieRanged)
        {
            return zombieRanged; 
        }
        else if (randomValue < chanceZombieNormal + chanceZombieRanged + chanceZombieExplosif)
        {
            return zombieExplosif;
        }
        else 
        {
            return zombieEnorme;
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        Vector3 centerPosition = transform.position + spawnCenterOffset;

        Vector3 randomDirection = Random.insideUnitSphere;
        randomDirection.y = 0; 
        randomDirection = randomDirection.normalized;

        float randomDistance = Random.Range(0f, spawnRadius);
        Vector3 spawnPosition = centerPosition + (randomDirection * randomDistance);

        return spawnPosition;
    }

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
