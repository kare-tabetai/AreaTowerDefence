using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneManager : MonoSingleton<MainSceneManager>
{
    [SerializeField]
    Transform target;

	void Start () {
		
	}
	
	void Update () {
        var touchResult = TouchInputManager.Instance.GetTouchingWarldPosition();
        if (!touchResult.wasHit) { return; }
        if (touchResult.hitInfo.collider.tag == "Stage")
        {
            target.position = touchResult.hitInfo.point;
        }
        if (touchResult.hitInfo.collider.tag == "Players")
        {
            touchResult.hitInfo.collider.GetComponent<Walker>().Touch();
        }
    }
}
