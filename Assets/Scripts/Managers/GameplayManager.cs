using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : Singleton<GameplayManager>
{
    // the player(owner) of this client
    public string localPlayerId;
    public PlayerController playerPrefab;
    Dictionary<string, PlayerController> playerUnits = new Dictionary<string, PlayerController>();
    Dictionary<string, PlayerInfoData> prevPlayerData = new Dictionary<string, PlayerInfoData>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public PlayerController GetLocalPlayer()
    {
        if( playerUnits.ContainsKey( localPlayerId ) )
            return playerUnits[localPlayerId];
        return null;
    }

    public void SpawnPlayer( string playerId, Vector3 pos, bool isLocalPlayer )
    {
        if( playerUnits.ContainsKey( playerId ) )
        {
            Debug.Assert( false, playerId + " player already  exist." );
        }
        else
        {
            PlayerController player = Instantiate( playerPrefab );
            player.transform.position = pos;
            player.SetId( playerId, isLocalPlayer );
            playerUnits.Add( playerId, player );

            if( isLocalPlayer )
                localPlayerId = playerId;
        }
    }

    public void UpdatePlayer( PlayerInfoData playerData, PlayerInfoData prevPlayerData, float delta )
    {
        if( playerUnits.ContainsKey( playerData.id ) )
        {
            if( playerData.id != localPlayerId )
            {
                Vector3 nextPos = playerData.pos;
                Quaternion nextRotation = playerData.rotation;
                if( CanvasManager.Instance.reconciliation.isOn )
                {
                    // To be implemented
                }
                if( CanvasManager.Instance.interpolation.isOn && prevPlayerData != null )
                {
                    nextPos = Vector3.Lerp( prevPlayerData.pos, playerData.pos, delta );
                    nextRotation = Quaternion.Lerp( prevPlayerData.rotation, playerData.rotation, delta );
                }

                playerUnits[playerData.id].transform.position = nextPos;
                playerUnits[playerData.id].transform.rotation = nextRotation;
                playerUnits[playerData.id].SetHealth( playerData.health );
            }

            if( playerData.command != null && playerData.command != "" )
            {
                //Debug.Log( " " + player.id.ToString() + " ] " + player.action.ToString() );
                playerUnits[playerData.id].AddCommand( playerData.command );
            }
        }
    }

    public void DisconnectPlayer( string playerId )
    {
        if( playerUnits.ContainsKey( playerId ) )
        {
            if( playerId == localPlayerId )
            {
                Debug.LogWarning( "You has been disconnected." );
            }
            else
            {
                Destroy( playerUnits[playerId].gameObject );
                playerUnits.Remove( playerId );
            }
        }
    }

    public void RevivePlayer()
    {
        PlayerController localPlayer = GetLocalPlayer();
        if( localPlayer != null )
        {
            localPlayer.Reset();
        }
    }
}
