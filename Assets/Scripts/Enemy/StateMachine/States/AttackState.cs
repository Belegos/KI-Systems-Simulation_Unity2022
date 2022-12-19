using System.ComponentModel;
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
            if (Vector3.Distance(Manager.CurrentEnemyEntity.transform.position, Manager.CurrentTaget.transform.position) <= 1)
            {
                if (AttackIsReady)
                {
                    AttackIsReady = false;
                    Manager.CoolDownTimer = 2f;
                    Debug.Log("Animation Attack");
                    Manager.Animator.Play("Attack", 0);
                    DamagaDoMethod();
                }
            }
        }
        public void DamagaDoMethod()
        {
            GameManager._gameManager._playerHealth.DamageUnit(10);
            GameManager._gameManager.UpdateHealthBar(GameManager._gameManager._playerHealth.Health);
            Debug.Log(GameManager._gameManager._playerHealth.Health);
        }

        public override State ExecuteCurrentState(StateManager Manager)
        {
            Attack(Manager);
            return Manager.ChaseState;
        }
    }
}
