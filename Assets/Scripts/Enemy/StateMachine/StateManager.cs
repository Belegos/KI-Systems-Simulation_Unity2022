using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace StateManager
{
    public class StateManager : MonoBehaviour
    {
        #region props
        public IdleState IdleState { get => idleState; }
        public ChaseState ChaseState { get => chaseState; }
        public AttackState AttackState { get => attackState; }
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
        public State CurrentState
        {
            get
            {
                return currentState;
            }
            private set
            {
                currentState = value;
            }
        }
        #endregion

        [SerializeField] private State currentState;
        [SerializeField] private IdleState idleState;
        [SerializeField] private ChaseState chaseState;
        [SerializeField] private AttackState attackState;

        [SerializeField] private bool _isInChaseRange;
        [SerializeField] private bool _isInAttackRange;

        private void Start()
        {
            idleState = new IdleState();
            chaseState = new ChaseState();
            attackState = new AttackState();
            currentState = idleState;
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
            if (currentState == chaseState && !IsInAttackRange && !IsInChaseRange)
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