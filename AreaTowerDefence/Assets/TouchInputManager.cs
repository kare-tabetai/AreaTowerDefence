using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInputManager : MonoSingleton<TouchInputManager> {

    [SerializeField]
    bool isTouchInput;
    [SerializeField]
    LayerMask layerMask;

    public struct RayCastResult
    {
        public bool wasHit;
        public RaycastHit hitInfo;
    }

	void Start () {
		
	}

    public RayCastResult GetTouchingWarldPosition()
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

    RayCastResult TouchRayCast()
    {
        var result = new RayCastResult();
        result.wasHit = false;

        if (Input.touchCount <= 0) { return result; }
        Vector3 touchPos = Input.GetTouch(0).position;
        RaycastHit hitInfo;
        result.wasHit = RayCast(touchPos,out hitInfo);
        result.hitInfo = hitInfo;
        return result;
    }

    RayCastResult MouseRayCast()
    {
        var result = new RayCastResult();
        result.wasHit = false;

        if (Input.GetMouseButton(0))
        {
            Vector3 touchPos = Input.mousePosition;
            RaycastHit hitInfo;
            result.wasHit = RayCast(touchPos,out hitInfo);
            result.hitInfo = hitInfo;
            return result;
        }
        return result;
    }

    bool RayCast(Vector3 touchPos,out RaycastHit hitInfo)
    {
        Ray ray = Camera.main.ScreenPointToRay(touchPos);
        return Physics.Raycast(ray, out hitInfo, 20f, layerMask);
    }
}
