using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField] int ammoAmount = 5;
    [SerializeField] AmmoType ammoType;
    [SerializeField] int respawnTimer = 10;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            FindObjectOfType<Ammo>().IncreaseCurrentAmmo(ammoType, ammoAmount);
            StartCoroutine(RespawnTimer());
        }
    }

    private IEnumerator RespawnTimer()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        gameObject.GetComponent<SphereCollider>().enabled = false;
        float timer = respawnTimer;
        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        transform.GetChild(0).gameObject.SetActive(true);
        gameObject.GetComponent<SphereCollider>().enabled = true;
    }
}
