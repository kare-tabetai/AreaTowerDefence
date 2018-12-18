using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Walker : Unit, iTouchBegin {

    [SerializeField]
    GameObject instantiatableAreaPrefab;

    GameObject instableArea;
    Transform target;
    NavMeshAgent agent;
    Animator animator;
	void Start () {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        target = MainSceneManager.Instance.GetNearestOtherPlayerTower(PlayerNumber,transform.position);
        agent.SetDestination(target.position);
        animator.SetFloat("Velocity", agent.speed);
    }

    public void TouchBegin(TouchInputManager.TouchInfo touchInfo)
    {
        if (touchInfo.PlayerNum != PlayerNumber)
        {
            print("自分が生成したオブジェクト以外がタッチされました");
            return;
        }
        if (instableArea != null)
        {
            print("instavleAreaが生成されています");
        }
        agent.isStopped = true;
        animator.SetFloat("Velocity", 0);
        InstantiateInstantiatableArea();
    }

    //AIからTouchBeginを介さずにアクセスさせるためpublic
    public void InstantiateInstantiatableArea()
    {
        const float OffsetY = 0.05f;//重なってちらつくのを防ぐため
        var pos = transform.position;
        pos.y = OffsetY;
        instableArea = Instantiate(instantiatableAreaPrefab, pos, instantiatableAreaPrefab.transform.rotation);
        instableArea.GetComponent<Actor>().Initialize(PlayerNumber);
    }
}
