using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Walker : Unit, iTouchEnd
{

    [SerializeField]
    GameObject instantiatePrefab;

    GameObject instantiateArea;
	void Start () {
    }

    void Update()
    {
        if (!isInitialized) { return; }
    }

    public void TouchEnd(TouchInputManager.TouchInfo touchInfo)
    {
        if (!isInitialized) { return; }

        if (touchInfo.PlayerNum != PlayerNumber)
        {
            print("自分が生成したオブジェクト以外がタッチされました");
            return;
        }
        if (instantiateArea != null)
        {
            print("instaatiateAreaが生成されています");
            return;
        }
        agent.isStopped = true;
        animator.SetFloat("Velocity", 0);
        InstantiateArea();
    }

    //AIからTouchBeginを介さずにアクセスさせるためpublic
    void InstantiateArea()
    {
        const float OffsetY = 0.05f;//重なってちらつくのを防ぐため
        var pos = transform.position;
        pos.y += OffsetY;
        instantiateArea = Instantiate(instantiatePrefab, pos, instantiatePrefab.transform.rotation);
        instantiateArea.GetComponent<Actor>().Initialize(PlayerNumber);
    }
}
