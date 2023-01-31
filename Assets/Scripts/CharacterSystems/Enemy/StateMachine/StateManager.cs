using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.Serialization;

namespace StateManager
{
    public class StateManager : MonoBehaviour, ISearchForTag
    {
        #region props
        public IdleState IdleState { get => _idleState; }
        public ChaseState ChaseState { get => _chaseState; }
        public AttackState AttackState { get => _attackState; }
        public bool IsInAttackRange
        {
            get { return isInAttackRange; ; }
            set { isInAttackRange = value; }
        }

        public bool IsInChaseRange
        {
            get { return isInChaseRange; }
            set { isInChaseRange = value; }
        }
        public State CurrentState
        {
            get
            {
                return _currentState;
            }
            private set
            {
                _currentState = value;
            }
        }
        public Animator Animator
        {
            get { return _animator; }
            set { _animator = value; }
        }
        public GameObject CurrentTaget { get => currentTarget; }

        public GameObject CurrentEnemyEntity
        {
            get { return currentEnemyEntity; }
        }
        public float CoolDownTimer
        {
            get { return _coolDownTimer; }
            set { _coolDownTimer = value; }
        }
        public int AttackDamage {
            get { return attackDamage; }
            set { attackDamage = value; }
        }

        #endregion

        private State _currentState;
        private IdleState _idleState;
        private ChaseState _chaseState;
        private AttackState _attackState;
        private Animator _animator;
        private NavMeshAgent _navAgent;

        [FormerlySerializedAs("_isInChaseRange")] [SerializeField] private bool isInChaseRange;
        [FormerlySerializedAs("_isInAttackRange")] [SerializeField] private bool isInAttackRange;
        [SerializeField] private GameObject myTarget;
        [SerializeField] private GameObject currentTarget;
        [FormerlySerializedAs("_currentEnemyEntity")] [SerializeField] private GameObject currentEnemyEntity;

        [FormerlySerializedAs("_searchForTag")] [SerializeField] private string searchForTag = "Player";
        [FormerlySerializedAs("_range")] [SerializeField] private int range = 3;
        [FormerlySerializedAs("_tetherRange")] [SerializeField] private int tetherRange = 5;
        [FormerlySerializedAs("_attackRange")] [SerializeField] private int attackRange = 1;
        [FormerlySerializedAs("_attackDamage")] [SerializeField] private int attackDamage = 1;
        [FormerlySerializedAs("_startPosition")] [SerializeField] private Vector3 startPosition;
        private float _velocity; //parameter for animator can't be written with an underscore, otherwise it won't work
        private int _velocityHash;
        private float _coolDownTimer;
 
        private void Start()
        {
            _velocityHash = Animator.StringToHash("velocity");

            startPosition = this.transform.position;
            myTarget = GameObject.FindGameObjectWithTag(searchForTag);

            _idleState = new IdleState();
            _chaseState = new ChaseState();
            _attackState = new AttackState();
            _currentState = _idleState;

            _navAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();
            InvokeRepeating("DistanceCheck", 0, 0.5f); //checks every 0.5 sec if the target is in range, if not sets target Destination back to starting point
            InvokeRepeating("CheckDistanceBetweenEnemyAndPlayer", 0, 0.5f); //checks every 0.5 sec if the target is in _attackRange, if true sets bool IsInAttackRange, for attack state switch

        }

        void Update()
        {
            MovementAnimation();
            RunStateMachine();
            TargetNullCheck();

            if (CoolDownTimer >= 0)
            {
                CoolDownTimer -= Time.deltaTime;
            }
            else
            {
                _attackState.attackIsReady = true;
            }
        }
        #region Methods

        private void CheckDistanceBetweenEnemyAndPlayer()
        {
            if (Vector3.Distance(myTarget.transform.position, this.transform.position) < attackRange)
            {
                IsInAttackRange = true;
            }
            else
            {
                IsInAttackRange = false;
            }
        }


        private void MovementAnimationOld() //sets NavAgent.velocity and updates float for blendtree
        {
            if (_navAgent.velocity.x < 0)
            {
                _velocity = _navAgent.velocity.x * -1;
            }
            else
            {
                _velocity = _navAgent.velocity.x;
            }
            _animator.SetFloat(_velocityHash, _velocity);
        }
        private void RunStateMachine() //checks for current state and switchs if nessesary
        {
            State nextState = _currentState?.ExecuteCurrentState(this); //null checks, if not null run current state. If it is null ignore it

            if (_currentState is null)
            {
                nextState = IdleState;
            }
            if (_currentState == _chaseState && IsInAttackRange && IsInChaseRange)
            {
                nextState = AttackState;
            }
            if (_currentState == _chaseState && !IsInAttackRange && IsInChaseRange)
            {
                nextState = _chaseState;
            }
            if (_currentState == _chaseState && !IsInAttackRange && !IsInChaseRange)
            {
                nextState = IdleState;
            }

            SwitchToNextState(nextState);
        }
        private void SwitchToNextState(State nextState)
        {
            _currentState = nextState;
        }
        public void TargetNullCheck() //follows target, unless it is null, returns to start position
        {
            if (CurrentState != null)
            {
                if (CurrentState is ChaseState)
                {
                    if (currentTarget != null)
                    {
                        _navAgent.destination = currentTarget.transform.position;
                    }
                    else if (_navAgent.destination != startPosition)
                    {
                        _navAgent.destination = startPosition;
                    }
                }
                if (CurrentState is IdleState)
                {
                    if (_navAgent.destination != startPosition)
                    {
                        _navAgent.destination = startPosition;
                    }
                }
            }
        }
        public void DistanceCheck() //checks if Target is in Range, if Target is out if range sets to null
        {
            float dist = Vector3.Distance(this.transform.position, myTarget.transform.position);
            if (dist < range)
            {
                currentTarget = myTarget;
                IsInChaseRange = true;
            }
            else if (dist > tetherRange)
            {
                currentTarget = null;
                IsInChaseRange = false;
            }
        }

        private void MovementAnimation() //sets NavAgent.velocity and updates float for blendtree animations
        {
            _velocity = _navAgent.velocity.magnitude / _navAgent.speed;
            _animator.SetFloat(_velocityHash, _velocity);
        }
        #endregion
    }
}