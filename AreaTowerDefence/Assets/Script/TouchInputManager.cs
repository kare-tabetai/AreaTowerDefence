using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInputManager : MonoSingleton<TouchInputManager> {

    [SerializeField]
    bool isTouchInput;
    [SerializeField]
    LayerMask layerMask;
    [SerializeField]
    float rayLength = 20f;

    bool touched;

    public struct RayCastResult
    {
        public bool WasHit;
        public bool TouchDown;
        public RaycastHit HitInfo;
    }

	void Start () {
		
	}

    void LateUpdate()
    {
        if (isTouchInput)
        {
            if (0 < Input.touchCount)
            {
                touched = true;
            }
            else
            {
                touched = false;
            }
        }
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
        result.WasHit = false;
        result.TouchDown = false;

        if (Input.touchCount <= 0) { return result; }
        Vector3 touchPos = Input.GetTouch(0).position;
        RaycastHit hitInfo;
        result.WasHit = RayCast(touchPos,out hitInfo);
        result.HitInfo = hitInfo;
        if (!touched)
        {
            result.TouchDown = true;
        }
        return result;
    }

    RayCastResult MouseRayCast()
    {
        var result = new RayCastResult();
        result.WasHit = false;
        result.TouchDown = false;

        if (Input.GetMouseButton(0))
        {
            Vector3 touchPos = Input.mousePosition;
            RaycastHit hitInfo;
            result.WasHit = RayCast(touchPos,out hitInfo);
            result.HitInfo = hitInfo;
            if (Input.GetMouseButtonDown(0))
            {
                result.TouchDown = true;
            }
            return result;
        }
        return result;
    }

    bool RayCast(Vector3 touchPos,out RaycastHit hitInfo)
    {
        Ray ray = Camera.main.ScreenPointToRay(touchPos);
        return Physics.Raycast(ray, out hitInfo, rayLength, layerMask);
    }
}
