using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : Singleton<HUD>
{
    [SerializeField] UnitBase player;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] TextMeshProUGUI killText;
    public Slider healthSlider;
    public GameObject deathPopup;
    PlayerHealth playerHealth;
    Ammo playerAmmo;

    private void Awake()
    {
        deathPopup.SetActive( false );
    }
    // Start is called before the first frame update
    public void SetPlayer( UnitBase player )
    {
        this.player = player;
        //playerHealth = player.GetComponent<PlayerHealth>();
        //playerAmmo = player.GetComponent<Ammo>();
    }

    // Update is called once per frame
    void Update()
    {
        if( this.player )
        {
            healthSlider.value = player.HealthRate;
        }
        //healthText.text = playerHealth.GetHealth().ToString();
        //ammoText.GetComponent<TMPro.TextMeshProUGUI>().text = "Ammo: " + playerAmmo.GetCurrentAmmoCount(AmmoType.Bullets).ToString();
        //ammoText.text = playerAmmo.GetCurrentAmmoCount(AmmoType.Bullets).ToString();
    }

    public void ShowDeathPopup()
    {
        deathPopup.SetActive( true );
        Cursor.visible = true;
    }

    public void Respawn()
    {
        deathPopup.SetActive( false );
        Cursor.visible = false;
        PlayerController.Instance.AddCommand( PlayerCommand.Respawn );
    }
}
