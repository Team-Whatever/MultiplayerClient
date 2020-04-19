using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;
using Unity.Networking.Transport;
using NetworkMessages;
using System;
using System.Text;
using System.Collections.Generic;

public class NetworkServer : MonoBehaviour
{
    public NetworkDriver m_Driver;
    public ushort serverPort;
    private NativeList<NetworkConnection> m_Connections;
    Dictionary<NetworkConnection, string> m_ClientIds;
    float lastSentTime;

    void Start()
    {
        m_Driver = NetworkDriver.Create();
        var endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = serverPort;
        if( m_Driver.Bind( endpoint ) != 0 )
            Debug.Log( "[Server] Failed to bind to port " + serverPort );
        else
            m_Driver.Listen();

        m_Connections = new NativeList<NetworkConnection>( 16, Allocator.Persistent );
        m_ClientIds = new Dictionary<NetworkConnection, string>();

        UnitBase.IsRunOnServer = true;
    }
    void SendToClient( string message, NetworkConnection c )
    {
        if( c.IsCreated )
        {
            var writer = m_Driver.BeginSend( NetworkPipeline.Null, c );
            NativeArray<byte> bytes = new NativeArray<byte>( Encoding.ASCII.GetBytes( message ), Allocator.Temp );
            writer.WriteBytes( bytes );
            m_Driver.EndSend( writer );
        }
    }

    public void OnDestroy()
    {
        m_Driver.Dispose();
        m_Connections.Dispose();
    }

    void OnConnect( NetworkConnection c )
    {
        m_Connections.Add( c );
        Debug.Log( "[Server] Accepted a connection : " + c.InternalId );
    }

    void OnData( int i, DataStreamReader stream )
    {
        NativeArray<byte> bytes = new NativeArray<byte>( stream.Length, Allocator.Temp );
        stream.ReadBytes( bytes );
        string recMsg = Encoding.ASCII.GetString( bytes.ToArray() );
        NetworkHeader header = JsonUtility.FromJson<NetworkHeader>( recMsg );

        switch( header.cmd )
        {
            case Commands.LOGIN:
                LoginMsg lMsg = JsonUtility.FromJson<LoginMsg>( recMsg );
                Debug.Log( "[Server] Client login message received! : " + lMsg.clientId);
                OnClientLogIn( i, lMsg.clientId );
                break;
            case Commands.PLAYER_UPDATE:
                PlayerUpdateMsg puMsg = JsonUtility.FromJson<PlayerUpdateMsg>( recMsg );
                //Debug.Log( "[Server] Player update message received! : " + puMsg.player.ToString() );
                GameServerManager.Instance.UpdatePlayerCommands( puMsg.player, puMsg.commands );
                break;
            case Commands.SERVER_UPDATE:
                ServerUpdateMsg suMsg = JsonUtility.FromJson<ServerUpdateMsg>( recMsg );
                Debug.Log( "[Server] Server update message received!" );
                break;
            default:
                Debug.Log( "[Server] Unrecognized message received!" );
                break;
        }
    }

    void OnClientLogIn( int i, string clientId )
    {
        m_ClientIds.Add( m_Connections[i], clientId );
        PlayerData playerData = GameServerManager.Instance.SpawnPlayer( clientId );

        if( playerData != null )
        {
            PlayerSpawnMsg msg = new PlayerSpawnMsg( clientId, playerData );
            SendToClient( JsonUtility.ToJson( msg ), m_Connections[i] );
        }
    }

    void OnDisconnect( int i )
    {
        Debug.Log( "[Server] Client disconnected from server : " + i );
        if( i < m_Connections.Length )
        {
            RemovePlayer( m_Connections[i] );
            m_Connections[i] = default( NetworkConnection );
        }
    }



    void FixedUpdate()
    {
        m_Driver.ScheduleUpdate().Complete();

        // CleanUpConnections
        for( int i = 0; i < m_Connections.Length; i++ )
        {
            if( !m_Connections[i].IsCreated )
            {
                RemovePlayer( m_Connections[i] );
                m_Connections.RemoveAtSwapBack( i );
                --i;
            }
        }

        // AcceptNewConnections
        NetworkConnection c = m_Driver.Accept();
        while( c != default( NetworkConnection ) )
        {
            OnConnect( c );

            // Check if there is another new connection
            c = m_Driver.Accept();
        }

        DataStreamReader stream;
        for( int i = 0; i < m_Connections.Length; i++ )
        {
            Assert.IsTrue( m_Connections[i].IsCreated );

            NetworkEvent.Type cmd;
            cmd = m_Driver.PopEventForConnection( m_Connections[i], out stream );
            while( cmd != NetworkEvent.Type.Empty )
            {
                if( cmd == NetworkEvent.Type.Data )
                {
                    OnData( i, stream );
                }
                else if( cmd == NetworkEvent.Type.Disconnect )
                {
                    OnDisconnect( i );
                }

                cmd = m_Driver.PopEventForConnection( m_Connections[i], out stream );
            }
        }

        if( GameServerManager.Instance.HasClientChanged || Time.time - lastSentTime > 0.05f )
        {
            for( int i = 0; i < m_Connections.Length; i++ )
            {
                ServerUpdateMsg m = new ServerUpdateMsg( GameServerManager.Instance.playersData, GameServerManager.Instance.playersCommands );
                if( m_Connections[i].IsCreated )
                    SendToClient( JsonUtility.ToJson( m ), m_Connections[i] );
                GameServerManager.Instance.playersCommands.Clear();
            }
            GameServerManager.Instance.HasClientChanged = false;
            lastSentTime = Time.time;
        }
    }

    void RemovePlayer( NetworkConnection c )
    {
        if( m_ClientIds.ContainsKey( c ) )
        {
            string clientId = m_ClientIds[c];
            m_ClientIds.Remove( c );
            GameServerManager.Instance.RemovePlayer( clientId );
        }
        else
        {
            Debug.LogWarning( "[Server] Client not found : " + c.InternalId );
        }
    }
}