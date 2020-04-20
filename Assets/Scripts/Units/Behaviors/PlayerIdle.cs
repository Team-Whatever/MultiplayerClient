using System.Collections;
using UnityEngine;

public class PlayerIdle : BaseState
{
    public override void Enter()
    {
        owner.animator.SetFloat( "Speed_f", 0.0f );
    }

    public override void Execute()
    {
    }

    public override void Exit()
    {
    }

    public override string ToString()
    {
        return "Idle";
    }

}
