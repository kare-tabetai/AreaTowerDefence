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
    [SerializeField,Tooltip("このInputをしているPlayerの番号")]
    int playerNum;

    public TouchInfo CurrentTouchInfo { private set; get; }

    public struct TouchInfo
    {
        public bool Touched;//タッチされているか
        public bool ObjectHit;//タッチ位置からのレイがオブジェクトに当たったか
        public Touch Touch;
        public RaycastHit RayCastInfo;
        public int PlayerNum;
    }

	void Start () {
        //playerNumがPlayerであることを表明
        Debug.Assert(
            MainSceneManager.Instance
            .Controllers[playerNum]
            .GetComponent<PlayerActorController>()
            );
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
        touchInfo.PlayerNum = playerNum;
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
            touchInfo.PlayerNum = playerNum;
            return touchInfo;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            touchInfo.Touch = new Touch();
            touchInfo.Touch.phase = TouchPhase.Ended;
            touchInfo.Touched = true;
            touchInfo.PlayerNum = playerNum;
            return touchInfo;
        }
        else if (Input.GetMouseButton(0))
        {
            touchInfo.Touch = new Touch();
            touchInfo.Touch.phase = TouchPhase.Moved;
            touchInfo.Touched = true;
            touchInfo.PlayerNum = playerNum;
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
