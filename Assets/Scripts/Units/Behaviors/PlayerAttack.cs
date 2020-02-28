using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : PlayerBaseState
{
	public override void Enter()
	{
        owner.animator.SetBool( "Shoot_b", true );
        owner.animator.SetInteger( "WeaponType_int", (int)owner.currentWeapon.weaponType );
    }

	public override void Execute()
	{
	}

	public override void Exit()
	{
        owner.animator.SetBool( "Shoot_b", false );
    }

	public override string ToString()
	{
		return "Attack";
	}
}
