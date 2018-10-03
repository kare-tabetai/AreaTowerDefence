using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collisionが必要だけどTriggerで検知したい時に
/// 同じGameObjectに両方つけると気持ち悪いので
/// 子階層からMessageを飛ばすようにしたクラス
/// </summary>
public class ChildTrigger : MonoBehaviour {

    [SerializeField]
    MonoBehaviour[] parentMonoBehaviour;
    [SerializeField]
    string onTriggerEnterCallMethod = "OnTriggerEnter";
    [SerializeField]
    string onTriggerStayCallMethod = "OnTriggerStay";
    [SerializeField]
    string onTriggerExitCallMethod = "OnTriggerExit";

	void Start () {
        Debug.Assert(parentMonoBehaviour.Length != 0);
	}

    private void OnTriggerEnter(Collider other)
    {
        foreach(var monoBehaviour in parentMonoBehaviour)
        {
            monoBehaviour.SendMessage(onTriggerEnterCallMethod, other,SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        foreach (var monoBehaviour in parentMonoBehaviour)
        {
            monoBehaviour.SendMessage(onTriggerStayCallMethod, other, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        foreach (var monoBehaviour in parentMonoBehaviour)
        {
            monoBehaviour.SendMessage(onTriggerExitCallMethod, other, SendMessageOptions.DontRequireReceiver);
        }
    }
}
