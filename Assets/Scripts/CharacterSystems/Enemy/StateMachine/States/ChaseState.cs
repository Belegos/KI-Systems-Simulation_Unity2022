using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateManager
{
    [System.Serializable]
    public class ChaseState : State
    {
        public override State ExecuteCurrentState(StateManager Manager)
        {

            if (Manager.IsInChaseRange && Manager.IsInAttackRange)//when player is in chase range and in attack range, switch to attack state
            {
                return Manager.AttackState;
            }
            if (!Manager.IsInChaseRange) //when player is not in sightrange, return to idle
            {
                return Manager.IdleState;
            }
            if (!Manager.IsInAttackRange && !Manager.IsInChaseRange) //when player is not in sight and in attack range, return to idle
            {
                return Manager.IdleState;
            }
            if (!Manager.IsInAttackRange && Manager.IsInChaseRange)
            {
                return this;
            }
            else //should never happen. Debug-Fallback
            {
                Debug.Log("Something went wrong in Statemanager. Returning to Idle");
                return Manager.IdleState;
            }
        }
    }
}

