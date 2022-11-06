using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    public ChaseState ChaseState;
    public bool canSeeThePlayer;
    public override State ExecuteCurrentState()
    {
        if (!canSeeThePlayer)
        {
            return this;
        }
        else return ChaseState;
        
    }
}
