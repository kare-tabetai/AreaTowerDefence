using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全シーン汎用的に使えるものをここに，それ以外はActorControllerや各SceneManagerに
/// Editorとスマホで両対応してたが，エディタ用のほうでもスマホ対応できてた
/// UpdateをほかのUpdateよりも前に呼び出したいのでスクリプトの優先順を早くしている
/// </summary>
[DefaultExecutionOrder(-10)]
public class TouchInputManager : MonoSingleton<TouchInputManager> {

    [SerializeField]
    bool isTouchInput;//スマホならtrue
    [SerializeField]
    LayerMask layerMask;
    [SerializeField]
    float rayLength = 20f;
    [SerializeField,Tooltip("このInputをしているPlayerの番号")]
    int playerNum;
    [SerializeField]
    TouchInfo currentTouchInfo;
    public TouchInfo CurrentTouchInfo { get { return currentTouchInfo; } }

    [System.Serializable]
    public struct TouchInfo
    {
        [Tooltip("タッチされているか")]
        public bool Touching;
        [Tooltip("Window座標でのタッチされている位置")]
        public Vector2 TouchWindowPos;
        [Tooltip("タッチ位置からのレイがオブジェクトに当たったか")]
        public bool ObjectHit;//RayCastInfoのcollderでも調べられるが
        public Touch Touch;
        public RaycastHit RayCastInfo;
        [Tooltip("TouchIndoごと送ったほうが効率的なのでこっちにもPlayerNumberを含めている,TouchInfoは毎回再初期化されるので再代入している")]
        public int PlayerNum;
    }

	void Start () {
        MainSceneManager.Instance.AssertPlayerController(playerNum);

	}

    void Update()
    {
        currentTouchInfo = TouchInfoUpdate();
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
            touchInfo.Touching = false;
            return touchInfo;
        }else
        {
            touchInfo.Touching = true;
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
        Vector2 touchPos = Input.mousePosition;
        RaycastHit hitInfo;
        var touchInfo = new TouchInfo();
        touchInfo.ObjectHit = RayCast(touchPos, out hitInfo);
        touchInfo.RayCastInfo = hitInfo;
        if (Input.GetMouseButtonDown(0))
        {
            //touch側と処理を共通化したいのでマウス入力をTouch構造体に置き換えてる
            touchInfo.Touch = new Touch();
            touchInfo.Touch.phase = TouchPhase.Began;
            touchInfo.Touching = true;
            touchInfo.PlayerNum = playerNum;
            touchInfo.TouchWindowPos = touchPos;
            
            return touchInfo;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            touchInfo.Touch = new Touch();
            touchInfo.Touch.phase = TouchPhase.Ended;
            touchInfo.Touching = true;
            touchInfo.PlayerNum = playerNum;
            
            return touchInfo;
        }
        else if (Input.GetMouseButton(0))
        {
            touchInfo.Touch = new Touch();
            touchInfo.Touch.phase = TouchPhase.Moved;
            touchInfo.Touching = true;
            touchInfo.PlayerNum = playerNum;
            touchInfo.Touch.deltaPosition = touchPos - currentTouchInfo.TouchWindowPos;
            touchInfo.TouchWindowPos = touchPos;
            return touchInfo;
        }
        touchInfo.Touching = false;
        return touchInfo;
    }

    bool RayCast(Vector3 touchPos,out RaycastHit hitInfo)
    {
        Ray ray = Camera.main.ScreenPointToRay(touchPos);
        return Physics.Raycast(ray, out hitInfo, rayLength, layerMask);
    }
}
