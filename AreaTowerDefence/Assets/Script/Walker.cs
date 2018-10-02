using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Walker : Unit, iTouchBegin {

    [SerializeField]
    GameObject instantiatableArea;

    Transform target;
    NavMeshAgent agent;
	void Start () {
        agent = GetComponent<NavMeshAgent>();
        target = MainSceneManager.Instance.GetNearestOtherPlayerTower(PlayerNumber,transform.position);
	}

    public void TouchBegin(TouchInputManager.TouchInfo touchInfo)
    {
        if (touchInfo.PlayerNum != PlayerNumber)
        {
            print("自分が生成したオブジェクト以外がタッチされました");
            return;
        }
        InstantiateInstantiatableArea();
    }

    //AIからTouchBeginを介さずにアクセスさせるためpublic
    public void InstantiateInstantiatableArea()
    {
        const float OffsetY = 0.05f;//重なってちらつくのを防ぐため
        var pos = transform.position;
        pos.y = OffsetY;
        var instantiateObject = Instantiate(instantiatableArea, pos, instantiatableArea.transform.rotation);
        instantiateObject.GetComponent<Actor>().Initialize(PlayerNumber);
    }
	
	void Update () {
        agent.SetDestination(target.position);
    }
}
