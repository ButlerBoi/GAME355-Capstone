using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField]
    private Slider healthSlider;
    [SerializeField]
    private Transform target;
    [SerializeField]
    private Vector3 offset;


    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        healthSlider.value = currentHealth / maxHealth;
    }
    void LateUpdate()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Enemy").transform;
            if (target == null)
            {
                Debug.LogError("Enemy reference not found!");
                return;
            }
        }

        transform.position = target.position + offset;

        float targetScaleX = target.localScale.x;
        Vector3 healthBarScale = transform.localScale;

        if (targetScaleX > 0f)
        {
            healthBarScale.x = Mathf.Abs(healthBarScale.x);
        }
        else
        {
            healthBarScale.x = -Mathf.Abs(healthBarScale.x);
        }
        transform.localScale = healthBarScale;
    }
}

