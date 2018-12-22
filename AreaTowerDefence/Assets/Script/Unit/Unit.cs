using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 動く駒を作るときはこれを継承
/// </summary>
public abstract class Unit : HpActor {

    [SerializeField]
    protected TextMeshPro debugText;

    protected void UnitStart()
    {
        debugText = GetComponentInChildren<TextMeshPro>();
    }

    protected static float GetNearestUnit(Unit self,List<Unit> unitList,out Unit nearestUnit)
    {
        Debug.Assert(unitList.Count != 0);

        float nearestSqrDist = Mathf.Infinity;
        nearestUnit = null;
        foreach (var hpActor in unitList)
        {
            Debug.Assert(hpActor.PlayerNumber != self.PlayerNumber);
            float sqrDist = Vector3.SqrMagnitude(hpActor.transform.position - self.transform.position);
            if (sqrDist < nearestSqrDist)
            {
                nearestSqrDist = sqrDist;
                nearestUnit = hpActor;
            }
        }
        Debug.Assert(nearestUnit != null);
        Debug.Assert(nearestUnit.gameObject != null);
        Debug.Assert(nearestSqrDist != Mathf.Infinity);
        return nearestSqrDist;
    }
}
