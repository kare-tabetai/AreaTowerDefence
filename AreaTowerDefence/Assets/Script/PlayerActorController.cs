using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActorController : ActorController {

    public UnitData DraggingUnitData;//ドラッグして選択中のUnit,DebugようにHideInspectorしてない

    GameObject draggingUnit;

    void Start () {
		
	}

    public void InstantiatableUnitButtonBeginDrag(UnitData instUnit)
    {
        Debug.Assert(DraggingUnitData == null);
        Debug.Assert(draggingUnit == null);

        DraggingUnitData = instUnit;
        draggingUnit = Instantiate(instUnit.DraggingUnit);
    }

    void Update () {
        TouchInput();
    }

    void TouchInput()
    {
        var touchInfo = TouchInputManager.Instance.CurrentTouchInfo;
        if (!touchInfo.Touched)
        {
            DraggingUnitData = null;
            Destroy(draggingUnit);
            draggingUnit = null;
            return;
        }
        if (!touchInfo.ObjectHit)
        {
            return;
        }
        //ここまで来ていれば接触点がある
        if (draggingUnit != null)
        {
           draggingUnit.transform.position = touchInfo.RayCastInfo.point;
        }

        if (touchInfo.RayCastInfo.collider.tag == "Stage")
        {

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
        if (DraggingUnitData == null) { return; }

        //予算が足りなければ帰る
        if (!costCounter.Pay(DraggingUnitData.InstantiateCost))
        {
            return;
        }

        var instantiatedObject = Instantiate(DraggingUnitData.InstantiateUnit, MainSceneManager.Instance.ActorNode);
        var pos = instantiatePosition;
        pos.y = OffsetY;
        instantiatedObject.transform.position = pos;
    }
}
