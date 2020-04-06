using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;
using NetworkMessages;
using System;
using System.Text;

public class NetworkClient : MonoBehaviour
{
    public NetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    public string serverIP = "3.219.69.41";
    public ushort serverPort = 12346;
    float lastTimestamp;

    void Start()
    {
        m_Driver = NetworkDriver.Create();
        m_Connection = default( NetworkConnection );
        var endpoint = NetworkEndPoint.Parse( serverIP, serverPort );
        m_Connection = m_Driver.Connect( endpoint );
        GameplayManager.Instance.IsServer = false;
    }

    void SendToServer( string message )
    {
        Debug.Log( "[Client] Send message to server : " + message );
        var writer = m_Driver.BeginSend( m_Connection );
        NativeArray<byte> bytes = new NativeArray<byte>( Encoding.ASCII.GetBytes( message ), Allocator.Temp );
        writer.WriteBytes( bytes );
        m_Driver.EndSend( writer );

        lastTimestamp = Time.time;
    }

    void OnConnect()
    {
        Debug.Log( "[Client] We are now connected to the server : " + m_Connection.InternalId );

        // Example to send a handshake message:
        //HandshakeMsg m = new HandshakeMsg();
        //m.player.id = m_Connection.InternalId.ToString();
        //SendToServer(JsonUtility.ToJson(m));
    }

    void OnData( DataStreamReader stream )
    {
        NativeArray<byte> bytes = new NativeArray<byte>( stream.Length, Allocator.Temp );
        stream.ReadBytes( bytes );
        string recMsg = Encoding.ASCII.GetString( bytes.ToArray() );
        NetworkHeader header = JsonUtility.FromJson<NetworkHeader>( recMsg );

        switch( header.cmd )
        {
            case Commands.CONNECTED:
                ConnectMsg hsMsg = JsonUtility.FromJson<ConnectMsg>( recMsg );
                Debug.Log( "[Client] Handshake message received! " + hsMsg.player.id );
                GameplayManager.Instance.SpawnPlayer( hsMsg.player, true );
                break;
            case Commands.PLAYER_UPDATE:
                PlayerUpdateMsg puMsg = JsonUtility.FromJson<PlayerUpdateMsg>( recMsg );
                Debug.Log( "[Client] Player update message received!" );
                break;
            case Commands.SERVER_UPDATE:
                ServerUpdateMsg suMsg = JsonUtility.FromJson<ServerUpdateMsg>( recMsg );
                Debug.Log( "[Client] Server update message received!" + suMsg.players.ToArrayString() );
                GameplayManager.Instance.UpdatePlayers( suMsg.players );
                break;
            default:
                Debug.Log( "[Client] Unrecognized message received!" );
                break;
        }
    }

    void Disconnect()
    {
        m_Connection.Disconnect( m_Driver );
        m_Connection = default( NetworkConnection );
    }

    void OnDisconnect()
    {
        Debug.Log( "[Client] got disconnected from server" );
        m_Connection = default( NetworkConnection );
    }

    public void OnDestroy()
    {
        m_Driver.Dispose();
    }
    void Update()
    {
        m_Driver.ScheduleUpdate().Complete();

        if( !m_Connection.IsCreated )
        {
            return;
        }

        DataStreamReader stream;
        NetworkEvent.Type cmd;
        cmd = m_Connection.PopEvent( m_Driver, out stream );
        while( cmd != NetworkEvent.Type.Empty )
        {
            if( cmd == NetworkEvent.Type.Connect )
            {
                OnConnect();
            }
            else if( cmd == NetworkEvent.Type.Data )
            {
                OnData( stream );
            }
            else if( cmd == NetworkEvent.Type.Disconnect )
            {
                OnDisconnect();
            }

            cmd = m_Connection.PopEvent( m_Driver, out stream );
        }

        SendLocalPlayerData();

    }

    void SendLocalPlayerData()
    {
        UnitBase localPlayer = PlayerController.Instance.localPlayer;
        if( localPlayer )
        {
            // send data at least once in two seconds
            if( PlayerController.Instance.HasCommand() || Time.time - lastTimestamp > 2.0 )
            {
                PlayerUpdateMsg puMsg = new PlayerUpdateMsg( localPlayer.GetPlayerData(), PlayerController.Instance.PopCommands() );
                SendToServer( JsonUtility.ToJson( puMsg ) );
                PlayerController.Instance.ClearDirtyFlag();
            }
        }
    }
}