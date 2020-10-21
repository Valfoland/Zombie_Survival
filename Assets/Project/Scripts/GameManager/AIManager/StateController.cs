using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateController : MonoBehaviour
{
    public BotManager BotManager;
    public State currentState;
    public EnemyStats enemyStats;
    public Transform eyes;
    public State remainState;
    public Transform chaseTarget;
    public bool IsDeath;
    public bool IsChase;

    public float TargetPos;
    public float LeftTime;

    [HideInInspector] public Vector3 NextWayPoint;
    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public Vector3 mainPosition;

    void Awake()
    {
        mainPosition = GetComponent<Transform>().position;
        navMeshAgent = GetComponent<NavMeshAgent>();
        NextWayPoint = GetComponent<Transform>().position;
        TargetPos = Random.Range(EnemyStats.POSITION_MIN, EnemyStats.POSITION_MAX);
    }


    private void Update()
    {
        if (!GameManager.isEndGame)
        {
            currentState.UpdateState(this);
        }
    }

    public void TransitionToState(State nextState)
    {
        if (nextState != remainState)
        {
            currentState = nextState;
        }
    }

    private void OnDrawGizmos()
    {
        if (currentState != null && eyes != null)
        {
            Gizmos.color = currentState.sceneGizmoColor;
            Gizmos.DrawWireSphere(eyes.position, enemyStats.LookOutRange);
        }
    }
}