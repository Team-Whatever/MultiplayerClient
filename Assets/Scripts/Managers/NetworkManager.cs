using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net.Sockets;
using System.Net;

[Serializable]
public class PlayerInfoData
{
    public string id;
    public Color color;
    
    public Vector3 pos;
    public Quaternion rotation;

    public float health;

    public string command;
}


public class NetworkManager : Singleton<NetworkManager>
{
    List<PlayerReceivedData> newPlayers = new List<PlayerReceivedData>();
    List<PlayerReceivedData> disconnectedPlayers = new List<PlayerReceivedData>();

    public UdpClient udp;
    public string serverIp = "3.219.69.41";
    public int serverPort = 12345;
    public string clientId;

    public float numUpdatePerSecond = 10.0f;
    public float estimatedLag = 200.0f; // in mili seconds

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        udp = new UdpClient();

        udp.Connect( serverIp, serverPort );

        Byte[] sendBytes = Encoding.ASCII.GetBytes("{\'message\':\'connect\'}");
      
        udp.Send(sendBytes, sendBytes.Length);

        udp.BeginReceive(new AsyncCallback(OnReceived), udp);

        InvokeRepeating("HeartBeat", 1.0f, 1.0f / numUpdatePerSecond );
    }

    void OnDestroy(){
        udp.Dispose();
    }


    public enum commands{
        SERVER_CONNECTED,
        NEW_CLIENT,
        UPDATE,
        CLIENT_DROPPED,
        CLIENT_FIRE,
    };
    
    [Serializable]
    public class Message{
        public commands cmd;
    }

    [Serializable]
    public struct receivedColor
    {
        public float R;
        public float G;
        public float B;

        public static implicit operator Color( receivedColor value )
        {
            return new Color( value.R, value.G, value.B );
        }
    }

    [Serializable]
    public struct receivedPos
    {
        public float x;
        public float y;
        public float z;

        public override string ToString()
        {
            return String.Format( "[ {0}, {1}, {2} ]", x, y, z );
        }

        public static implicit operator Vector3( receivedPos value )
        {
            return new Vector3( value.x, value.y, value.z );
        }
    }

    [Serializable]
    public struct receivedRotation
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public static implicit operator Quaternion( receivedRotation value )
        {
            return new Quaternion( value.x, value.y, value.z, value.w );
        }
    }

    [Serializable]
    public class PlayerReceivedData {
        public string id;
        public receivedColor color;
        public receivedPos pos;
        public receivedRotation rotation;
        public float health;
        public string command;


        public static implicit operator PlayerInfoData( PlayerReceivedData data )
        {
            PlayerInfoData info = new PlayerInfoData();
            info.id = data.id;
            info.color = data.color;
            info.pos = data.pos;
            info.rotation = data.rotation;
            info.health = data.health;
            info.command = data.command;
            return info;
        }
    }

    [Serializable]
    public class NewPlayer{
        public commands cmd;
        public PlayerReceivedData player;
    }

    [Serializable]
    public class GameState{
        public commands cmd;
        public PlayerReceivedData[] players;
    }

    public Message latestMessage;
    public GameState lastestGameState;
    bool isGameStateProcessed = false;

    GameState previousGameState = null;
    float previousTime = 0;
    float latestTime = 0;
    float currentTime;
    
    void OnReceived(IAsyncResult result){
        // this is what had been passed into BeginReceive as the second parameter:
        UdpClient socket = result.AsyncState as UdpClient;
        
        // points towards whoever had sent the message:
        IPEndPoint source = new IPEndPoint(0, 0);

        // get the actual message and fill out the source:
        byte[] message = socket.EndReceive(result, ref source);
        
        // do what you'd like with `message` here:
        string returnData = Encoding.ASCII.GetString(message);
        //Debug.Log("Got this: " + returnData);
        
        latestMessage = JsonUtility.FromJson<Message>(returnData);
        try{
            switch(latestMessage.cmd){
                case commands.SERVER_CONNECTED:
                    {
                        NewPlayer newPlayer = JsonUtility.FromJson<NewPlayer>( returnData );
                        clientId = newPlayer.player.id;
                        newPlayers.Add( newPlayer.player );
                        break;
                    }
                case commands.NEW_CLIENT:
                    {
                        NewPlayer newPlayer = JsonUtility.FromJson<NewPlayer>( returnData );
                        newPlayers.Add( newPlayer.player );
                        break;
                    }
                case commands.UPDATE:
                    previousTime = latestTime;
                    previousGameState = lastestGameState;

                    latestTime = currentTime;
                    lastestGameState = JsonUtility.FromJson<GameState>( returnData );
                    isGameStateProcessed = false;
                    break;
                case commands.CLIENT_DROPPED:
                    NewPlayer droppedPlayer = JsonUtility.FromJson<NewPlayer>( returnData );
                    disconnectedPlayers.Add( droppedPlayer.player );
                    break;
                default:
                    Debug.Log("Error");
                    break;
            }
        }
        catch (Exception e){
            Debug.Log(e.ToString());
        }
        
        // schedule the next receive operation once reading is done:
        socket.BeginReceive(new AsyncCallback(OnReceived), socket);
    }

    void SpawnPlayers(  ){
        if( newPlayers.Count > 0 )
        {
            foreach( var newPlayer in newPlayers )
            {
                Vector3 pos = new Vector3( newPlayer.pos.x, newPlayer.pos.y, newPlayer.pos.z );

                //PlayerController player = Instantiate( playerPrefab );
                //player.transform.position = pos;
                //player.SetId( newPlayer.id, clientId == newPlayer.id );
                //playerUnits.Add( newPlayer.id, player );

                GameplayManager.Instance.SpawnPlayer( newPlayer.id, pos, clientId == newPlayer.id );
            }
            newPlayers.Clear();
        }
    }

    void UpdatePlayers(){
        if( lastestGameState != null & isGameStateProcessed == false )
        {
            foreach( var player in lastestGameState.players )
            {
                bool prevPlayerExist = false;
                PlayerReceivedData prevPlayer = null;
                float delta = 0.0f;
                if( previousGameState != null )
                {
                    prevPlayerExist = Array.Exists( previousGameState.players, p => p.id == player.id );
                    prevPlayer = Array.Find( previousGameState.players, p => p.id == player.id );
                    delta = ( Time.time - latestTime ) / ( latestTime - previousTime );
                }
                if( prevPlayerExist )
                    GameplayManager.Instance.UpdatePlayer( player, prevPlayer, delta );
                else
                    GameplayManager.Instance.UpdatePlayer( player, null, delta );
            }
            isGameStateProcessed = true;
        }
    }

    void DestroyPlayers(){
        if( disconnectedPlayers.Count > 0 )
        {
            foreach( var droppedPlayer in disconnectedPlayers )
            {
                GameplayManager.Instance.DisconnectPlayer( droppedPlayer.id );
            }
            disconnectedPlayers.Clear();
        }
    }
    
    void HeartBeat(){

        if( clientId != null )
        {
            PlayerController localPlayer = GameplayManager.Instance.GetLocalPlayer();
            PlayerInfoData data = new PlayerInfoData();
            data.id = clientId;
            data.pos = localPlayer.transform.position;
            data.rotation = localPlayer.transform.rotation;
            data.health = localPlayer.currentHealth;
            if( localPlayer.HasMessage() )
                data.command = localPlayer.PopMessage();
            string messageData = JsonUtility.ToJson( data );

            Byte[] sendBytes = Encoding.ASCII.GetBytes(messageData);
            udp.Send( sendBytes, sendBytes.Length );
        }
    }

    void Update(){
        currentTime = Time.time;
        SpawnPlayers();
        UpdatePlayers();
        DestroyPlayers();
    }
}