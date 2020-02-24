using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int teamId;
    public float damage;
    public float ciriticalChance;

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
                Destroy( gameObject );
            }
        }
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
