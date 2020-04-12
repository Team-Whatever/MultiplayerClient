﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameServerManager : Singleton<GameServerManager>
{
    // have dictionary and the list together for the performance
    public List<PlayerData> playersData = new List<PlayerData>();
    public List<PlayerCommandData> playersCommands = new List<PlayerCommandData>();
    Dictionary<string, PlayerData> playersDataDict = new Dictionary<string, PlayerData>();

    Dictionary<string, PlayerData> prevPlayerData = new Dictionary<string, PlayerData>();
    Dictionary<string, UnitBase> playerUnits = new Dictionary<string, UnitBase>();

    [HideInInspector]
    public float lastUpdatedTime;
    float prevUpdatedTime;
    public bool HasClientChanged { get; set; }

    public PlayerData SpawnPlayer( string clientId )
    {
        if( playersDataDict.ContainsKey( clientId ) )
        {
            Debug.LogWarning( clientId + " player already  exist." );
            return playersDataDict[clientId];
        }
        else
        {
            PlayerData newPlayerData = PlayerUnitManager.Instance.NewPlayer( clientId );
            UnitBase player = Instantiate( PlayerUnitManager.Instance.playerPrefab );
            player.SetPlayerData( newPlayerData, false );

            playersData.Add( newPlayerData );
            playersDataDict.Add( clientId, newPlayerData );
            
            playerUnits.Add( clientId, player );
            return newPlayerData;
        }
    }

    public void UpdatePlayerCommands( PlayerData playerData, List<PlayerCommandData> commands )
    {
        lastUpdatedTime = Time.time;

        if( playerUnits.ContainsKey( playerData.id ) )
        {
            if( playerUnits[playerData.id].transform.position != playerData.position )
                Debug.Log( "position needs to be updated" );
            playerUnits[playerData.id].transform.position = playerData.position;
            playerUnits[playerData.id].transform.rotation = playerData.rotation;
            playersDataDict[playerData.id].position = playerData.position;
            playersDataDict[playerData.id].rotation = playerData.rotation;
            foreach( var command in commands )
                UpdatePlayerCommand( playerUnits[playerData.id], command );
        }
        else
        {
            Debug.LogWarning( "[Server] Player not found, respawn it " + playerData.id );
            SpawnPlayer( playerData.id );
        }

        HasClientChanged = true;
        prevUpdatedTime = lastUpdatedTime;
    }

    void UpdatePlayerCommand( UnitBase unit, PlayerCommandData cmd )
    {
        Debug.Log( "Player " + unit.PlayerId + " " + cmd.command.ToString() + " , value = " + cmd.value );
        switch( cmd.command )
        {
            case PlayerCommand.LookUp:
                break;
            case PlayerCommand.LookDown:
                break;
            case PlayerCommand.FireBullet:
                unit.FireBullet();
                playersCommands.Add( cmd );
                break;
            default:
                Debug.Assert( false, "TODO: Missing command : " + cmd.command.ToString() );
                break;
        }
    }

}