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
    /// <summary>
    /// Prefabs & children
    /// </summary>
    public GameObject killParticle;
    public GameObject model;
    public Animator animator;
    public GameObject cameraSpot;
    public PlayerUI unitUI;

    /// <summary>
    /// Navigation Agent
    /// </summary>
    public NavMeshAgent agent;
    public float moveSpeed;

    /// <summary>
    /// Identification
    /// </summary>
    public GameObject[] unitModels;
    [HideInInspector] public string playerId;
    public int teamId;
    [HideInInspector] public bool isLocalPlayer;

    /// <summary>
    /// Server side events
    /// </summary>
    // messages received from the server
    Queue<PlayerCommand> commandQueue = new Queue<PlayerCommand>();

    /// <summary>
    /// Weapon
    /// </summary>
    public WeaponBase currentWeapon;

    /// <summary>
    /// Health
    /// </summary>
    public float currentHealth;
    public float maxHealth;
    public float HealthRate
    {
        get { return currentHealth / maxHealth; }
    }
    public bool IsAlive
    {
        get { return currentHealth > 0.0f; }
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

        currentHealth = maxHealth;
    }

    protected virtual void Start()
    {
        Reset();

        EnableDebug( false );
    }

    public void Reset()
    {
        currentHealth = maxHealth;
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

        if( isLocalPlayer )
        {

        }
        else
        {
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
    public void SetUserId( string clientId, int unitId, int teamId, bool isLocal )
    {
        this.playerId = clientId;
        this.teamId = teamId;
        if( unitUI )
            unitUI.SetUserData( clientId, isLocal );

        if( unitId < unitModels.Length )
        {
            for( var i = 0; i < unitModels.Length; i++ )
            {
                unitModels[i].SetActive( i == unitId );
            }
        }

        isLocalPlayer = isLocal;
        if( isLocalPlayer )
        {
            Camera.main.transform.parent = cameraSpot.transform;
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localRotation = Quaternion.identity;
        }
    }

    #endregion

    #region Nav Mesh Agent
    public void StartNavigation()
    {
        if( agent )
            agent.isStopped = false;
    }

    public void StopNavigation()
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
        }
    }

    public void MoveBy( Vector3 direction )
    {
        if( agent )
        {
            agent.velocity = direction * moveSpeed;
        }
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
        if( isLocalPlayer )
            PlayerController.Instance.OnDead();
        //UnitKilled.Invoke(this);
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max( currentHealth - damage, 0.0f );
        if( currentHealth <= 0 )
            Die();
        else
        {
            unitUI.SetHealthBarProgress(HealthRate);
        }
            
    }

    public void Heal(float healAmount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);

        //unitUI.SetEnergyBarProgress(HealthRate);
    }

    public void SetHealth(float health)
    {
        currentHealth = Mathf.Min( maxHealth, health );

        //unitUI.SetEnergyBarProgress( HealthRate );
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
