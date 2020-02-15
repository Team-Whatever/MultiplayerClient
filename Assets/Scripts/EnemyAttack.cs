using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] float damage = 40.0f;
    PlayerHealth target;

    void Start()
    {
        target = FindObjectOfType<PlayerHealth>();
    }

    public void AttackHitEvent()
    {
        if (target == null) return;

        if (target.gameObject.tag == "Player")
        {
            target.TakeDamage(damage);
        }
    }

}
