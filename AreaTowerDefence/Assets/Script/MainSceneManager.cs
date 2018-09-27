using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneManager : MonoSingleton<MainSceneManager>
{
    [SerializeField]
    public Transform target;
    [SerializeField]
    GameObject[] InstantiatableActors;

    public GameObject DraggingActor;//ドラッグして選択中のActor

	void Start () {
		
	}

    public void InstantiatableActorButtonBeginDrag(GameObject instActor)
    {
        print("drag");
        DraggingActor = instActor;
    }
	
	void Update () {
        var touchInfo = TouchInputManager.Instance.CurrentTouchInfo;
        if (!touchInfo.Touched)
        {
            DraggingActor = null;
            return;
        }
        if (!touchInfo.ObjectHit)
        {
            return;
        }

        if (touchInfo.RayCastInfo.collider.tag == "Stage")
        {
            target.position = touchInfo.RayCastInfo.point;
        }

        if(touchInfo.Touch.phase == TouchPhase.Began)
        {
            var touchObject = touchInfo.RayCastInfo.collider.GetComponent<iTouchBegin>();
            if (touchObject != null)
            {
                touchObject.TouchBegin(touchInfo);
            }
        }
        if (touchInfo.Touch.phase == TouchPhase.Ended)
        {
            var touchObject = touchInfo.RayCastInfo.collider.GetComponent<iTouchEnd>();
            if (touchObject != null)
            {
                touchObject.TouchEnd(touchInfo);
            }
        }
    }
}
