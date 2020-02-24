using System;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    protected BaseState curState;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if( curState != null )
            curState.Execute();
    }

    protected void ChangeState( BaseState newState )
    {
        if( newState != curState )
        {
            if( curState != null )
            {
                //DebugExtension.LogLevel( curState.owner.ToString() + " is exiting " + curState.ToString() + " state.", DebugExtension.LogType.AI );
                curState.Exit();
            }
            curState = newState;
            if( curState != null )
            {
                //DebugExtension.LogLevel( curState.owner.ToString() + " is entering " + curState.ToString() + " state.", DebugExtension.LogType.AI );
                curState.Enter();
            }
        }
    }
}
