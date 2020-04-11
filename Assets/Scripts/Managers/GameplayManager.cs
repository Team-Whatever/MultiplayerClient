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

            playersData.Add( playerInfo );
            playerUnits.Add( playerInfo.id, player );

            if( isLocalPlayer )
            {
                PlayerController.Instance.localPlayer = player;
                PlayerController.Instance.localPlayerId = playerInfo.id;
            }
        }
    }

    public void UpdatePlayers( List<PlayerData> playersData )
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
                playerUnits[playerData.id].UpdateTransform( playerData.position, playerData.rotation, 
                    playerData.id == PlayerController.Instance.localPlayerId );
            }
            else
            {
                SpawnPlayer( playerData, false );
            }
            playerUnits[playerData.id].IsLatestDataReceived = true;
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
            playersData.Remove( playerUnits[clientId].GetPlayerData() );
            playerUnits.Remove( clientId );
            Destroy( playerUnits[clientId].gameObject );
        }
    }

}
