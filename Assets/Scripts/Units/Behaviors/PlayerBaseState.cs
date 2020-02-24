using UnityEngine;

public abstract class PlayerBaseState : BaseState
{
    protected PlayerUnit player;
    protected override void Awake()
    {
        base.Awake();
        player = owner as PlayerUnit;
    }
}
