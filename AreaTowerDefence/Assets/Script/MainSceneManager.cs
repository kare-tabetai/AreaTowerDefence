using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneManager : MonoSingleton<MainSceneManager>
{
    [SerializeField]
    public Transform[] PlayerTower;
    [SerializeField]
    public Color[] PlayerColor;

    public GameObject DraggingActor;//ドラッグして選択中のActor

	void Start () {
        Debug.Assert(PlayerTower.Length == PlayerColor.Length);
	}

    public void InstantiatableActorButtonBeginDrag(GameObject instActor)
    {
        print("drag");
        DraggingActor = instActor;
    }
	
	void Update () {
        var touchInfo = TouchInputManager.Instance.CurrentTouchInfo;
        if (!touchInfo.Touched)
        {
            DraggingActor = null;
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

        if(touchInfo.Touch.phase == TouchPhase.Began)
        {
            var touchObject = touchInfo.RayCastInfo.collider.GetComponent<iTouchBegin>();
            if (touchObject != null)
            {
                touchObject.TouchBegin(touchInfo);
            }
        }
        if (touchInfo.Touch.phase == TouchPhase.Ended)
        {
            var touchObject = touchInfo.RayCastInfo.collider.GetComponent<iTouchEnd>();
            if (touchObject != null)
            {
                touchObject.TouchEnd(touchInfo);
            }
        }
    }

    public Transform GetNearestOtherPlayerTower(int playerNum,Vector3 comparedPosition)
    {
        Debug.Assert(PlayerTower.Length != 0);
        Transform nearestTower = null;
        float sqrDist = float.PositiveInfinity;
        for(int pNum = 0; pNum < PlayerTower.Length; pNum++)
        {
            if (pNum == playerNum) { continue; }
            float dist = Vector3.SqrMagnitude(PlayerTower[pNum].position - comparedPosition);
            if (dist < sqrDist)
            {
                sqrDist = dist;
                nearestTower = PlayerTower[pNum];
            }
        }
        Debug.Assert(nearestTower != null);
        return nearestTower;
    }
}
