using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public int enemyCount = 5;
    public float radius = 3.0f;
    public GameObject enemyPrefab; 

    // Start is called before the first frame update
    void Start()
    {
        SpawnEnemies();
    }

    // Spawn enemies at random positions around the spawner
    void SpawnEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }

    // Generate a random spawn position around the spawner
    Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector3 spawnPosition = transform.position + (Vector3)randomDirection * radius;

        return spawnPosition;
    }
}
