using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : PlayerBaseState
{
	public override void Enter()
	{
		player.StopNavigation();
		ChangeAnimation(AnimState.Attack);
	}

	public override void Execute()
	{
	}

	public override void Exit()
	{
	}

	public override string ToString()
	{
		return "Attack";
	}
}
