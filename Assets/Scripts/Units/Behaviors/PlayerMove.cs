using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : PlayerBaseState
{
	public override void Enter()
	{
        owner.StartNavigation();
        ChangeAnimation(AnimState.Run);
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
	}

	public override string ToString()
	{
		return "Run";
	}
}
