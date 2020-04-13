using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public enum UnitState
{
    Idle,
    Move,
    Wander,
    Chase,
    Attack,
    Dodge,
    Flee,
    Die,
}

public enum AnimState
{
    Idle,
    Walking,
    Run,
    Attack,
    Dodging,
    Dying,
}

public enum WeaponType
{
    HandGun = 1,
    Auto1 = 2,
    Auto2 = 3,
    ShotGun = 4,
    Rifle1 = 5,
    Rifle2 = 6,
    SubMachineGun = 7,
    RPG_Shoot = 8,
    MiniGun = 9,
    Grenade = 10,
}

[System.Serializable] public class EventUnitKilled : UnityEvent<UnitBase> { }

public class UnitBase : StateMachine
{
    // returns true when a unit is spawned on the server
    public static bool IsRunOnServer;

    /// <summary>
    /// Prefabs & children
    /// </summary>
    public GameObject killParticle;
    public GameObject model;
    public Animator animator;
    public GameObject cameraSpot;
    public PlayerUI unitUI;
    public GameObject[] unitModels;

    /// <summary>
    /// Navigation Agent
    /// </summary>
    public NavMeshAgent agent;
    public float moveSpeed = 10;
    public float angularSpeed = 30;
    public Vector3? targetPosition;
    public Quaternion? targetRotation;

    /// <summary>
    /// Identification
    /// </summary>
    public bool IsLocalPlayer { get; set; }
    public string PlayerId
    {
        get
        {
            return playerInfo != null ? playerInfo.id : string.Empty;
        }
    }
    public int TeamId
    {
        get
        {
            return playerInfo != null ? playerInfo.teamId : -1;
        }
    }
    public int UnitId
    {
        get
        {
            return playerInfo != null ? playerInfo.unitId : -1;
        }
    }


    /// <summary>
    /// Server side events
    /// </summary>
    // messages received from the server
    Queue<PlayerCommand> commandQueue = new Queue<PlayerCommand>();
    // checks if the unit data is up to dated
    public bool IsLatestDataReceived { get; set; }
    PlayerData playerInfo;

    /// <summary>
    /// Weapon
    /// </summary>
    public WeaponBase currentWeapon;

    /// <summary>
    /// Health
    /// </summary>
    public float CurrentHealth {
        get { return playerInfo != null ? playerInfo.health : 0.0f; }
        set { if( playerInfo != null ) playerInfo.health = value; }
    }
    public float maxHealth;
    public float HealthRate
    {
        get { return CurrentHealth / maxHealth; }
    }
    public bool IsAlive
    {
        get { return CurrentHealth > 0.0f; }
    }
    public bool IsMaxHealth
    {
        get { return CurrentHealth == maxHealth; }
    }
    public EventUnitKilled UnitKilled;

    /// <summary>
    /// States
    /// </summary>
    public UnitState State { get { return curState.unitState; } }
    Dictionary<UnitState, BaseState> _states;

    /// <summary>
    /// Targetting
    /// </summary>
    public Vector3 aimingDirection;

    /// <summary>
    /// Debug Information
    /// </summary>
    public bool debugDraw = false;

    protected void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
    }

    protected virtual void Awake()
    {
        _states = new Dictionary<UnitState, BaseState>();
        BaseState[] allStates = GetComponentsInChildren<BaseState>();
        foreach( var state in allStates )
        {
            _states.Add( state.unitState, state );
        }

        CurrentHealth = maxHealth;
    }

    protected virtual void Start()
    {
        Reset();

        EnableDebug( false );
    }

    public void Reset()
    {
        CurrentHealth = maxHealth;
        model.SetActive( true );

        currentWeapon.owner = this;
        ChangeState( UnitState.Idle );
    }   

    [System.Diagnostics.Conditional("DEBUG")]
    void EnableDebug( bool enable )
    {
        if( Debug.isDebugBuild )
            debugDraw = enable;
    }

    public void ChangeState( UnitState newState )
    {
        if( _states.ContainsKey( newState ) )
            ChangeState( _states[newState] );
    }

    public void ChangeAnimation( AnimState anim )
    {
        //animator.SetInteger( "AnimState", ( int )anim );
        //DebugExtension.LogLevel( "Change Animation to " + anim.ToString(), DebugExtension.LogType.Animation );
        //animator.SetTrigger( anim.ToString() );
    }

    protected virtual void FixedUpdate()
    {
        if( !IsAlive )
            return;

        if( curState != null )
            curState.Execute();

        if( IsLocalPlayer )
        {

        }
        else
        {
            float timeElapsed = Mathf.Clamp( Time.time - GameplayManager.Instance.lastUpdatedTime, 0.0f, 1.0f );
            if( targetPosition.HasValue )
            {
                transform.position = Vector3.Lerp( transform.position, targetPosition.Value, timeElapsed );
            }
            if( targetRotation.HasValue )
            {
                transform.rotation = Quaternion.Lerp( transform.rotation, targetRotation.Value, timeElapsed );
            }

            if( unitUI )
                unitUI.gameObject.transform.rotation = Camera.main.transform.rotation;
        }

        if( debugDraw )
        {
            //DebugExtension.DrawCircle( transform.position, sightRange, Color.green );
            //DebugExtension.DrawCircle( transform.position, currentWeapon.attackRange, Color.red );
            //unitUI.DebugInfoText.text = curState.ToString();
        }
        else
        {
            //unitUI.DebugInfoText.text = string.Empty;
        }

        if( commandQueue.Count > 0 )
        {
            ExecuteCommand( commandQueue.Dequeue() );
        }
    }

    #region Commands
    public void AddCommand( string commandStr )
    {
        PlayerCommand command;
        if( System.Enum.TryParse( commandStr, out command ) )
            AddCommand( command );
    }

    public void AddCommand( PlayerCommand command )
    {
        Debug.Log( "[" + Time.time.ToString() + "] receive command : " + command.ToString() );
        commandQueue.Enqueue( command );
    }

    void ExecuteCommand( PlayerCommand command )
    {
        Debug.Log( "[ " + Time.time.ToString() + "] execute command : " + command.ToString() );
        switch( command )
        {
            case PlayerCommand.FireBullet:
                FireBullet();
                break;
            default:
                break;
        }
    }

    public void FireBullet()
    {
        ChangeState( UnitState.Attack );
    }


    #endregion

    #region User Data
    
    public void SetPlayerData( PlayerData data, bool isLocal )
    {
        playerInfo = data;
        SetModel( data.unitId );

        if( unitUI )
            unitUI.SetUserData( data.id, isLocal );

        transform.position = data.position;
        transform.rotation = data.rotation;

        IsLocalPlayer = isLocal;
        if( IsLocalPlayer )
        {
            Camera.main.transform.parent = cameraSpot.transform;
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localRotation = Quaternion.identity;
        }
    }

    public void ValidatePlayerData()
    {
        playerInfo.position = transform.position;
        playerInfo.rotation = transform.rotation;
    }

    public PlayerData GetPlayerData()
    {
        return playerInfo;
    }

    public void UpdatePlayerData( PlayerData data, bool isLocalPlayer )
    {
        playerInfo = data;
        if( !isLocalPlayer )
        {
            targetPosition = data.position;
            targetRotation = data.rotation;
        }
        SetHealth( data.health );
        playerInfo.lastUpdateTime = Time.time;
    }

    void SetModel( int unitId )
    {
        for( int i = 0; i < unitModels.Length; i++ )
        {
            unitModels[i].SetActive( i == unitId );
        }
    }

    #endregion

    #region Nav Mesh Agent
    public void StartNavigation()
    {
        if( agent )
            agent.isStopped = false;
    }

    public void StopMove()
    {
        if( agent )
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
    }

    public void MoveTo( Vector3 position )
    {
        if( agent )
        {
            agent.SetDestination( position );
            if( agent.isStopped == true )
                ChangeState( UnitState.Move );
        }
    }

    public void MoveBy( Vector3 direction )
    {
        if( agent )
        {
            //Debug.Log( string.Format( "Move by {0}", direction.ToString() ) );
            agent.velocity = direction * moveSpeed;

            if( agent.isStopped == true )
                ChangeState( UnitState.Move );
        }
    }

    public void Rotate( float angle )
    {
        transform.Rotate( Vector3.up, angle * angularSpeed );
    }

    public bool IsReachedTarget()
    {
        if( !agent.pathPending )
        {
            if( debugDraw )
            {
                Debug.DrawLine( transform.position, agent.destination, Color.blue );
            }
            if( agent.remainingDistance <= agent.stoppingDistance )
            {
                if( !agent.hasPath || agent.velocity.sqrMagnitude <= float.Epsilon )
                    return true;
            }
        }
        return false;
    }


    #endregion

    public void Die()
    {
        gameObject.SetActive(false);
        if( IsLocalPlayer )
            PlayerController.Instance.OnDead();
        //UnitKilled.Invoke(this);
    }

    public virtual void TakeDamage(float damage)
    {
        if( IsAlive )
        {
            CurrentHealth = Mathf.Max( CurrentHealth - damage, 0.0f );
            if( CurrentHealth <= 0 )
                Die();
            else
            {
                unitUI.SetHealthBarProgress( HealthRate );
            }
        }
    }

    public void Heal(float healAmount)
    {
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + healAmount);

        //unitUI.SetEnergyBarProgress(HealthRate);
    }

    public virtual void SetHealth(float health)
    {
        bool isDead = false;
        if( health <= 0.0f && CurrentHealth > 0.0f )
            isDead = true;

        CurrentHealth = Mathf.Min( maxHealth, health );

        if( isDead )
            Die();
        else
            unitUI.SetHealthBarProgress( HealthRate );
    }

    #region Weapon functions

    public void OnBeginAttack()
    {
        currentWeapon.OnBeginAttack();
    }

    public void OnFire( )
    {
        currentWeapon.OnFire();
    }

    public void OnEndAttack()
    {
        currentWeapon.OnEndAttack();
        ChangeState( UnitState.Idle );
    }

    public void Aim( Vector3 target )
    {
        aimingDirection = ( target - transform.position ).normalized;
        if( debugDraw )
        {
            Debug.DrawLine( transform.position, target, Color.red );
            //Debug.Log( this.ToString() + " : Look at : " + target.position.ToString() + " with dir of : " + aimingDirection.ToString() );
        }
        transform.rotation = Quaternion.LookRotation( aimingDirection );
    }

    public virtual bool CanAttack()
    {
        return currentWeapon.CanAttack();
    }

    #endregion

}
