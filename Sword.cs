using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public float speed = 10f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public AudioClip shootingSound;
    public float attackCooldown = 0.5f;
    public float criticalChance = 10f;

    private float lastAttackTime;
    private AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = AudioManager.instance;
    }

    public void Shoot()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            GameObject bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            
            float rand = Random.Range(0f, 100f);
            bool isCritical = rand <= criticalChance;

            bullet.GetComponent<Bullet>().isCritical = isCritical;

            bullet.GetComponent<Rigidbody2D>().AddForce(firePoint.right * bullet.GetComponent<Bullet>().speed, ForceMode2D.Impulse);

            lastAttackTime = Time.time;

            PlayShootingSound();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            Shoot();
        }
    }

    private void PlayShootingSound()
    {
        if (audioManager != null)
        {
            AudioManager.instance.SetSoundEffectVolume(0.25f);
            audioManager.PlaySoundEffect(2);
        }
    }
}
