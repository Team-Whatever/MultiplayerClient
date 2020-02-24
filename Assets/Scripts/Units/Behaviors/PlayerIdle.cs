using System.Collections;
using UnityEngine;

public class PlayerIdle : BaseState
{

    public override void Enter()
    {
        owner.StopNavigation();
        ChangeAnimation( AnimState.Idle );
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
