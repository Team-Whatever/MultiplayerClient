using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int teamId;
    public float damage;
    public float ciriticalChance;
    [SerializeField] GameObject hitEffect;

    private void OnTriggerEnter( Collider other )
    {
        if( other.transform.tag == "Player" )
        {
            UnitBase targetUnit = other.GetComponent<UnitBase>();
            if( targetUnit == null )
                return;

            // HACK : until we get server-side physics
            // each client determines the damage it self
            if( !targetUnit.isLocalPlayer )
                return;

            if( targetUnit != null && teamId != targetUnit.teamId )
            {
                targetUnit.TakeDamage( damage );

                // TODO : change this to OnCollisionEnter to know what's the contact point
                // turn off trigger event and and rigid body

                Destroy( gameObject );
            }
        }
    }

    private void CreateHitImpact( RaycastHit hit )
    {
        GameObject hitEffectSpawn = Instantiate( hitEffect, hit.point, Quaternion.LookRotation( hit.normal ) );
        Destroy( hitEffectSpawn, 0.2f );
    }

    public void Fire( int teamId, float damage, float speed, float criticalChance )
    {
        this.teamId = teamId;
        this.damage = damage;
        this.ciriticalChance = criticalChance;
        GetComponent<Rigidbody>().AddForce( transform.forward * speed, ForceMode.Impulse );
        Destroy(gameObject, 5.0f);
    }
}
