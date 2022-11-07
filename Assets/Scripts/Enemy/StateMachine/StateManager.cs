using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

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

        private State currentState;
        private IdleState idleState;
        private ChaseState chaseState;
        private AttackState attackState;
        private Animator animator;
        private NavMeshAgent NavAgent;

        [SerializeField] private bool _isInChaseRange;
        [SerializeField] private bool _isInAttackRange;
        [SerializeField] private GameObject myTarget;
        [SerializeField] private GameObject currentTarget;
        [SerializeField] private string _searchForTag = "Player";
        [SerializeField] private int _range = 3;
        [SerializeField] private int _tetherRange = 5;
        [SerializeField] private Vector3 _startPosition;
        private GameObject _selfTarget;
        private float velocity;
        private int VelocityHash;

        private void Start()
        {
            VelocityHash = Animator.StringToHash("Velocity");
            
            _startPosition = this.transform.position;
            myTarget = GameObject.FindGameObjectWithTag(_searchForTag);
            
            idleState = new IdleState();
            chaseState = new ChaseState();
            attackState = new AttackState();
            currentState = idleState;

            NavAgent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();
            InvokeRepeating("DistanceCheck", 0, 0.5f); //checks every 0.5 sec if the target is in range, if not sets target Destination back to starting point
        }

        void Update()
        {
            MovementAnimation();
            RunStateMachine();
            TargetNullCheck();
        }

        #region Methods
        private void MovementAnimation() //sets NavAgent.velocity and updates float for blendtree
        {
            if (NavAgent.velocity.x < 0)
            {
                velocity = NavAgent.velocity.x * -1;
            }
            else
            {
                velocity = NavAgent.velocity.x;
            }
            animator.SetFloat(VelocityHash, velocity);
        }
        private void RunStateMachine() //checks for current state and switchs if nessesary
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
        public void TargetNullCheck() //follows target, unless it is null, returns to start position
        {
            if (CurrentState != null)
            {
                if (CurrentState is ChaseState)
                {
                    Debug.Log($"Current State is {CurrentState}");
                    if (currentTarget != null)
                    {
                        NavAgent.destination = currentTarget.transform.position;
                    }
                    else if (NavAgent.destination != _startPosition)
                    {
                        NavAgent.destination = _startPosition;
                    }
                }
                if (CurrentState is IdleState)
                {
                    Debug.Log($"Current State is {CurrentState}");
                    if (NavAgent.destination != _startPosition)
                    {
                        NavAgent.destination = _startPosition;
                    }
                }
                if (CurrentState is AttackState)
                {
                    Debug.Log($"Current State is: {CurrentState}");
                }
            }
        }
        public void DistanceCheck() //checks if Target is in Range, if Target is out if range sets to null
        {
            float dist = Vector3.Distance(this.transform.position, myTarget.transform.position);
            if (dist < _range)
            {
                currentTarget = myTarget;
                IsInChaseRange = true;
            }
            else if (dist > _tetherRange)
            {
                currentTarget = null;
                IsInChaseRange = false;
            }
        }
        #endregion
    }
}