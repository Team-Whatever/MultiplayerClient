using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum PlayerCommand
{
    MoveForward,
    MoveBackward,
    MoveLeft,
    MoveRight,
    RotateLeft,
    RotateRight,
    TurnHorizontal,
    LookUp,
    LookDown,
    FireBullet,
};

[System.Serializable]
public class PlayerCommandData
{
    public PlayerCommand command;
    public float value;

    public PlayerCommandData( PlayerCommand cmd )
        : this( cmd, 0.0f )
    {
    }

    public PlayerCommandData( PlayerCommand cmd, float val )
    {
        command = cmd;
        value = val;
    }
}

public class PlayerController : Singleton<PlayerController>
{
    [HideInInspector] public UnitBase localPlayer;
    [HideInInspector] public bool isLocalPlayer;
    [HideInInspector] public string localPlayerId;
    
    // messages to be sent to the server
    List<PlayerCommandData> commandQueue = new List<PlayerCommandData>();
    public bool IsDirtyFlag { get; private set; }

    public void Awake()
    {
    }

    private void Start()
    {
    }

    public void FixedUpdate()
    {
        if( localPlayer && localPlayer.IsAlive )
        {
            Transform curTransform = localPlayer.transform;
            Vector3 moveVector = Vector3.zero;
            if( Input.GetKey( KeyCode.W ) )
            {
                moveVector += curTransform.forward;
                AddCommand( PlayerCommand.MoveForward );
            }
            if( Input.GetKey( KeyCode.S ) )
            {
                moveVector += -curTransform.forward;
                AddCommand( PlayerCommand.MoveBackward );
            }
            if( Input.GetKey( KeyCode.A ) )
            {
                moveVector -= curTransform.right;
                AddCommand( PlayerCommand.MoveLeft );
            }
            if( Input.GetKey( KeyCode.D ) )
            {
                moveVector += curTransform.right;
                AddCommand( PlayerCommand.MoveRight );
            }
            if( moveVector != Vector3.zero )
                localPlayer.MoveBy( moveVector );
            else
                localPlayer.StopMove();

            // mouse right drag
            if( Input.GetMouseButton( 1 ) )
            {
                float axis = Input.GetAxis( "Mouse X" );
                if( axis != 0.0 )
                {
                    localPlayer.Rotate( axis );
                    AddCommand( PlayerCommand.TurnHorizontal, axis );
                }
            }
            else
            {
                if( Input.GetKey( KeyCode.Q ) )
                {
                    localPlayer.Rotate( -Time.fixedDeltaTime );
                    AddCommand( PlayerCommand.RotateLeft );
                }
                else if( Input.GetKey( KeyCode.E ) )
                {
                    localPlayer.Rotate( Time.fixedDeltaTime );
                    AddCommand( PlayerCommand.RotateRight );
                }
            }

            StartCoroutine( UpdateTransform( curTransform, 0.0f ) );


            if( Input.GetKeyDown( KeyCode.Space ) )
            {
                if( localPlayer.CanAttack() )
                    AddCommand( PlayerCommand.FireBullet );
            }
        }
    }

    IEnumerator UpdateTransform( Transform newTransform, float waitingTime )
    {
        yield return new WaitForSeconds( waitingTime );
        localPlayer.transform.position = newTransform.position;
        localPlayer.transform.rotation = newTransform.rotation;
    }

    public void AddCommand( PlayerCommand command )
    {
        AddCommand( command, 0.0f );
    }

    public void AddCommand( PlayerCommand command, float value )
    {
        Debug.Log( "[ " + Time.time.ToString() + "] Send command : " + command.ToString() + ", value = " + value );
        commandQueue.Add( new PlayerCommandData( command, value ) );
    }

    public bool HasCommand()
    {
        return commandQueue.Count > 0;
    }

    public List<PlayerCommandData> PopCommands()
    {
        var commands = new List<PlayerCommandData>(commandQueue);
        commandQueue.Clear();
        return commands;
    }

    public void OnDead()
    {
        CanvasManager.Instance.ShowDeadUI();
    }

    public void RevivePlayer()
    {
        localPlayer.Reset();

        // TODO : find player respawning point
        //localPlayer.transform.position = playerSpawner.transform.position;
    }

    public void ClearDirtyFlag()
    {
        IsDirtyFlag = false;
    }
}
