using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI ammoText;
    PlayerHealth playerHealth;
    Ammo playerAmmo;
    // Start is called before the first frame update
    void Start()
    {
        playerHealth = player.GetComponent<PlayerHealth>();
        playerAmmo = player.GetComponent<Ammo>();
    }

    // Update is called once per frame
    void Update()
    {
        healthText.GetComponent<TMPro.TextMeshProUGUI>().text = "Health: " + playerHealth.GetHealth().ToString();
        //healthText.text = playerHealth.GetHealth().ToString();
        ammoText.GetComponent<TMPro.TextMeshProUGUI>().text = "Ammo: " + playerAmmo.GetCurrentAmmoCount(AmmoType.Bullets).ToString();
        //ammoText.text = playerAmmo.GetCurrentAmmoCount(AmmoType.Bullets).ToString();
    }
}
