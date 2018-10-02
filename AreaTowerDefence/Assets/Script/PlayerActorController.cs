using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActorController : ActorController {

    public GameObject DraggingActor;//ドラッグして選択中のActor,DebugようにHideInspectorしてない

    void Start () {
		
	}

    public void InstantiatableActorButtonBeginDrag(GameObject instActor)
    {
        DraggingActor = instActor;
    }

    void Update () {
        TouchInput();
    }

    void TouchInput()
    {
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
            //PlayerTower[0].position = touchInfo.RayCastInfo.point;
        }

        if (touchInfo.Touch.phase == TouchPhase.Began)
        {
            TouchBegin(touchInfo);
        }
        if (touchInfo.Touch.phase == TouchPhase.Ended)
        {
            TouchEnded(touchInfo);
        }
    }

    void TouchBegin(TouchInputManager.TouchInfo touchInfo)
    {
        var touchObject = touchInfo.RayCastInfo.collider.GetComponent<iTouchBegin>();//&&でしてもいいが毎回GetCompするのを防ぐため
        if (touchObject != null)
        {
            touchObject.TouchBegin(touchInfo);
        }
    }

    void TouchEnded(TouchInputManager.TouchInfo touchInfo)
    {
        if(touchInfo.RayCastInfo.collider.tag== "InstantiatableArea")
        {
            InstantiateDraggedActor(touchInfo.RayCastInfo.point);
        }
        var touchObject = touchInfo.RayCastInfo.collider.GetComponent<iTouchEnd>();
        if (touchObject != null)
        {
            touchObject.TouchEnd(touchInfo);
        }
    }

    void InstantiateDraggedActor(Vector3 instantiatePosition)
    {
        const float OffsetY = 0.05f;//重なりを防ぐため

        var instantiateActor = DraggingActor.GetComponent<Actor>();

        //予算が足りなければ帰る
        if (!costCounter.Pay(instantiateActor.InstantiateCost))
        {
            return;
        }

        var instantiatedObject = Instantiate(DraggingActor);
        var pos = instantiatePosition;
        pos.y = OffsetY;
        instantiatedObject.transform.position = pos;
    }
}
