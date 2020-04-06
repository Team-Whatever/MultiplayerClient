using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : Singleton<GameplayManager>
{
    // the player(owner) of this client
    public UnitBase playerPrefab;
    Dictionary<int, UnitBase> playerUnits = new Dictionary<int, UnitBase>();
    //Dictionary<int, List<PlayerCommandData>> playerCommands = new Dictionary<int, List<PlayerCommandData>>();
    public List<PlayerData> playersData = new List<PlayerData>();

    Dictionary<int, PlayerData> prevPlayerData = new Dictionary<int, PlayerData>();

    public float lastUpdatedTime;
    public float prevUpdatedTime;

    public bool IsServer { get; set; }
    public bool HasClientChanged { get; set; }

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
            playersData.Add( playerInfo );
            player.SetPlayerData( playerInfo, isLocalPlayer );
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
                if( playerData.id == PlayerController.Instance.localPlayerId )
                {
                    playerUnits[playerData.id].transform.position = playerData.position;
                    playerUnits[playerData.id].transform.rotation = playerData.rotation;
                }
                else
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

    public void UpdatePlayerCommands( PlayerData playerData, List<PlayerCommandData> commands )
    {
        // only works in server
        if( !IsServer )
            return;

        lastUpdatedTime = Time.time;

        if( playerUnits.ContainsKey( playerData.id ) )
        {
            foreach( var command in commands )
                UpdatePlayerCommand( playerUnits[playerData.id], command );
            playerUnits[playerData.id].UpdatePlayerData();
        }
        else
        {
            Debug.LogWarning( "Player not found, respawn it " + playerData.id );
            SpawnPlayer( playerData, false );
        }

        HasClientChanged = true;
        prevUpdatedTime = lastUpdatedTime;
    }

    void UpdatePlayerCommand( UnitBase unit, PlayerCommandData cmd )
    {
        // only works in server
        if( !IsServer )
            return;

        switch( cmd.command )
        {
            case PlayerCommand.MoveForward:
                unit.MoveBy( unit.transform.forward );
                break;
            case PlayerCommand.MoveBackward:
                unit.MoveBy( -unit.transform.forward );
                break;
            case PlayerCommand.MoveLeft:
                unit.MoveBy( -unit.transform.right );
                break;
            case PlayerCommand.MoveRight:
                unit.MoveBy( unit.transform.right );
                break;
            case PlayerCommand.RotateLeft:
                unit.Rotate( -Time.fixedDeltaTime );
                break;
            case PlayerCommand.RotateRight:
                unit.Rotate( Time.fixedDeltaTime );
                break;
            case PlayerCommand.TurnHorizontal:
                unit.Rotate( cmd.value );
                break;
            case PlayerCommand.LookUp:
                break;
            case PlayerCommand.LookDown:
                break;
            case PlayerCommand.FireBullet:
                unit.FireBullet();
                break;
            default:
                Debug.Assert( false, "TODO: Missing command : " + cmd.command.ToString() );
                break;
        }
    }

    public void DisconnectPlayer( int playerId )
    {
        if( playerUnits.ContainsKey( playerId ) )
        {
            playersData.Remove( playerUnits[playerId].GetPlayerData() );
            playerUnits.Remove( playerId );
            Destroy( playerUnits[playerId].gameObject );
        }
    }

}
