using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : WeaponBase
{
    [SerializeField] Camera FPCamera;
    
    [SerializeField] Ammo ammoSlot;
    [SerializeField] AmmoType ammoType;

    /// <summary>
    /// Bullet Properties
    /// </summary>
    public Bullet bulletPrefab;
    public float bulletSpeed;
    public Transform bulletSpawnPoint;
    //public float angleOfSpreadingInFire;   // AI can have variation on firing angle.
    public GameObject attackParticle;
    [SerializeField] ParticleSystem muzzleEffect;

    public override void OnBeginAttack()
    {
    }

    public override void OnFire()
    {
        if( CanAttack() )
        {
            FireBullet();
            lastAttackedTime = Time.time;
        }
    }

    public override void OnEndAttack()
    {
        owner.ChangeState( UnitState.Idle );
    }

    void FireBullet()
    {
        Bullet bullet = Instantiate( bulletPrefab, bulletSpawnPoint.position, owner.transform.rotation );
        float damage = attackDamage * damageMultiplier + Random.Range( -damageSpread, damageSpread );
        bullet.Fire( owner.teamId, damage, bulletSpeed, criticalChance * criticalMultiplier );

        if( attackParticle != null )
        {
            GameObject particle = Instantiate( attackParticle, bulletSpawnPoint.position, bulletSpawnPoint.rotation );
            Destroy( particle, 2.0f );
        }
        if( muzzleEffect != null )
        {
            muzzleEffect.Play();
        }
    }

    private void PlayMuzzleFlash()
    {
        muzzleEffect.Play();
    }

}
