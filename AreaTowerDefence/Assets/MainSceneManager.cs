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
        if (!touchResult.WasHit) { return; }
        if (touchResult.HitInfo.collider.tag == "Stage")
        {
            target.position = touchResult.HitInfo.point;
        }
        if (touchResult.HitInfo.collider.tag == "Player"
            &&touchResult.TouchDown == true)
        {
            touchResult.HitInfo.collider.GetComponent<Walker>().Touch(touchResult);
        }
    }
}
