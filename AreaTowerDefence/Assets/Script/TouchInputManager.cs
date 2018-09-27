using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//UpdateをほかのUpdateよりも前に呼び出したいのでスクリプトの優先順を早くしている
[DefaultExecutionOrder(-10)]
public class TouchInputManager : MonoSingleton<TouchInputManager> {

    [SerializeField]
    bool isTouchInput;
    [SerializeField]
    LayerMask layerMask;
    [SerializeField]
    float rayLength = 20f;

    public TouchInfo CurrentTouchInfo { private set; get; }

    public struct TouchInfo
    {
        public bool Touched;//タッチされているか
        public bool ObjectHit;//タッチ位置からのレイがオブジェクトに当たったか
        public Touch Touch;
        public RaycastHit RayCastInfo;
    }

	void Start () {
		
	}

    void Update()
    {
        CurrentTouchInfo = TouchInfoUpdate();
    }

    TouchInfo TouchInfoUpdate()
    {
        if (isTouchInput)
        {
            return TouchRayCast();
        }
        else
        {
            return MouseRayCast();
        }
    }

    TouchInfo TouchRayCast()
    {
        var touchInfo = new TouchInfo();

        if (Input.touchCount <= 0)
        {
            touchInfo.Touched = false;
            return touchInfo;
        }
        var touch = Input.GetTouch(0);
        Vector3 touchPos = touch.position;
        RaycastHit hitInfo;

        touchInfo.ObjectHit = RayCast(touchPos,out hitInfo);
        touchInfo.RayCastInfo = hitInfo;
        touchInfo.Touch = touch;
        return touchInfo;
    }

    TouchInfo MouseRayCast()
    {
        Vector3 touchPos = Input.mousePosition;
        RaycastHit hitInfo;
        var touchInfo = new TouchInfo();
        touchInfo.ObjectHit = RayCast(touchPos, out hitInfo);
        touchInfo.RayCastInfo = hitInfo;
        if (Input.GetMouseButtonDown(0))
        {
            touchInfo.Touch = new Touch();
            touchInfo.Touch.phase = TouchPhase.Began;
            touchInfo.Touched = true;
            return touchInfo;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            touchInfo.Touch = new Touch();
            touchInfo.Touch.phase = TouchPhase.Ended;
            touchInfo.Touched = true;
            return touchInfo;
        }
        else if (Input.GetMouseButton(0))
        {
            touchInfo.Touch = new Touch();
            touchInfo.Touch.phase = TouchPhase.Moved;
            touchInfo.Touched = true;
            return touchInfo;
        }
        touchInfo.Touched = false;
        return touchInfo;
    }

    bool RayCast(Vector3 touchPos,out RaycastHit hitInfo)
    {
        Ray ray = Camera.main.ScreenPointToRay(touchPos);
        return Physics.Raycast(ray, out hitInfo, rayLength, layerMask);
    }
}
