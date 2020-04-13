using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] float healthAmount = 5;
    [SerializeField] float respawnTimer = 10;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            UnitBase targetUnit = collider.GetComponent<UnitBase>();
            if( targetUnit == null )
                return;

            if( !targetUnit.IsMaxHealth )
            {
                if( UnitBase.IsRunOnServer )
                {
                    targetUnit.Heal( healthAmount );
                }
                StartCoroutine( RespawnTimer() );
            }
        }
    }

    private IEnumerator RespawnTimer()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        gameObject.GetComponent<SphereCollider>().enabled = false;
        float timer = respawnTimer;
        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
        gameObject.GetComponent<SphereCollider>().enabled = true;
    }
}
