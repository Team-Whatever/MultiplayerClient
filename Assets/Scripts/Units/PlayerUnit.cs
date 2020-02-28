using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerUnit : UnitBase
{
    public Vector3 movingDirection;

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


