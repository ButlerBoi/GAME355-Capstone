using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public AudioClip pickupSound;
    private AudioSource audioSource;

    public enum PickupType
    {
        Speed,
        Health
    }

    public PickupType pickupType;
    public int healthBonus = 20;
    public float speedBonus = 5f;
    public float duration = 2f;
    private bool coroutineActive = false;
    private SpriteRenderer spriteRenderer;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !coroutineActive)
        {
            Player playerController = other.GetComponent<Player>();

            if (playerController != null)
            {
                ApplyBonus(playerController);
                PlayPickupSound();
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
        }
    }

    private void PlayPickupSound()
    {
        // Check if an AudioSource is attached and a sound is assigned
        if (audioSource != null && pickupSound != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }
    }
    private IEnumerator HealthDelay()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
    private IEnumerator ApplySpeedBonus(Player playerController)
    {
        // Set the flag to indicate that the coroutine is active
        coroutineActive = true;

        Debug.Log("Applying Speed Bonus");
        playerController.IncreaseSpeed(speedBonus);

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        yield return new WaitForSeconds(duration);

        Debug.Log("Resetting Speed");
        playerController.ResetSpeed();

        // Reset the flag when the coroutine is done
        coroutineActive = false;

        // Destroy the pickup GameObject after the coroutine is complete
        Destroy(gameObject);
    }
}
