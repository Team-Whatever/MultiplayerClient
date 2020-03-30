using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NetworkMessages
{
    public enum Commands
    {
        PLAYER_UPDATE,
        SERVER_UPDATE,
        HANDSHAKE,
    }
    [System.Serializable]
    public class NetworkHeader
    {
        public Commands cmd;
    }
    [System.Serializable]
    public class HandshakeMsg : NetworkHeader
    {
        public PlayerData player;
        public HandshakeMsg( PlayerData data )
        {      // Constructor
            cmd = Commands.HANDSHAKE;
            player = data;
        }
    }
    [System.Serializable]
    public class PlayerUpdateMsg : NetworkHeader
    {
        public PlayerData player;
        public PlayerUpdateMsg( PlayerData data )
        {      // Constructor
            cmd = Commands.PLAYER_UPDATE;
            player = data;
        }
    };
    [System.Serializable]
    public class ServerUpdateMsg : NetworkHeader
    {
        public List<PlayerData> players;
        public ServerUpdateMsg( List<PlayerData> data )
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
