using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatableArea : MonoBehaviour,TouchObject {

	void Start () {
		
	}

    public void Touch(TouchInputManager.RayCastResult rayCastResult)
    {
        const float OffsetY = 0.05f;//重なってちらつくのを防ぐため

        var instantiatedObject = Instantiate(MainSceneManager.Instance.GetInstantiatableActor(InstantiatableActor.Walker));
        var pos = rayCastResult.HitInfo.point;
        pos.y = OffsetY;
        instantiatedObject.transform.position = pos;
    }

    void Update () {
		
	}
}
