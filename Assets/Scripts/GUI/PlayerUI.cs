﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    /// <summary>
    /// UI features
    /// </summary>
    public TextMeshProUGUI clientIdText;
    public Slider healthBar;

    public void SetUserData( string clientId, bool isLocalPlayer )
    {
        if( clientIdText != null )
        {
            clientIdText.text = clientId.Substring( 0, 6 );
            clientIdText.color = isLocalPlayer ? Color.red : Color.gray;
        }
    }

    public void SetHealthBarProgress( float percent )
    {
        if( healthBar )
        {
            healthBar.gameObject.SetActive( percent < 1.0f );
            healthBar.value = percent;
        }
    }
}
