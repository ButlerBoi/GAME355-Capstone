using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float wanderSpeed = 1.0f;
    public float wanderRadius = 1.5f;
    public int maxHealth = 100;
    public int damage = 10;
    public float healthDropChance = 0.25f; //%
    public float speedDropChance = 0.25f; //%
    public float aggroRange = 4f;
    public GameObject healthPickupPrefab; 
    public GameObject speedPickupPrefab; 
    public GameObject damageNum;

    public Transform enemy;
    public Rigidbody2D rb;
    private Vector2 movement;
    private int currentHealth;
    private float minIdleTime = 1.5f;
    private float maxIdleTime = 3.0f;
    private float wanderTime;
    private Vector3 wanderDestination;
    private Vector3 spawnPoint;
    private bool isWandering = false;

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

    private enum EnemyState
    {
        Idle,
        Wandering,
        Pursuing
    }

    private EnemyState currentState = EnemyState.Idle;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar = GetComponentInChildren<EnemyHealthBar>();
        AssignPlayerTarget();
        StartCoroutine(UpdateState());
        healthBar.UpdateHealthBar(currentHealth, maxHealth);
        spawnPoint = transform.position;
    }
    void Update()
    {

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            LootDrop();
        }

        DebugExtension.DrawCircle(transform.position, Color.red, aggroRange);

        if (currentState == EnemyState.Wandering || currentState == EnemyState.Pursuing)
        {
            moveCharacter(movement);
        }
    }
    IEnumerator UpdateState()
    {
        while (true)
        {
            switch (currentState)
            {
                case EnemyState.Idle:
                    yield return StartCoroutine(IdleState());
                    break;
                case EnemyState.Wandering:
                    yield return StartCoroutine(WanderState());
                    break;
                case EnemyState.Pursuing:
                    yield return StartCoroutine(PursueState());
                    break;
            }
        }
    }
    IEnumerator IdleState()
    {
        Debug.Log("IDLE START");

        // Check for pursuit conditions
        if (Vector3.Distance(transform.position, player.transform.position) <= aggroRange)
        {
            currentState = EnemyState.Pursuing;
            yield break;
        }

        // Wait for a random duration before wandering again
        float idleDuration = Random.Range(minIdleTime, maxIdleTime);
        yield return new WaitForSeconds(idleDuration);
        Debug.Log("IDLE FINISH");

        // After idle duration, switch back to wandering
        currentState = EnemyState.Wandering;
        isWandering = false; // Ensure isWandering is reset
    }
    IEnumerator WanderState()
    {
        while (currentState == EnemyState.Wandering)
        {
            // Check for pursuit conditions
            if (Vector3.Distance(transform.position, player.transform.position) <= aggroRange)
            {
                currentState = EnemyState.Pursuing;
                yield break;
            }

            if (!isWandering)
            {
                // Set a random destination within the wander radius centered on the spawn point
                Vector2 randomDirection = Random.insideUnitCircle.normalized * wanderRadius;
                wanderDestination = spawnPoint + new Vector3(randomDirection.x, randomDirection.y, 0f);
                isWandering = true;
                wanderTime = Time.time;
                DebugExtension.DrawCircle(spawnPoint, Color.white, 0.2f);
            }
            Debug.Log("WANDER");

            // Move towards the destination
            Vector3 direction = wanderDestination - transform.position;
            movement = direction.normalized;

            moveCharacter(movement);

            // Check if arrived at destination
            if (Vector3.Distance(transform.position, wanderDestination) < 0.1f)
            {
                isWandering = false;
                currentState = EnemyState.Idle;
            }
            yield return null;
        }
    }

    IEnumerator PursueState()
    {
        while (currentState == EnemyState.Pursuing)
        {
            // Check if player is within aggro range
            if (Vector3.Distance(transform.position, player.transform.position) > aggroRange)
            {
                currentState = EnemyState.Idle; // Player out of range, switch to Idle state
                yield break;
            }

            //Pursue the player
            Vector2 playerVelocity = player.GetComponent<Rigidbody2D>().velocity;
            Vector2 predictedPosition = (Vector2)player.position + (playerVelocity * predictionTime);
            Vector2 direction = (predictedPosition - (Vector2)transform.position).normalized;
            movement = direction;
            moveSpeed = 10.0f;
            moveCharacter(movement);
            Debug.Log("PURSUE");

            Vector3 scale = transform.localScale;
            if (player.transform.position.x > transform.position.x)
            {
                scale.x = Mathf.Abs(scale.x) * -1;
            }
            else
            {
                scale.x = Mathf.Abs(scale.x);
            }
            transform.localScale = scale;

            yield return new WaitForSeconds(reactionDelay);
        }
    }

    void moveCharacter(Vector2 direction)
    {
        rb.MovePosition((Vector2)transform.position + (direction * moveSpeed * Time.deltaTime));
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().TakeDamage(damage);
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
