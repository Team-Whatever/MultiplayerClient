using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDie : BaseState
{
    public override void Enter()
    {
        owner.animator.SetBool( "Death_b", true );
        owner.animator.SetInteger( "DeathType_int", Random.Range( 1, 2 ) );
    }

    public override void Execute()
    {
    }

    public override void Exit()
    {
        owner.animator.SetBool( "Death_b", false );
        owner.animator.Rebind();
    }

    public override string ToString()
    {
        return "Die";
    }

}
