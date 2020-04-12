using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerUnitManager : Singleton<PlayerUnitManager>
{
    List<PlayerData> players = new List<PlayerData>();
    Dictionary<string, PlayerData> playersDict = new Dictionary<string, PlayerData>();

    public UnitBase playerPrefab;
    List<int> unitModelIDs;
    List<int> usedModelIDs = new List<int>();

    // TODO : get the number from actual unit model;
    public static int MaxUnitTypes { get { return 10; } }
    public static float MaxHealth { get { return 100.0f; } }
    public static int MaxPlayers { get { return 10; } }

    public int NumPlayers
    {
        get { return players.Count; }
    }

    void Awake()
    {
        unitModelIDs = Enumerable.Range( 0, MaxUnitTypes ).OrderBy( x => Random.value ).ToList();
    }

    public PlayerData NewPlayer( string id )
    {
        if( playersDict.ContainsKey( id ) )
        {
            Debug.LogWarning( "Player already exist : " + id.ToString() );
            return playersDict[id];
        }

        if( unitModelIDs.Count == 0 )
        {
            unitModelIDs.AddRange( usedModelIDs );
            usedModelIDs.Clear();
        }
        
        // get from the last due to the performace of the list
        int modelId = unitModelIDs[unitModelIDs.Count - 1];
        usedModelIDs.Add( modelId );
        unitModelIDs.RemoveAt( unitModelIDs.Count - 1 );

        var newPlayer = new PlayerData( id, modelId, MaxHealth );
        players.Add( newPlayer );
        playersDict.Add( id, newPlayer );
        
        return newPlayer;
    }

    public void RemovePlayer( string id )
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

    public void UpdatePlayer( string id, PlayerData data )
    {
        if( playersDict.ContainsKey( id ) )
        {
            int idx = players.IndexOf( playersDict[id] );
            //players[idx] = data;
            playersDict[id] = data;
            Debug.Assert( players[idx] == playersDict[id], "List and the Dictionary should be identical" );
        }
        else
        {
            Debug.LogWarning( "Player not found : " + id.ToString() );
        }
    }

    public PlayerData GetPlayer( string id )
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

    public List<PlayerData> GetPlayers()
    {
        return players;
    }

}
