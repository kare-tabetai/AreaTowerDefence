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

    public void Touch(TouchInputManager.RayCastResult rayCastResult)
    {
        const float OffsetY = 0.05f;//重なってちらつくのを防ぐため
        var pos = rayCastResult.HitInfo.point;
        pos.y = OffsetY;
        Instantiate(instableArea, pos,instableArea.transform.rotation);
    }
	
	void Update () {
        agent.SetDestination(target.position);
    }
}
