using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMoveCommand : iUnitCommand
{
    /// <summary>
    /// この移動を示す矢印スプライトあれば終了時破棄する
    /// </summary>
    SpriteRenderer allowSprite;

    public void Initialize(UnitInformation unitInfo, Vector3 movePosition, SpriteRenderer allowSprite = null)
    {
        if (!unitInfo.Movable) { return; }

        unitInfo.Animator.SetFloat("Velocity", unitInfo.Agent.speed);
        unitInfo.Agent.isStopped = false;
        unitInfo.Agent.SetDestination(movePosition);
        this.allowSprite = allowSprite;
    }
    public void UpdateUnitInstruction(UnitInformation unitInfo)
    {
        const float GoalDistance = 0.7f;

        if (unitInfo.Movable)
        {
            unitInfo.Animator.SetFloat("Velocity", unitInfo.Agent.speed);
            unitInfo.Agent.isStopped = false;
        }
        else
        {
            unitInfo.Animator.SetFloat("Velocity", 0);
            unitInfo.Agent.isStopped = true;
        }

        var distance = unitInfo.Agent.destination - unitInfo.Unit.transform.position;
        var sqeDist = distance.sqrMagnitude;
        //目的地到達
        if (sqeDist < GoalDistance * GoalDistance)
        {
            Finalize(unitInfo);
            unitInfo.InstrucitonQueue.Dequeue();
            return;
        }
        
    }
    public void Finalize(UnitInformation unitInfo)
    {
        if (allowSprite != null) { GameObject.Destroy(allowSprite.gameObject); }
    }
}
