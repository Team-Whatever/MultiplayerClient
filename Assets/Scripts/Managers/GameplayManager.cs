using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : Singleton<GameplayManager>
{
    Dictionary<string, UnitBase> playerUnits = new Dictionary<string, UnitBase>();
    public List<PlayerData> playersData = new List<PlayerData>();

    [HideInInspector]
    public float lastUpdatedTime;
    float prevUpdatedTime;

    // expected 200ms lag on the network
    public static float estimatedLag = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnPlayer( PlayerData playerInfo, bool isLocalPlayer )
    {
        if( playerUnits.ContainsKey( playerInfo.id ) )
        {
            Debug.LogWarning( playerInfo.id + " player already  exist." );
        }
        else
        {
            UnitBase player = Instantiate( PlayerUnitManager.Instance.playerPrefab );
            player.SetPlayerData( playerInfo, isLocalPlayer );
            Debug.LogWarning( playerInfo.id + " player spawned at : " + playerInfo.position.ToString() );

            playersData.Add( playerInfo );
            playerUnits.Add( playerInfo.id, player );

            if( isLocalPlayer )
            {
                PlayerController.Instance.localPlayer = player;
                PlayerController.Instance.localPlayerId = playerInfo.id;
            }
        }
    }

    public void UpdatePlayers( List<PlayerData> playersData, List<PlayerCommandData> playersCommands )
    {
        lastUpdatedTime = Time.time;
        // 
        foreach( var kv in playerUnits )
        {
            kv.Value.IsLatestDataReceived = false;
        }
        foreach( var playerData in playersData )
        {
            if( playerUnits.ContainsKey( playerData.id ) )
            {
                playerUnits[playerData.id].UpdatePlayerData( playerData, 
                    playerData.id == PlayerController.Instance.localPlayerId );
            }
            else
            {
                SpawnPlayer( playerData, false );
            }
            playerUnits[playerData.id].IsLatestDataReceived = true;
        }
        foreach( var commandData in playersCommands )
        {
            if( playerUnits.ContainsKey( commandData.playerId ) )
            {
                playerUnits[commandData.playerId].AddCommand( commandData.command );
                playerUnits[commandData.playerId].IsLatestDataReceived = true;
            }
        }
        foreach( var kv in playerUnits )
        {
            if( kv.Value.IsLatestDataReceived == false )
            {
                // disconnect one player at a time
                DisconnectPlayer( kv.Key );
                break;
            }
        }

        prevUpdatedTime = lastUpdatedTime;
    }


    public void DisconnectPlayer( string clientId )
    {
        if( PlayerController.Instance.localPlayerId != clientId && playerUnits.ContainsKey( clientId ) )
        {
            Destroy( playerUnits[clientId].gameObject );
            playersData.Remove( playerUnits[clientId].GetPlayerData() );
            playerUnits.Remove( clientId );
        }
    }

}
