using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Soldier : Unit {

    Transform target;
    NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = MainSceneManager.Instance.GetNearestOtherPlayerTower(PlayerNumber,transform.position);
    }

    void Update()
    {
        agent.SetDestination(target.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    void EnemyCheck()
    {

    }
}
