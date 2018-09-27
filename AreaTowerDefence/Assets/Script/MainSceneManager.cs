using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InstantiatableActor
{
    Walker,

}

public class MainSceneManager : MonoSingleton<MainSceneManager>
{
    [SerializeField]
    public Transform target;
    [SerializeField]
    GameObject[] InstantiatableActors;

    public GameObject GetInstantiatableActor(InstantiatableActor instActor)
    {
        return InstantiatableActors[(int)instActor];
    }

	void Start () {
		
	}
	
	void Update () {
        var touchResult = TouchInputManager.Instance.GetTouchingWarldPosition();
        if (!touchResult.WasHit) { return; }
        if (touchResult.HitInfo.collider.tag == "Stage")
        {
            target.position = touchResult.HitInfo.point;
        }

        var touchObject = touchResult.HitInfo.collider.GetComponent<TouchObject>();
        if (touchObject != null
            &&touchResult.TouchDown == true)
        {
            touchObject.Touch(touchResult);
        }
    }
}
