using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateManager
{
    public class IdleState : State
    {
        public override State ExecuteCurrentState(StateManager Manager)
        {
            if (!Manager.IsInChaseRange)
            {
                return this;
            }
            else return Manager.ChaseState;

        }
    }
}
