using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Walker : MonoBehaviour {

    [SerializeField]
    Transform target;
    [SerializeField]
    GameObject instableArea;

    NavMeshAgent agent;
	void Start () {
        agent = GetComponent<NavMeshAgent>();
	}

    public void Touch()
    {
        Instantiate(instableArea, transform.parent);
        instableArea.transform.position = transform.position;
    }
	
	void Update () {
        agent.SetDestination(target.position);
    }
}
