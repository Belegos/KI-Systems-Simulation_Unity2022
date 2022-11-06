using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace StateManager
{
    public class StateManager : MonoBehaviour
    {
        public IdleState IdleState { get => idleState; }
        public ChaseState ChaseState { get => chaseState; }
        public AttackState AttackState { get => attackState; }

        public State currentState;
        [SerializeField] private IdleState idleState;
        [SerializeField] private ChaseState chaseState;
        [SerializeField] private AttackState attackState;


        [SerializeField] private bool _isInChaseRange;
        [SerializeField] private bool _isInAttackRange;

        public bool IsInAttackRange
        {
            get { return _isInAttackRange; ; }
            set { _isInAttackRange = value; }
        }

        public bool IsInChaseRange
        {
            get { return _isInChaseRange; }
            set { _isInChaseRange = value; }
        }


        private void Start()
        {
            currentState = IdleState;
        }

        void Update()
        {
            RunStateMachine();
        }
        #region Methods
        private void RunStateMachine()
        {
            State nextState = currentState?.ExecuteCurrentState(this); //null checks, if not null run current state. If it is null ignore it

            if (currentState is null)
            {
                nextState = IdleState;
            }
            if (currentState == chaseState && IsInAttackRange && IsInChaseRange)
            {
                nextState = AttackState;
            }
            if (currentState == chaseState && !IsInAttackRange && IsInChaseRange)
            {
                nextState = chaseState;
            }
            if(currentState == chaseState && !IsInAttackRange && !IsInChaseRange)
            {
                nextState = IdleState;
            }

            SwitchToNextState(nextState);
        }
        private void SwitchToNextState(State nextState)
        {
            currentState = nextState;
        }
        #endregion
    }
}