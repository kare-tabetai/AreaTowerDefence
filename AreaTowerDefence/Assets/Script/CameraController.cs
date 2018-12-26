using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : Actor
{
    [SerializeField]
    float cameraVelocity=1;
    [SerializeField]
    bool xAxisLock;
    [SerializeField]
    bool zAxisLock;
    [SerializeField]
    public bool isStopped;
    [SerializeField]
    Vector2 areaCenter;
    [SerializeField]
    Vector2 areaSize;
    [SerializeField,Tooltip("移動制限のギズモのY")]
    float areaViewCenter;

    TouchInputManager touchInputManager;
    PlayerActorController playerActorController;
    void Start ()
	{
        touchInputManager = TouchInputManager.Instance;
        MainSceneManager.Instance.AssertPlayerController(PlayerNumber);
        playerActorController = (PlayerActorController)MainSceneManager.Instance.Controllers[PlayerNumber];
    }

    void OnDrawGizmos()
    {
        var center = areaCenter.XYtoXZ();
        center.y = areaViewCenter;
        Gizmos.DrawWireCube(center, areaSize.XYtoXZ());
    }

    //カメラ系はいろいろ処理してからのほうがいいのでLateUpdateにしている
    void LateUpdate ()
	{
        CameraMove();
    }

    void CameraMove()
    {
        if (isStopped) { return; }
        if (playerActorController.IsActorDragging) { return; }//Actorをドラックしていたら動かさない
        var deltaPos = touchInputManager.CurrentTouchInfo.Touch.deltaPosition;
        if (xAxisLock) { deltaPos.x = 0; }
        if (zAxisLock) { deltaPos.y = 0; }
        Vector2 deltaVec = deltaPos * cameraVelocity * Time.deltaTime;
        transform.Translate(-deltaVec.XYtoXZ(), Space.World);//x,y平面をx,z平面に変換
        var clampVec = transform.position;
        if (!xAxisLock)
        {
            clampVec.x =
            Mathf.Clamp(clampVec.x,
            areaCenter.x - areaSize.x / 2,
            areaCenter.x + areaSize.x / 2);
        }
        if (!zAxisLock)
        {
            clampVec.z = Mathf.Clamp(clampVec.z,
            areaCenter.y - areaSize.y / 2,
            areaCenter.y + areaSize.y / 2);
        }
        transform.position = clampVec;
    }
}
