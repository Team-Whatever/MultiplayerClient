using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : PlayerBaseState
{
    [Tooltip("run if this is greater than 0.5, otherwise walk")]
    public float moveSpeed = 0.6f;
	public override void Enter()
	{
        owner.StartNavigation();
        
        owner.animator.SetBool( "Static_b", false );
        owner.animator.SetFloat( "Speed_f", moveSpeed );
    }

	public override void Execute()
	{
        if( player.isMoving == false )
            ChangeState( UnitState.Idle );

        owner.transform.rotation = Quaternion.LookRotation( player.movingDirection );
        //owner.transform.Translate( player.movingDirection * player.moveSpeed * Time.deltaTime, Space.World );
        player.MoveAgent(player.movingDirection);
        owner.aimingDirection = this.transform.forward;
    }

	public override void Exit()
	{
        owner.animator.SetBool( "Static_b", true );
        owner.animator.SetFloat( "Speed_f", 0.0f );
    }

	public override string ToString()
	{
		return "Run";
	}
}
