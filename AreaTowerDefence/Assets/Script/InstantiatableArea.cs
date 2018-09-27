using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatableArea : Actor,iTouchEnd {

	void Start ()
    {
		
	}

    public void TouchEnd(TouchInputManager.TouchInfo touchInfo)
    {
        const float OffsetY = 0.05f;//重なりを防ぐため

        var instantiatedObject = Instantiate(MainSceneManager.Instance.DraggingActor);
        var pos = touchInfo.RayCastInfo.point;
        pos.y = OffsetY;
        instantiatedObject.transform.position = pos;
    }

    void Update () {
		
	}
}
