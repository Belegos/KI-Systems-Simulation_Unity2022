using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;

public class StateManager : MonoBehaviour
{
    #region .NET4_Singleton
    /// <summary>
    /// Pass a delegate to the constructor that calls the Singleton constructor.
    /// Allows to check wether or not the instance has been created with IsValueCreates property
    /// </summary>
    private StateManager()
    {
    }
    private static readonly Lazy<StateManager> lazy = new Lazy<StateManager>(() => new StateManager());
    public static StateManager Instance
    {
        get
        {
            return lazy.Value;
        }
    }
    #endregion

    public State currentState;
    [SerializeField] private IdleState idleState;
    [SerializeField] private ChaseState chaseState;
    [SerializeField] private AttackState attackState;
    [SerializeField] private GameObject Player;
    [SerializeField] private Vector3 Offset = new Vector3(2,2,0);
    NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        RunStateMachine();
    }
    #region Methods
    private void RunStateMachine()
    {
        State nextState = currentState?.ExecuteCurrentState(); //null checks, if not null run current state. If it is null ignore it

        if (nextState == null)
        {
            Debug.Log($"Current State: ''{currentState}'' is null");
            return;
        }if(currentState == chaseState)
        {
            agent.destination = Player.transform.position + Offset;
        }
        else
        {
            SwitchToNextState(nextState);
        }
    }
    private void SwitchToNextState(State nextState)
    {
        currentState = nextState;
    }
    #endregion
}
