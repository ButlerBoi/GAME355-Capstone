using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float spawnRate = 1.0f;
    public float delay = 3f;
    public int enemyCount;
    public GameObject enemy;
    bool waveDone = true;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (waveDone == true)
        {
            StartCoroutine(waveSpawner());
        }
    }
 
    IEnumerator waveSpawner()
    {
        waveDone = false;

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemyClone = Instantiate(enemy, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(spawnRate);
        }

        yield return new WaitForSeconds(delay);

        waveDone = true;
    }
}
