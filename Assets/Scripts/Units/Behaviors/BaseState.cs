using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState : MonoBehaviour
{
    public UnitState unitState;

    [HideInInspector]
    public UnitBase owner;

    protected virtual void Awake()
    {
        owner = GetComponentInParent<UnitBase>();
    }

    public abstract void Enter();
    public abstract void Execute();
    public abstract void Exit();

    protected void ChangeState( UnitState state )
    {
        owner.ChangeState( state );
    }

    protected void ChangeAnimation( AnimState anim )
    {
        owner.ChangeAnimation( anim );
    }
}
