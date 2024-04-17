using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public int minCount = 1;
    public int maxCount = 3;
    public float radius = 3.0f;
    public GameObject enemyPrefab;

    private static List<Spawner> allSpawners = new List<Spawner>();
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    void Start()
    {
        InitializeAllSpawners();
        SpawnEnemies();
    }

    void InitializeAllSpawners()
    {
        if (!allSpawners.Contains(this))
        {
            allSpawners.Add(this);
        }
    }

    void SpawnEnemies()
    {
        int enemyCount = Random.Range(minCount, maxCount + 1);

        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, transform); // Instantiate as child of the spawner
            spawnedEnemies.Add(enemy);
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector3 spawnPosition = transform.position + (Vector3)randomDirection * radius;
        return spawnPosition;
    }

    public static bool AreAllEnemiesDestroyed()
    {
        foreach (Spawner spawner in allSpawners)
        {
            if (!spawner.CheckAllEnemiesDestroyed())
            {
                return false;
            }
        }
        return true;
    }

    bool CheckAllEnemiesDestroyed()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                return false;
            }
        }
        return true;
    }

    void ResetLevel()
    {
        SceneManager.LoadScene("Win");
    }

    void Update()
    {
        if (AreAllEnemiesDestroyed())
        {
            ResetLevel();
        }
    }

    public void ResetSpawner()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        spawnedEnemies.Clear();
        SpawnEnemies();
    }
}
