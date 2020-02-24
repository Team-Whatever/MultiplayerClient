using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public float attackRange;
    public float attackDamage;
    public float damageMultiplier = 1.0f;
    private float damageSpread;

    public float criticalChance;
    public float criticalMultiplier = 1.0f;
    
    public float attackCooldown;

    /// <summary>
    /// Bullet Properties
    /// </summary>
    public Bullet bulletPrefab;
    public float bulletSpeed;
    public Transform bulletSpawnPoint;
    //public float angleOfSpreadingInFire;   // AI can have variation on firing angle.
    public GameObject attackParticle;


    // HACK : early instantiation may cause CanAttack() to false.
    protected float lastAttackedTime = -10.0f;
    public float cooldownProgress
    {
        get
        {
            return Mathf.Clamp01((Time.time - lastAttackedTime) / attackCooldown);
        }
    }
    public bool isBeganAttack;
    
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

    public virtual void OnBeginAttack()
    {
    }

    public virtual void OnFire()
    {
        if( CanAttack() )
        {
            FireBullet();
            lastAttackedTime = Time.time;
        }
    }

    public virtual void OnEndAttack()
    {
        owner.ChangeState( UnitState.Idle );
    }

    void FireBullet()
    {
        Bullet bullet = Instantiate( bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation );
        float damage = attackDamage * damageMultiplier + Random.Range( -damageSpread, damageSpread );
        bullet.Fire( owner.teamId, damage, bulletSpeed, criticalChance * criticalMultiplier );

        if( attackParticle != null )
        {
            GameObject particle = Instantiate( attackParticle, bulletSpawnPoint.position, bulletSpawnPoint.rotation );
            Destroy( particle, 2.0f );
        }

    }

}
