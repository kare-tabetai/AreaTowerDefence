using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActorController : ActorController {

    public GameObject DraggingUnit;//ドラッグして選択中のUnit,DebugようにHideInspectorしてない

    void Start () {
		
	}

    public void InstantiatableUnitButtonBeginDrag(GameObject instUnit)
    {
        DraggingUnit = instUnit;
    }

    void Update () {
        TouchInput();
    }

    void TouchInput()
    {
        var touchInfo = TouchInputManager.Instance.CurrentTouchInfo;
        if (!touchInfo.Touched)
        {
            DraggingUnit = null;
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
            InstantiateDraggedUnit(touchInfo.RayCastInfo.point);
        }
        var touchObject = touchInfo.RayCastInfo.collider.GetComponent<iTouchEnd>();
        if (touchObject != null)
        {
            touchObject.TouchEnd(touchInfo);
        }
    }

    void InstantiateDraggedUnit(Vector3 instantiatePosition)
    {
        const float OffsetY = 0.05f;//重なりを防ぐため
        if (DraggingUnit == null) { return; }
        var instantiateUnit = DraggingUnit.GetComponent<Unit>();

        //予算が足りなければ帰る
        if (!costCounter.Pay(instantiateUnit.InstantiateCost))
        {
            return;
        }

        var instantiatedObject = Instantiate(DraggingUnit,MainSceneManager.Instance.ActorNode);
        var pos = instantiatePosition;
        pos.y = OffsetY;
        instantiatedObject.transform.position = pos;
    }
}
