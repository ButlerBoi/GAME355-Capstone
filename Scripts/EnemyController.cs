using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public int maxHealth = 100;
    public int damage = 10;
    public float healthDropChance = 0.25f; //%
    public float speedDropChance = 0.25f; //%
    public GameObject healthPickupPrefab; 
    public GameObject speedPickupPrefab; 
    public GameObject damageNum;

    public Transform enemy;
    public Rigidbody2D rb;
    private Vector2 movement;
    private int currentHealth;

    [SerializeField]
    private Transform player;
    [SerializeField]
    private float predictionTime = 1.0f;
    [SerializeField]
    private float reactionDelay = 0.5f;
    [SerializeField]
    EnemyHealthBar healthBar;
    [SerializeField]
    private Transform referencePointForHealthBar;
    [SerializeField]
    private float knockbackDistance = 1.0f;

    private Vector2 knockbackDirection;


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar = GetComponentInChildren<EnemyHealthBar>();
        AssignPlayerTarget();
        StartCoroutine(PursueWithDelay());
        healthBar.UpdateHealthBar(currentHealth, maxHealth);
    }
    void AssignPlayerTarget()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player not found!");
        }
    }

    void Update()
    {
   
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            LootDrop();
        }

    }

    IEnumerator PursueWithDelay()
    {
        while (true)
        {
            // "reaction time" delay
            yield return new WaitForSeconds(reactionDelay);

            if (player != null)
            {
                Vector2 playerVelocity = player.GetComponent<Rigidbody2D>().velocity;
                Vector2 predictedPosition = (Vector2)player.position + (playerVelocity * predictionTime);

                Vector3 scale = transform.localScale;
                if (player.transform.position.x > transform.position.x)
                {
                    scale.x = Mathf.Abs(scale.x) * -1;
                }
                else
                {
                    scale.x = Mathf.Abs(scale.x);
                }

                Vector2 direction = (predictedPosition - (Vector2)transform.position).normalized;
                movement = direction;

                transform.localScale = scale;
            }
            yield return null;
        }
    }
 
    private void FixedUpdate()
    {
        moveCharacter(movement);
    }
    void moveCharacter(Vector2 direction)
    {
        rb.MovePosition((Vector2)transform.position + (direction * moveSpeed * Time.deltaTime));
    }
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        healthBar.UpdateHealthBar(currentHealth, maxHealth);

        DamageText(amount);

        StartCoroutine(ApplyKnockback());
    }

    private void DamageText(int amount)
    {
        GameObject points = Instantiate(damageNum, transform.position, Quaternion.identity) as GameObject;
        points.transform.GetChild(0).GetComponent<TextMesh>().text = amount.ToString();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().TakeDamage(damage);
        }

    }

    IEnumerator ApplyKnockback()
    {
        knockbackDirection = -(player.position - transform.position).normalized;

        Vector2 originalPosition = transform.position;
        Vector2 targetPosition = originalPosition + knockbackDirection * knockbackDistance;

        float elapsedTime = 0f;
        float duration = 0.2f;

        while (elapsedTime < duration)
        {
            transform.position = Vector2.Lerp(originalPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public void LootDrop(){

        float randomValue = Random.value;

        if (randomValue <= healthDropChance)
        {
            InstantiateDrop(healthPickupPrefab);
        } else if (randomValue <= healthDropChance + speedDropChance)
        {
            InstantiateDrop(speedPickupPrefab);
        }
    }
    private void InstantiateDrop(GameObject prefab)
    {
        if (prefab != null)
        {
            Instantiate(prefab, transform.position, Quaternion.identity);
        }
    }

}
