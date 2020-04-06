using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NetworkMessages
{


    public enum Commands
    {
        CONNECTED,
        PLAYER_UPDATE,
        SERVER_UPDATE,
    }
    [System.Serializable]
    public class NetworkHeader
    {
        public Commands cmd;
    }
    [System.Serializable]
    public class ConnectMsg : NetworkHeader
    {
        public PlayerData player;
        public ConnectMsg( PlayerData data )
        {      // Constructor
            cmd = Commands.CONNECTED;
            player = data;
        }
    }
    [System.Serializable]
    public class PlayerUpdateMsg : NetworkHeader
    {
        public PlayerData player;
        public List<PlayerCommandData> commands;
        public PlayerUpdateMsg( PlayerData data, List<PlayerCommandData> cmds )
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
        //public List<PlayerCommandData> commands;
        public ServerUpdateMsg( List<PlayerData> data/*, List<PlayerCommandData> cmds*/ )
        {      // Constructor
            cmd = Commands.SERVER_UPDATE;
            players = data;
            //commands = cmds;
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
