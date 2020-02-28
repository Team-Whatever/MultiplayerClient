using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : PlayerBaseState
{
    [Tooltip("run if this is greater than 0.5, otherwise walk")]
    public float moveSpeed = 0.6f;

    float prevAngularSpeed;
	public override void Enter()
	{
        owner.StartNavigation();
        
        owner.animator.SetBool( "Static_b", false );
        owner.animator.SetFloat( "Speed_f", moveSpeed );

        prevAngularSpeed = owner.agent.angularSpeed;
        owner.agent.angularSpeed = 0.0f;
    }

	public override void Execute()
	{
        if( player.agent.isStopped == true )
            ChangeState( UnitState.Idle );
    }

	public override void Exit()
	{
        owner.StopMove();
        owner.animator.SetBool( "Static_b", true );
        owner.animator.SetFloat( "Speed_f", 0.0f );
        owner.agent.angularSpeed = prevAngularSpeed;
    }

	public override string ToString()
	{
		return "Run";
	}
}
