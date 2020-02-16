using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : Singleton<GameplayManager>
{
    // the player(owner) of this client
    public string localPlayerId;
    public PlayerUnit playerPrefab;
    Dictionary<string, PlayerUnit> playerUnits = new Dictionary<string, PlayerUnit>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnPlayer( string playerId, Vector3 pos, bool isLocalPlayer )
    {
        if( playerUnits.ContainsKey( playerId ) )
        {
            Debug.Assert( false, playerId + " player already  exist." );
        }
        else
        {
            PlayerUnit player = Instantiate( playerPrefab );
            player.transform.position = pos;
            player.SetId( playerId, isLocalPlayer );
            playerUnits.Add( playerId, player );
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

    }
}
