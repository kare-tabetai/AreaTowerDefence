using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TouchObject{
    void Touch(TouchInputManager.RayCastResult rayCastResult);
}
