using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float maxHealth = 100;
    [SerializeField] float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            GetComponent<DeathHandler>().HandleDeath();
        }
    }

    public void ReceiveHealth(int health)
    {
        currentHealth += health;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    public float GetHealth() { return currentHealth; }
}
