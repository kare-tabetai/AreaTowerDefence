using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Walker : Actor, iTouchBegin {

    [SerializeField]
    GameObject instantiatableArea;

    Transform target;
    NavMeshAgent agent;
	void Start () {
        agent = GetComponent<NavMeshAgent>();
        target = MainSceneManager.Instance.GetNearestOtherPlayerTower(PlayerNum,transform.position);
	}

    public void TouchBegin(TouchInputManager.TouchInfo touchInfo)
    {
        InstantiateInstantiatableArea();
    }

    //AIからTouchBeginを介さずにアクセスさせるためpublic
    public void InstantiateInstantiatableArea()
    {
        const float OffsetY = 0.05f;//重なってちらつくのを防ぐため
        var pos = transform.position;
        pos.y = OffsetY;
        Instantiate(instantiatableArea, pos, instantiatableArea.transform.rotation);
    }
	
	void Update () {
        agent.SetDestination(target.position);
    }
}
