using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{

    public enum PickupType
    {
        Speed,
        Health,
        Coin
    }

    public PickupType pickupType;
    public int healthBonus = 20;
    public float speedBonus = 5f;
    public float duration = 2f;
    private bool coroutineActive = false;
    private SpriteRenderer spriteRenderer;
    private AudioManager audioManager;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioManager = AudioManager.instance;

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !coroutineActive)
        {
            Player playerController = other.GetComponent<Player>();

            if (playerController != null)
            {
                ApplyBonus(playerController);
            }
        }
    }

    private void ApplyBonus(Player playerController)
    {
        switch (pickupType)
        {
            case PickupType.Health:
                playerController.Heal(healthBonus);
                StartCoroutine(HealthDelay());
                break;
            case PickupType.Speed:
                StartCoroutine(ApplySpeedBonus(playerController));
                break;
            case PickupType.Coin:
                StartCoroutine(CoinCount());
                break;
        }
    }

    private IEnumerator HealthDelay()
    {
        audioManager.PlaySoundEffect(4);
        Destroy(gameObject);
        yield return 0;
    }
    private IEnumerator CoinCount()
    {
        audioManager.PlaySoundEffect(3);
        Destroy(gameObject);
        yield return 0;
    }
    private IEnumerator ApplySpeedBonus(Player playerController)
    {
        // Set the flag to indicate that the coroutine is active
        coroutineActive = true;

        Debug.Log("Applying Speed Bonus");
        playerController.IncreaseSpeed(speedBonus);
        audioManager.PlaySoundEffect(4);
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        yield return new WaitForSeconds(duration);

        Debug.Log("Resetting Speed");
        playerController.ResetSpeed();

        coroutineActive = false;

        Destroy(gameObject);
    }
}
