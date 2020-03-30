using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerUnitManager
{
    List<PlayerData> players = new List<PlayerData>();
    Dictionary<int, PlayerData> playersDict = new Dictionary<int, PlayerData>();
    public List<int> unitModels;

    // indicate whther to update a player information to the other clients
    public bool IsDirtyFlag { get; private set; }

    // TODO : get the number from actual unit model;
    public static int MaxUnitTypes { get { return 10; } }
    public static float MaxHealth { get { return 100.0f; } }
    public static int MaxPlayers { get { return 10; } }

    public int NumPlayers
    {
        get { return players.Count; }
    }

    public PlayerUnitManager()
    {
        unitModels = Enumerable.Range( 0, MaxUnitTypes ).OrderBy( x => Random.value ).ToList();
    }

    public bool NewPlayer( int id )
    {
        if( playersDict.ContainsKey( id ) )
        {
            Debug.LogWarning( "Player already exist : " + id.ToString() );
            return false;
        }

        int modelId = 0;
        if( id < unitModels.Count )
            modelId = unitModels[id];

        var newPlayer = new PlayerData( id, modelId, MaxHealth );
        players.Add( newPlayer );
        playersDict.Add( id, newPlayer );
        
        return true;
    }

    public void RemovePlayer( int id )
    {
        if( playersDict.ContainsKey( id ) )
        {
            players.Remove( playersDict[id] );
            playersDict.Remove( id );
        }
        else
        {
            Debug.LogWarning( "Player not found : " + id.ToString() );
        }
    }

    public void UpdatePlayer( int id, PlayerData data )
    {
        if( playersDict.ContainsKey( id ) )
        {
            //int idx = players.IndexOf( playersDict[id] );
            //players[idx] = data;
            playersDict[id] = data;
            
            IsDirtyFlag = true;
        }
        else
        {
            Debug.LogWarning( "Player not found : " + id.ToString() );
        }
    }

    public PlayerData GetPlayer( int id )
    {
        if( playersDict.ContainsKey( id ) )
        {
            return playersDict[id];
        }
        else
        {
            Debug.LogWarning( "Player not found : " + id.ToString() );
            return null;
        }
    }

    public void ClearDirtyFlag()
    {
        IsDirtyFlag = false;
    }

    public List<PlayerData> GetPlayers()
    {
        return players;
    }

}
