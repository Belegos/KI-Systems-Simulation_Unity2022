using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateManager
{
    public class AttackState : State
    {
        public override State ExecuteCurrentState(StateManager Manager)
        {
            Debug.Log("Player has been attacked");
            //return this;

            #region Debug
            Manager.IsInChaseRange = false;
            Manager.IsInAttackRange = false;
            #endregion
            return Manager.IdleState;
        }

    }
}
