using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerUnit : UnitBase
{
    public bool isMoving;
    public Vector3 movingDirection;

    public void Move( Vector3 targetPos )
    {
        isMoving = true;
        movingDirection = targetPos;
        ChangeState( UnitState.Chase );
    }

    public void MoveAgent(Vector3 offset)
    {
        if (agent)
        {
            agent.velocity = offset * moveSpeed;
        }
    }

    public void StopMove()
    {
        if (agent && agent.isOnNavMesh)
        {
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
        }

        isMoving = false;
    }

    public void Attack()
    {
        // Idle state will change the state to the attack state
        // if the player can attack the target
    }

    public void Aim( Vector3 targetDirection )
    {
        // attack behavior will aim the target automatically
    }

    public void StopAim()
    {

    }
}


