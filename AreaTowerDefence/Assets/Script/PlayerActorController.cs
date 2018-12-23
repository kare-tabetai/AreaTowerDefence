using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActorController : ActorController {

    [SerializeField,Disable]
    UnitData draggingUnitData;//ドラッグして選択中のUnit

    GameObject draggingUnit;

    void Start () {
		
	}

    public void InstantiatableUnitButtonBeginDrag(UnitData instUnit)
    {
        Debug.Assert(draggingUnitData == null);
        Debug.Assert(draggingUnit == null);

        draggingUnitData = instUnit;
        draggingUnit = Instantiate(instUnit.InstantiateUnit);
        draggingUnit.GetComponent<Unit>().UnitStart();
    }

    void Update () {
        TouchInput();
    }

    void TouchInput()
    {
        var touchInfo = TouchInputManager.Instance.CurrentTouchInfo;
        

        if (!touchInfo.Touching)
        {
            draggingUnitData = null;
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

    /// <summary>
    /// 現在ドラッグ中のunitを召喚する
    /// </summary>
    /// <param name="instantiatePosition"></param>
    void InstantiateDraggedUnit(Vector3 instantiatePosition)
    {
        const float OffsetY = 0.05f;//重なりを防ぐため
        if (draggingUnitData == null) { return; }

        //予算が足りなければ帰る
        if (!costCounter.Pay(draggingUnitData.InstantiateCost))
        {
            print("予算不足で召喚できませんでした");
            return;
        }

        draggingUnit.transform.parent = MainSceneManager.Instance.ActorNode;
        var instantiatedObject = draggingUnit;
        var pos = instantiatePosition;
        pos.y = OffsetY;
        instantiatedObject.transform.position = pos;
        instantiatedObject.GetComponent<Unit>().Initialize(PlayerNumber);
        draggingUnit = null;
        draggingUnitData = null;
    }
}
