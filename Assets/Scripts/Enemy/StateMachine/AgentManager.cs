using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace StateManager
{
    public class AgentManager : MonoBehaviour
    {
        [SerializeField] private GameObject myTarget;
        [SerializeField] private GameObject currentTarget;
        public NavMeshAgent NavAgent;

        private int _range;
        private int _tetherRange;

        private Vector3 _startPosition;
        [SerializeField] private StateManager Manager;

        void Start()
        {
            NavAgent = GetComponent<NavMeshAgent>();
            InvokeRepeating("DistanceCheck", 0, 0.5f);
            _startPosition = this.transform.position;
        }

        public void Update()
        {
            TargetNullCheck(Manager);
            TellCurrentState(Manager);
        }

        public void TargetNullCheck(StateManager stateManager)
        {
            if (stateManager.CurrentState != null)
            {
                if (stateManager.CurrentState is ChaseState)
                {
                    Debug.Log($"Current State is {stateManager.CurrentState}");
                    NavAgent.destination = currentTarget.transform.position;
                }
                if (stateManager.CurrentState is IdleState)
                {
                    Debug.Log($"Current State is {stateManager.CurrentState}");
                    if(NavAgent.destination != _startPosition)
                    {
                        NavAgent.destination = _startPosition;
                    }
                }
                if (stateManager.CurrentState is AttackState)
                {
                    Debug.Log($"Current State is {stateManager.CurrentState}");
                }
            }
            else
            {
                Debug.Log($"ERROR! {stateManager.CurrentState} is null. ERROR! File:AGENTMANAGER.CS");
                return;
            }
        }

        public void TellCurrentState(StateManager Manager)
        {
            Debug.Log($"Current State is: {Manager.CurrentState}");
        }

        public void DistanceCheck()
        {
            float dist = Vector3.Distance(this.transform.position, myTarget.transform.position);
            if(dist < _range) 
            {
                currentTarget = myTarget;
            }
            else if(dist < _tetherRange)
            {
                currentTarget = null;
            }
        }
    }
}
