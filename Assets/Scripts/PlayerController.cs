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
    LookUp,
    LookDown,
    FireBullet,
};

public class PlayerController : Singleton<PlayerController>
{
    public string playerId;
    public UnitBase localPlayer;
    public bool isLocalPlayer;
    
    public GameObject cameraSpot;
    public GameObject modelObj;
    Material material;

    public float moveSpeed = 1.0f;
    public float angularSpeed = 60.0f;

    // messages received from the server
    Queue<PlayerCommand> commandQueue = new Queue<PlayerCommand>();
    // messages to be sent to the server
    Queue<PlayerCommand> messageQueue = new Queue<PlayerCommand>();

    /// <summary>
    /// projectile variables
    /// </summary>
    public Transform bulletSpawnerTransform;
    public Bullet bulletPrefab;
    public float weaponCooldown;
    public float cooldownTime;

    /// <summary>
    /// player properties
    /// </summary>
    public float currentHealth;
    public float maxHealth = 100;
    bool IsAlive;

    /// <summary>
    /// UI features
    /// </summary>
    public GameObject uiPanel;
    public TextMeshProUGUI clientIdText;
    public Slider healthBar;

    public void Awake()
    {
        var cubeRenderer = GetComponentInChildren<Renderer>();
        material = cubeRenderer.material;
    }

    private void Start()
    {
        Reset();
    }

    public void Reset()
    {
        currentHealth = maxHealth;
        IsAlive = true;
        healthBar.value = 1.0f;
        modelObj.SetActive( true );
    }

    public void FixedUpdate()
    {
        if( isLocalPlayer )
        {
            Transform curTransform = transform;
            if( IsAlive )
            {
                if( Input.GetKey( KeyCode.W ) )
                {
                    curTransform.position += curTransform.forward * Time.deltaTime * moveSpeed;
                }
                if( Input.GetKey( KeyCode.S ) )
                {
                    curTransform.position -= curTransform.forward * Time.deltaTime * moveSpeed;
                }
                if( Input.GetKey( KeyCode.A ) )
                {
                    curTransform.position -= curTransform.right * Time.deltaTime * moveSpeed;
                }
                if( Input.GetKey( KeyCode.D ) )
                {
                    curTransform.position += curTransform.right * Time.deltaTime * moveSpeed;
                }

                // mouse right drag
                if( Input.GetMouseButton( 1 ) )
                {
                    float rotation = Input.GetAxis( "Mouse X" ) * angularSpeed;
                    curTransform.Rotate( Vector3.up, rotation );
                }
            }

            if( CanvasManager.Instance.prediction.isOn )
            {
                StartCoroutine( UpdateTransform( curTransform, NetworkManager.Instance.estimatedLag ) );
            }
            else
            {
                transform.position = curTransform.position;
                transform.rotation = curTransform.rotation;
            }

        }
        else
        {
            uiPanel.transform.rotation = Camera.main.transform.rotation;
        }

        if( Input.GetKeyDown( KeyCode.Space ) )
        {
            if( cooldownTime <= 0.0f )
                SendCommand( PlayerCommand.FireBullet );
        }

        if( cooldownTime > 0.0f )
        {
            cooldownTime -= Time.fixedDeltaTime;
        }

        if( currentHealth <= 0 && IsAlive )
        {
            Die();
        }
        else if( commandQueue.Count > 0 )
        {
            ExecuteCommand( commandQueue.Dequeue() );
        }
    }

    public void SendCommand( PlayerCommand command )
    {
        Debug.Log( "[ " + Time.time.ToString() + "] Send command : " + command.ToString() );
        messageQueue.Enqueue( command );
    }

    public bool HasMessage()
    {
        return messageQueue.Count > 0;
    }

    public string PopMessage()
    {
        return messageQueue.Dequeue().ToString();
    }

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

    public void SetId( string clientId, bool isLocal )
    {
        playerId = clientId;
        if( clientIdText != null )
        {
            clientIdText.text = playerId.Split( new char[] { '(', ',', ')' } )[2];
            clientIdText.color = isLocal ? Color.red : Color.gray;
        }

        isLocalPlayer = isLocal;
        if( isLocal )
        {
            Camera.main.transform.parent = cameraSpot.transform;
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localRotation = Quaternion.identity;
        }
    }

    IEnumerator UpdateTransform( Transform newTransform, float waittingTime )
    {
        yield return new WaitForSeconds( waittingTime );
        transform.position = newTransform.position;
        transform.rotation = newTransform.rotation;
    }

    public void SetColor( Color color )
    {
        material.SetColor( "_Color", color );
    }

    public void FireBullet()
    {
        ////Debug.Log( string.Format( "{0} : fire", id ) );
        //if( cooldownTime <= 0.0f )
        //{
        //    Bullet bullet = Instantiate( bulletPrefab, bulletSpawnerTransform.position, bulletSpawnerTransform.rotation );
        //    bullet.ownerId = id;
        //    bullet.Fire();
        //    cooldownTime = weaponCooldown; 
        //}
    }

    public void TakeDamage( float damage )
    {
        currentHealth = Mathf.Max( currentHealth - damage, 0.0f );
        SetHealth(currentHealth);
    }

    public void SetHealth( float health )
    {
        currentHealth = health;
        healthBar.value = currentHealth / maxHealth;
    }

    void Die()
    {
        Debug.Log( "Player Die : " + playerId );
        IsAlive = false;
        modelObj.SetActive(false);

        if( isLocalPlayer )
        {
            CanvasManager.Instance.ShowDeadUI();
        }
    }
}
