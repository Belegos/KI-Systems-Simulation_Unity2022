using System.ComponentModel;
using UnityEngine;
using UnityEngine.Serialization;

namespace StateManager
{
    [System.Serializable]
    public class AttackState : State, IAttack
    {
        [FormerlySerializedAs("Enemy")] [SerializeField] private GameObject enemy;
        [FormerlySerializedAs("AttackIsReady")] [SerializeField] public bool attackIsReady = true;
        public void Attack(StateManager manager)
        {
            if (Vector3.Distance(manager.CurrentEnemyEntity.transform.position, manager.CurrentTaget.transform.position) <= 1)
            {
                if (attackIsReady)
                {
                    attackIsReady = false;
                    manager.CoolDownTimer = 2f;
                    manager.Animator.Play("Attack", 0);
                    DamagaDoMethod();
                }
            }
        }
        public void DamagaDoMethod()
        {
            GameManager._gameManager.PlayerHealth.DamageUnit(10);
            GameManager._gameManager.UpdateHealthBar(GameManager._gameManager.PlayerHealth.Health);
        }

        public override State ExecuteCurrentState(StateManager manager)
        {
            Attack(manager);
            return manager.ChaseState;
        }
    }
}
