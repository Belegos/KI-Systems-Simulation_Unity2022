using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateManager
{
    [System.Serializable]
    public class AttackState : State, IAttack
    {
        [SerializeField] private GameObject Enemy;
        [SerializeField] public bool AttackIsReady = true;
        public void Attack(StateManager Manager)
        {
            //Manager.Animator.SetTrigger("Attack");
            if (Vector3.Distance(Manager.CurrentEnemyEntity.transform.position, Manager.CurrentTaget.transform.position) <= 1)
            {
                if (AttackIsReady)
                {
                    AttackIsReady = false;
                    Manager.CoolDownTimer = 2f;
                    Debug.Log("Animation Attack");
                    Manager.Animator.Play("Attack", 0);
                }
            }
        }

        public override State ExecuteCurrentState(StateManager Manager)
        {
            Attack(Manager);
            return Manager.ChaseState;
        }
    }
}
