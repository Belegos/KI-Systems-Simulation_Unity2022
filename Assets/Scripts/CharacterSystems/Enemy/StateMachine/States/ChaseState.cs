using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateManager
{
    [System.Serializable]
    public class ChaseState : State
    {
        public override State ExecuteCurrentState(StateManager manager)
        {

            if (manager.IsInChaseRange && manager.IsInAttackRange)//when player is in chase range and in attack range, switch to attack state
            {
                return manager.AttackState;
            }
            if (!manager.IsInChaseRange) //when player is not in sightrange, return to idle
            {
                return manager.IdleState;
            }
            if (!manager.IsInAttackRange && !manager.IsInChaseRange) //when player is not in sight and in attack range, return to idle
            {
                return manager.IdleState;
            }
            if (!manager.IsInAttackRange && manager.IsInChaseRange)
            {
                return this;
            }
            else //should never happen.
            {
                return manager.IdleState;
            }
        }
    }
}

