using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActorController : ActorController {

    [SerializeField, Disable,Tooltip("召喚されたunitの数")]
    int unitNum = 0;
    [SerializeField,Disable,Tooltip("ドラッグして選択中の召喚しようとしているUnit")]
    UnitData draggingInstUnitData;

    /// <summary>
    /// ドラッグして選択中の召喚しようとしているUnitの実体
    /// </summary>
    GameObject draggingInstUnit;

    /// <summary>
    /// 移動させるためなどにつまんでいる,生成,初期化済みunit
    /// </summary>
    Unit draggingIntializedUnit;

    [Tooltip("ActorをDragしていればtrue,カメラ移動でなんかつかんでいるかを見る用")]
    bool isActorDragging;
    public bool IsActorDragging { get { return isActorDragging; } }

    void Start () {
		
	}

    public void InstanteUnitButtonBeginDrag(UnitData instUnit)
    {
        Debug.Assert(draggingInstUnitData == null);
        Debug.Assert(draggingInstUnit == null);

        draggingInstUnitData = instUnit;
        draggingInstUnit = Instantiate(instUnit.InstantiateUnit);
        var unit = draggingInstUnit.GetComponent<Unit>();
        unit.Active();
        isActorDragging = true;
    }

    void Update () {
        TouchInput();
    }

    void TouchInput()
    {
        var touchInfo = TouchInputManager.Instance.CurrentTouchInfo;

        if (touchInfo.Touch.phase == TouchPhase.Began)
        {
            TouchBegin(touchInfo);
        }
        if (touchInfo.Touch.phase == TouchPhase.Moved)
        {
            TouchMoved(touchInfo);
        }
        if (touchInfo.Touch.phase == TouchPhase.Ended)
        {
            TouchEnded(touchInfo);
        }

    }

    void TouchBegin(TouchInputManager.TouchInfo touchInfo)
    {
        if (!touchInfo.ObjectHit) { return; }
        var touchObject = touchInfo.RayCastInfo.collider.GetComponent<iTouchBegin>();
        if (touchObject != null)
        {
            touchObject.TouchBegin(touchInfo);
        }

        var unit = touchInfo.RayCastInfo.collider.GetComponent<Unit>();
        if (unit != null&&unit.IsInitialized)
        {
            draggingIntializedUnit = unit;
            isActorDragging = true;
        }
    }

    void TouchMoved(TouchInputManager.TouchInfo touchInfo)
    {
        if (!touchInfo.ObjectHit) { return; }
        var touchObject = touchInfo.RayCastInfo.collider.GetComponent<iTouchMoved>();
        if (touchObject != null)
        {
            touchObject.TouchMoved(touchInfo);
        }

        if (draggingInstUnit != null)
        {
            draggingInstUnit.transform.position = touchInfo.RayCastInfo.point;
        }
        
    }

    void TouchEnded(TouchInputManager.TouchInfo touchInfo)
    {
        if (touchInfo.ObjectHit && touchInfo.RayCastInfo.collider.tag == "InstantiatableArea")
        {
            InstantiateDraggedUnit(touchInfo.RayCastInfo.point);
        }

        if (touchInfo.ObjectHit)
        {
            var touchObject = touchInfo.RayCastInfo.collider.GetComponent<iTouchEnd>();
            if (touchObject != null)
            {
                touchObject.TouchEnd(touchInfo);
            }

            if (draggingIntializedUnit != null)
            {
                //これより短い移動は
                const float DestinationMinLength = 0.5f;

                var destinationPos = touchInfo.RayCastInfo.point;
                Vector3 vec =
                    destinationPos - draggingIntializedUnit.transform.position;
                if (DestinationMinLength * DestinationMinLength <= vec.sqrMagnitude)
                {
                    var moveCommand = new UnitMoveCommand();
                    var unitInfo = draggingIntializedUnit.PackUnitInformation();
                    moveCommand.Initialize(unitInfo, destinationPos, true);
                    draggingIntializedUnit.CommandQueue.Enqueue(moveCommand);
                }
            }
        }

        draggingIntializedUnit = null;
        draggingInstUnitData = null;
        Destroy(draggingInstUnit);
        draggingInstUnit = null;
        isActorDragging = false;
    }

    /// <summary>
    /// 現在ドラッグ中のunitを召喚する
    /// </summary>
    /// <param name="instantiatePosition"></param>
    void InstantiateDraggedUnit(Vector3 instantiatePosition)
    {
        const float OffsetY = 0.05f;//重なりを防ぐため
        const int MaxUnitNum = 50;//unit数制限

        if (draggingInstUnitData == null) { return; }
        if (MaxUnitNum <= unitNum)
        {
            print("生成数が限界です");
            return;
        }

        //予算が足りなければ帰る
        if (!costCounter.Pay(draggingInstUnitData.InstantiateCost))
        {
            print("予算不足で召喚できませんでした");
            return;
        }

        unitNum++;
        draggingInstUnit.transform.parent = MainSceneManager.Instance.ActorNode;
        var instantiatedObject = draggingInstUnit;
        var pos = instantiatePosition;
        pos.y = OffsetY;
        instantiatedObject.transform.position = pos;
        instantiatedObject.GetComponent<Unit>().Initialize(PlayerNumber);
        draggingInstUnit = null;
        draggingInstUnitData = null;
    }
}
