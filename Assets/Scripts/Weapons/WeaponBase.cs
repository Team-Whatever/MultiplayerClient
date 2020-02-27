using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public WeaponType weaponType;

    public float attackRange;
    public float attackDamage;
    public float damageMultiplier = 1.0f;
    protected float damageSpread;

    public float criticalChance;
    public float criticalMultiplier = 1.0f;
    
    public float attackCooldown;


    // HACK : early instantiation may cause CanAttack() to false.
    protected float lastAttackedTime = -10.0f;
    public float cooldownProgress
    {
        get
        {
            return Mathf.Clamp01((Time.time - lastAttackedTime) / attackCooldown);
        }
    }
    
    [HideInInspector]
    public UnitBase owner;

    public bool CanAttack()
    {
        return Time.time - lastAttackedTime >= attackCooldown;
    }

    public bool IsInAttackRange( Transform target )
    {
        if( target != null && owner != null )
            return ( target.position - owner.transform.position ).sqrMagnitude <= attackRange * attackRange;
        return false;
    }

    public abstract void OnBeginAttack();
    public abstract void OnFire();
    public abstract void OnEndAttack();
}
