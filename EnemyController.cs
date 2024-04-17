using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float wanderSpeed = 1.0f;
    public float wanderRadius = 1.5f;
    public int maxHealth = 100;
    public int damage = 10;
    public float healthDropChance = 0.25f; //%
    public float speedDropChance = 0.25f; //%
    public float coinDropChance = 0.25f; //%
    public float aggroRange = 4f;
    public float predictionTime = 0.6f;
    public GameObject healthPickupPrefab; 
    public GameObject speedPickupPrefab; 
    public GameObject coinPickupPrefab; 
    public GameObject damageNum;
    public GridManager gridManager;

    public Transform enemy;
    public Rigidbody2D rb;

    private AudioManager audioManager;

    private Vector2 movement;
    private int currentHealth;
    private float minIdleTime = 0.5f;
    private float maxIdleTime = 2.0f;
    private Vector3 wanderDestination;
    private Vector3 spawnPoint;
    private bool isWandering = false;
    private Vector2 knockbackDirection;
    private AStarPathfinding aStarPathfinding;
    private NavMeshAgent agent;


    [SerializeField]
    private Transform player;
    [SerializeField]
    private float reactionDelay = 0.5f;
    [SerializeField]
    EnemyHealthBar healthBar;
    [SerializeField]
    private Transform referencePointForHealthBar;
    [SerializeField]
    private float knockbackDistance = 1.0f;


    private enum EnemyState
    {
        Idle,
        Wandering,
        Pursuing
    }

    private EnemyState currentState = EnemyState.Idle;

    void Start()
    {
        audioManager = AudioManager.instance;
        aStarPathfinding = FindAnyObjectByType<AStarPathfinding>();
        if (aStarPathfinding == null)
        {
            //Debug.LogError("AStarPathfinding component not found in the scene.");
        }

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found!");
        }

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
            audioManager.PlaySoundEffect(6);
            Destroy(gameObject);
            LootDrop();
        }

        DebugExtension.DrawCircle(transform.position, Color.red, aggroRange);

        if (currentState == EnemyState.Wandering || currentState == EnemyState.Pursuing)
        {
            MoveCharacter(movement);
        }
        else if (agent.isActiveAndEnabled)
        {
            agent.SetDestination(player.position);
        }
        FlipSprite(movement.x);
    }

    void FixedUpdate()
    {
        // Store the current rotation
        Quaternion currentRotation = agent.transform.rotation;

        // Restore the previous rotation to prevent the agent from rotating
        agent.transform.rotation = currentRotation;
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

        // Check for pursuit conditions
        if (Vector3.Distance(transform.position, player.transform.position) <= aggroRange)
        {
            currentState = EnemyState.Pursuing;
            yield break;
        }
        agent.isStopped = true;

        // Set the time when the idle state started
        float startTime = Time.time;

        // Wait for a random duration before wandering again
        float idleDuration = Random.Range(minIdleTime, maxIdleTime);
        while (Time.time - startTime < idleDuration)
        {
            // Check for pursuit conditions during idle time
            if (Vector3.Distance(transform.position, player.transform.position) <= aggroRange)
            {
                currentState = EnemyState.Pursuing;
                yield break;
            }

            // Wait for the next frame
            yield return null;
        }

        // After idle duration, switch back to wandering
        currentState = EnemyState.Wandering;
        isWandering = false; // Reset isWandering
    }
    IEnumerator WanderState()
    {
        agent.isStopped = true;
        float startTime = Time.time; // Record the start time
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
                DebugExtension.DrawCircle(spawnPoint, Color.white, 0.2f);
            }

            // Move towards the destination
            Vector3 direction = wanderDestination - transform.position;
            movement = direction.normalized;

            MoveCharacter(movement);

            // Check if arrived at destination or exceeded time
            if (Vector3.Distance(transform.position, wanderDestination) < 0.1f || Time.time - startTime > 0.55f)
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
            agent.isStopped = false;
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            // Check if the player is out of aggro range
            if (Vector3.Distance(transform.position, player.position) > aggroRange)
            {
                currentState = EnemyState.Idle;
                yield break;
            }
            // Predict player's position
            Vector3 playerVelocity = player.GetComponent<Rigidbody2D>().velocity;
            Vector3 predictedPosition = player.position + (playerVelocity * predictionTime);

            agent.SetDestination(predictedPosition);

            yield return new WaitForSeconds(reactionDelay);
        }
    }
    /*IEnumerator PursueState()
    {
        while (currentState == EnemyState.Pursuing)
        {
             // Move towards the player using A* pathfinding
            List<Vector3> path = aStarPathfinding.FindPath(transform.position, player.position);

            if (path != null && path.Count > 0)
            {
                Vector3 direction = (path[0] - transform.position).normalized;
                rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
            }

            // Add a delay before the next action
            yield return new WaitForSeconds(reactionDelay);
        }
    }*/

    void MoveTowardsPlayer()
    {
        // Ensure you have an instance of AStarPathfinding
        if (aStarPathfinding != null)
        {
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = player.position;

            // Find the path using A* pathfinding
            List<Vector3> path = aStarPathfinding.FindPath(startPosition, targetPosition);

            if (path != null && path.Count > 0)
            {
                // Move towards the first position in the path
                Vector3 direction = (path[0] - startPosition).normalized;
                rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
            }
        }
    }

    void FlipSprite(float directionX)
    {
        // Flip sprite according to movement direction
        if (directionX > 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (directionX < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
    void MoveCharacter(Vector2 direction)
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
        if (audioManager != null)
        {
            audioManager.PlaySoundEffect(1);
        }

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
        } else if (randomValue <= healthDropChance + speedDropChance + coinDropChance)
          {
              InstantiateDrop(coinPickupPrefab);
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
