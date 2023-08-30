using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MissileBoss : MonoBehaviour
{
    public Transform target; //��������λ��
    private NavMeshAgent _navMeshAgent;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        _navMeshAgent.SetDestination(target.position);
    }
}
