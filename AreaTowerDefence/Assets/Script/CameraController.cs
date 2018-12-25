using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : Actor
{
    [SerializeField]
    float cameraVelocity=1;

    TouchInputManager touchInputManager;
    PlayerActorController playerActorController;
    void Start ()
	{
        touchInputManager = TouchInputManager.Instance;
        MainSceneManager.Instance.AssertPlayerController(PlayerNumber);
        playerActorController = (PlayerActorController)MainSceneManager.Instance.Controllers[PlayerNumber];
    }

    //カメラ系はいろいろ処理してからのほうがいいのでLateUpdateにしている
	void LateUpdate ()
	{
        CameraMove();
    }

    void CameraMove()
    {
        if (playerActorController.IsActorDragging) { return; }//Actorをドラックしていたら動かさない
        var deltaPos = touchInputManager.CurrentTouchInfo.Touch.deltaPosition;
        Vector2 vec = deltaPos * cameraVelocity * Time.deltaTime;
        transform.Translate(-vec.x, 0, -vec.y, Space.World);//x,y平面をx,z平面に変換
    }
}
