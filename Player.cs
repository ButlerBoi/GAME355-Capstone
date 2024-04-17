using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    public int maxHealth = 100;

    public float dashDistance = 3f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.0f;

    public Rigidbody2D rb;
    public Text healthText;
    public Text dashText;
    public Transform weapon;
    public float weaponDistance;

    private bool isDashing = false;
    private float lastDashTime = -999f;

    private AudioManager audioManager;

    private int currentHealth;
    private Vector2 moveDir;
    private Vector3 mousePosition;
    private float originalMoveSpeed;
    private bool weaponFacingRight = true;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        originalMoveSpeed = moveSpeed;
        EnemyController enemy = FindAnyObjectByType<EnemyController>();
        audioManager = AudioManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
        UpdateDisplay();
        HandleDashInput();

        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - transform.position;
        weapon.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        weapon.position = transform.position + Quaternion.Euler(0, 0, angle) * new Vector3(weaponDistance, 0, 0);
        WeaponFlipControl(mousePosition);
    }
    void FixedUpdate()
    {

        if (moveDir.x > 0)
        {
            transform.localScale = Vector3.one;
        }
        else if (moveDir.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        Move();
    }

    private void WeaponFlipControl(Vector3 mousePos)
    {
        if (mousePos.x < weapon.position.x && weaponFacingRight)
        {
            FlipWeapon();
        }
        else if (mousePos.x > weapon.position.x && !weaponFacingRight)
        {
            FlipWeapon();
        }
    }
    private void FlipWeapon()
    {
        weaponFacingRight = !weaponFacingRight;
        weapon.localScale = new Vector3(weapon.localScale.x, weapon.localScale.y * -1, weapon.localScale.z);
    }

    void ProcessInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDir = new Vector2(moveX, moveY);
    }

    void Move()
    {
        if (!isDashing)
        {
            rb.velocity = new Vector2(moveDir.x * moveSpeed, moveDir.y * moveSpeed);
        }
    }

    void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > lastDashTime + dashCooldown)
        {
            lastDashTime = Time.time;
            StartCoroutine(Dash());
        }
    }
    IEnumerator Dash()
    {
        isDashing = true;
        float dashStartTime = Time.time;
        Vector2 dashDirection = moveDir;

        while (Time.time < dashStartTime + dashDuration)
        {
            rb.velocity = dashDirection * (dashDistance / dashDuration);
            yield return null;
        }

        rb.velocity = Vector2.zero;
        isDashing = false;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (audioManager != null)
        {
            audioManager.PlaySoundEffect(0);
        }

        if (currentHealth <= 0)
        {
            Debug.Log("Player defeated!");
            SceneManager.LoadScene("GameOver");
        }
    }
    void UpdateDisplay()
    {
        healthText.text = "Health: " + currentHealth.ToString();

        float remainingCooldown = Mathf.Max(0, lastDashTime + dashCooldown - Time.time);

        if (remainingCooldown <= 0)
        {
            dashText.text = "Dash: Ready";
        }
        else
        {
            dashText.text = "Dash:      ";
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log("Player healed. Current health: " + currentHealth);
    }

    public void IncreaseSpeed(float bonus)
    {
        moveSpeed += bonus;
        Debug.Log("Player speed increased. Current speed: " + moveSpeed);
    }

    public void ResetSpeed()
    {
        moveSpeed = originalMoveSpeed;
        Debug.Log("Player speed reset. Current speed: " + moveSpeed);
    }
}
