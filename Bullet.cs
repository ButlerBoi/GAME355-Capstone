using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public int minDamage = 1;
    public int maxDamage = 4;
    public bool isCritical;
    public GameObject damageNumObject;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 0.2f);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            int damage = Random.Range(minDamage, maxDamage + 1);

            if(isCritical)
            {
                damage *= 2;
            }

            DamageNum damageNum = damageNumObject.GetComponent<DamageNum>();

            if (damageNum != null)
            {
                damageNum.Critical(isCritical);
            }

            other.GetComponent<EnemyController>().TakeDamage(damage);
        }

    }
}

