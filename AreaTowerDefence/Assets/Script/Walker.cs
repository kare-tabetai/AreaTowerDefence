using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Walker : Actor, iTouchBegin {

    [SerializeField]
    GameObject instableArea;

    Transform target;
    NavMeshAgent agent;
	void Start () {
        agent = GetComponent<NavMeshAgent>();
        target = MainSceneManager.Instance.GetNearestOtherPlayerTower(PlayerNum,transform.position);
	}

    public void TouchBegin(TouchInputManager.TouchInfo touchInfo)
    {
        const float OffsetY = 0.05f;//重なってちらつくのを防ぐため
        var pos = touchInfo.RayCastInfo.point;
        pos.y = OffsetY;
        Instantiate(instableArea, pos,instableArea.transform.rotation);
    }
	
	void Update () {
        agent.SetDestination(target.position);
    }
}
