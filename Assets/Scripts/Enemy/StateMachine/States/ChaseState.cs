using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{
    public AttackState AttackState;
    public IdleState idleState;
    [SerializeField] private bool _isInAttackRange;
    [SerializeField] private bool _isInChaseRange;

    public bool IsInChaseRange
    {
        get { return _isInChaseRange; }
        set { _isInChaseRange = value; }
    }


    public bool IsInAttackRange
    {
        get { return _isInAttackRange; }
        set { _isInAttackRange = value; }
    }

    public override State ExecuteCurrentState()
    {
        if (!_isInChaseRange)
        {
            return idleState;
        }
        if (_isInChaseRange && _isInAttackRange)
        {
            return AttackState;
        }
        if(!_isInAttackRange && !_isInChaseRange)
        {
            return idleState;
        }
        if (!_isInAttackRange && _isInChaseRange)
        {
            return this;
        }
        else //should never happen. Debug-Fallback
        {
            Debug.Log("Something went wrong in Statemanager. Returning to Idle");
            return idleState;
        }
    }
}
