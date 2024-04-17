using System.Collections;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public GameObject coinPrefab;
    public int minCoins = 1;
    public int maxCoins = 4;
    public float spawnRadius = 1.0f;
    public Sprite openedSprite;

    private SpriteRenderer spriteRenderer;

    private AudioManager audioManager;

    private bool isOpened = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isOpened)
        {
            audioManager.PlaySoundEffect(7);
            isOpened = true;
            SpawnCoins();
            OpenChest();
        }
    }
    private void Start()
    {
        audioManager = AudioManager.instance;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void SpawnCoins()
    {
        int numCoins = Random.Range(minCoins, maxCoins + 1);
        Vector2 chestPosition = transform.position;

        for (int i = 0; i < numCoins; i++)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            Vector2 spawnPosition = chestPosition + randomDirection * spawnRadius;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPosition, 0.1f);
            if (colliders.Length == 0)
            {
                Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }

    private void OpenChest()
    {
        if (openedSprite != null)
        {
            spriteRenderer.sprite = openedSprite;
        }
    }
}

