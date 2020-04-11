using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NetworkMessages
{


    public enum Commands
    {
        CONNECTED,
        LOGIN,
        PLAYER_SPAWNED,
        PLAYER_UPDATE,
        SERVER_UPDATE,
    }
    [System.Serializable]
    public class NetworkHeader
    {
        public string clientId;
        public Commands cmd;
        public NetworkHeader( string id )
        {
            clientId = id;
        }
    }
    
    [System.Serializable]
    public class ConnectMsg : NetworkHeader
    {
        public ConnectMsg()
            : base( string.Empty )
        {      // Constructor
            cmd = Commands.CONNECTED;
        }
    }

    [System.Serializable]
    public class LoginMsg : NetworkHeader
    {
        public LoginMsg( string id )
            :base( id )
        {      // Constructor
            cmd = Commands.LOGIN;
        }
    }

    [System.Serializable]
    public class PlayerSpawnMsg : NetworkHeader
    {
        public PlayerData player;
        public PlayerSpawnMsg( string id, PlayerData data )
            : base( id )
        {      // Constructor
            cmd = Commands.PLAYER_SPAWNED;
            player = data;
        }
    }

    [System.Serializable]
    public class PlayerUpdateMsg : NetworkHeader
    {
        public PlayerData player;
        public List<PlayerCommandData> commands;
        public PlayerUpdateMsg( string id, PlayerData data, List<PlayerCommandData> cmds )
            : base( id )
        {      // Constructor
            cmd = Commands.PLAYER_UPDATE;
            player = data;
            commands = cmds;
        }
    };
    [System.Serializable]
    public class ServerUpdateMsg : NetworkHeader
    {
        public List<PlayerData> players;
        public ServerUpdateMsg( List<PlayerData> data/*, List<PlayerCommandData> cmds*/ )
            : base( string.Empty )
        {      // Constructor
            cmd = Commands.SERVER_UPDATE;
            players = data;
        }
    }
}

namespace NetworkObjects
{
    [System.Serializable]
    public class NetworkObject
    {
        public string id;
    }

    [System.Serializable]
    public class NetworkPlayer : NetworkObject
    {
        public Color cubeColor;
        public Vector3 position;
        public Quaternion rotation;

        public NetworkPlayer()
        {
        }
    }
}
