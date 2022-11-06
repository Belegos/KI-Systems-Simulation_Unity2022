using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace StateManager
{
    public class AgentManager : MonoBehaviour
    {
        private GameObject myTarget;
        private GameObject currentTarget;
        public NavMeshAgent NavAgent;

        private int _range;
        private int _tetherRange;

        private Vector3 _startPosition;
        private StateManager Manager;

        void Start()
        {
            NavAgent = GetComponent<NavMeshAgent>();
            Manager = GetComponent<StateManager>();
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
            if (stateManager.currentState != null)
            {
                if (stateManager.currentState is ChaseState)
                {
                    Debug.Log($"Current State is {stateManager.currentState}");
                    NavAgent.destination = currentTarget.transform.position;
                }
                if (stateManager.currentState is IdleState)
                {
                    Debug.Log($"Current State is {stateManager.currentState}");
                    if(NavAgent.destination != _startPosition)
                    {
                        NavAgent.destination = _startPosition;
                    }
                }
                if (stateManager.currentState is AttackState)
                {
                    Debug.Log($"Current State is {stateManager.currentState}");
                }
            }
            else
            {
                Debug.Log($"ERROR! {stateManager.currentState} is null. ERROR! File:AGENTMANAGER.CS");
                return;
            }
        }

        public void TellCurrentState(StateManager Manager)
        {
            Debug.Log($"Current State is: {Manager.currentState}");
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
