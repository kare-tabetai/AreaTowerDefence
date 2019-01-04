using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootRenderer : MonoBehaviour
{
    [SerializeField]
    LineRenderer lineRenderer;//GetCompoが面倒なので事前にアタッチ
    [SerializeField]
    Transform destination;

    /// <summary>
    /// Instantiateして呼び出す
    /// </summary>
    public void Initialize(Vector3 start,Vector3[] corners,Vector3 end)
    {
        lineRenderer.transform.position = start;
        lineRenderer.positionCount = corners.Length;
        for (int i = 0; i < corners.Length; i++)
        {
            var point = corners[i];
            lineRenderer.SetPosition(i, point);
        }
        var endPos = end;
        endPos.y += 0.1f;//床との重なり防止
        destination.position = endPos;
    }

    void Start ()
	{
		
	}

	void Update ()
	{
		
	}
}
