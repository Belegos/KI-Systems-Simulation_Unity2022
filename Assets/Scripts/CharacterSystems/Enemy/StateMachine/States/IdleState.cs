using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateManager
{
    [System.Serializable]
    public class IdleState : State
    {
        public override State ExecuteCurrentState(StateManager manager)
        {
            if (!manager.IsInChaseRange)
            {
                return this;
            }
            else return manager.ChaseState;

        }
    }
}
