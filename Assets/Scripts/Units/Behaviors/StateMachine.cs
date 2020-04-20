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
                //Debug.Log( curState.owner.ToString() + " is exiting " + curState.ToString() + " state." );
                curState.Exit();
            }
            curState = newState;
            if( curState != null )
            {
                //Debug.Log( curState.owner.ToString() + " is entering " + curState.ToString() + " state." );
                curState.Enter();
            }
        }
    }
}
