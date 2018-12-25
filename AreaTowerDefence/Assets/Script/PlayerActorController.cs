using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActorController : ActorController {

    [SerializeField,Disable,Tooltip("ドラッグして選択中の召喚しようとしているUnit")]
    UnitData draggingInstUnitData;
    [Tooltip("ドラッグして選択中の召喚しようとしているUnitの実体")]
    GameObject draggingInstUnit;

    [Tooltip("ActorをDragしていればtrue")]
    public bool isActorDragging;
    public bool IsActorDragging { get { return isActorDragging; } }

    void Start () {
		
	}

    public void InstantiatableUnitButtonBeginDrag(UnitData instUnit)
    {
        Debug.Assert(draggingInstUnitData == null);
        Debug.Assert(draggingInstUnit == null);

        draggingInstUnitData = instUnit;
        draggingInstUnit = Instantiate(instUnit.InstantiateUnit);
        var unit = draggingInstUnit.GetComponent<Unit>();
        unit.UnitStart();
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
        var touchObject = touchInfo.RayCastInfo.collider.GetComponent<iTouchBegin>();//&&でしてもいいが毎回GetCompするのを防ぐため
        if (touchObject != null)
        {
            touchObject.TouchBegin(touchInfo);
        }

        //毎フレームGetComponentしたくないので
        var actor = touchInfo.RayCastInfo.collider.GetComponent<Actor>();
        if (actor != null)
        {
            isActorDragging = true;
        }
    }

    void TouchMoved(TouchInputManager.TouchInfo touchInfo)
    {
        if (!touchInfo.ObjectHit) { return; }//この下はレイの接触点があった場合
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
        }

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
        if (draggingInstUnitData == null) { return; }

        //予算が足りなければ帰る
        if (!costCounter.Pay(draggingInstUnitData.InstantiateCost))
        {
            print("予算不足で召喚できませんでした");
            return;
        }

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
