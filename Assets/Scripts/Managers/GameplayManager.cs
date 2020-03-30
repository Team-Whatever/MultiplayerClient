using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : Singleton<GameplayManager>
{
    // the player(owner) of this client
    public int localPlayerId;
    public UnitBase playerPrefab;
    Dictionary<int, UnitBase> playerUnits = new Dictionary<int, UnitBase>();
    Dictionary<int, PlayerData> prevPlayerData = new Dictionary<int, PlayerData>();
    Dictionary<int, bool> playersUpdated = new Dictionary<int, bool>();
    public float lastUpdatedTime;
    public float prevUpdatedTime;

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
            Debug.Assert( false, playerInfo.id + " player already  exist." );
        }
        else
        {
            UnitBase player = Instantiate( playerPrefab );
            player.SetPlayerData( playerInfo, isLocalPlayer );
            playerUnits.Add( playerInfo.id, player );

            if( isLocalPlayer )
            {
                localPlayerId = playerInfo.id;
                PlayerController.Instance.localPlayer = player;
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
                if( playerData.id != localPlayerId )
                {
                    playerUnits[playerData.id].targetPosition = playerData.position;
                    playerUnits[playerData.id].targetRotation = playerData.rotation;
                }
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

    public void DisconnectPlayer( int playerId )
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

}
