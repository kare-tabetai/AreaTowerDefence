using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Soldier : Actor {

    Transform target;
    NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = MainSceneManager.Instance.GetNearestOtherPlayerTower(PlayerNum,transform.position);
    }

    void Update()
    {
        agent.SetDestination(target.position);
    }
}
