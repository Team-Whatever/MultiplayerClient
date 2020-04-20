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
    Respawn,
};

[System.Serializable]
public class PlayerCommandData
{
    public string playerId;
    public PlayerCommand command;
    public float value;

    public PlayerCommandData( string id, PlayerCommand cmd )
        : this( id, cmd, 0.0f )
    {
    }

    public PlayerCommandData( string id, PlayerCommand cmd, float val )
    {
        playerId = id;
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
            }
            if( Input.GetKey( KeyCode.S ) )
            {
                moveVector -= curTransform.forward;
            }
            if( Input.GetKey( KeyCode.A ) )
            {
                moveVector -= curTransform.right;
            }
            if( Input.GetKey( KeyCode.D ) )
            {
                moveVector += curTransform.right;
            }

            // mouse right drag
            float rotationValue = Input.GetAxis( "Mouse X" );
            float verticalValue = Input.GetAxis( "Mouse Y" );
            StartCoroutine( ASyncUpdateTransform( localPlayer, moveVector, rotationValue, verticalValue, GameplayManager.estimatedLag ) );

            if( Input.GetMouseButton( 0 ) || Input.GetKeyDown( KeyCode.Space ) )
            {
                if( localPlayer.CanAttack() )
                {
                    AddCommand( PlayerCommand.FireBullet );
                    StartCoroutine( AsyncFireBullet( localPlayer, GameplayManager.estimatedLag ) );
                }
            }
        }
    }

    IEnumerator ASyncUpdateTransform( UnitBase unit, Vector3 moveVector, float horizontalRotation, float verticalRotation, float waitingTime )
    {
        yield return new WaitForSeconds( waitingTime );
        if( moveVector != Vector3.zero )
        {
            unit.MoveBy( moveVector );
        }
        else
        {
            unit.StopMove();
        }

        if( horizontalRotation != 0.0f )
        {
            unit.Rotate( horizontalRotation );
        }
        //if( verticalRotation != 0.0f )
        //{
        //    // plus : goes up, but x-angle goes negative
        //    unit.LookUp( -verticalRotation );
        //}
    }

    IEnumerator AsyncFireBullet( UnitBase unit, float lagTime )
    {
        yield return new WaitForSeconds( lagTime );
        unit.FireBullet();
    }

    public void AddCommand( PlayerCommand command )
    {
        AddCommand( command, 0.0f );
    }

    public void AddCommand( PlayerCommand command, float value )
    {
        Debug.Log( "[ " + Time.time.ToString() + "] Send command : " + command.ToString() + ", value = " + value );
        commandQueue.Add( new PlayerCommandData( localPlayerId, command, value ) );
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
        HUD.Instance.ShowDeathPopup();
    }
}
