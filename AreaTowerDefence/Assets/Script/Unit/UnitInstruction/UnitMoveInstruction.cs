using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMoveInstruction : iUnitInstruction
{
    /// <summary>
    /// この移動を示す矢印スプライトあれば終了時破棄する
    /// </summary>
    SpriteRenderer allowSprite;

    public void Initialize(UnitInformation unitInfo, Vector3 movePosition, SpriteRenderer allowSprite = null)
    {
        unitInfo.Animator.SetFloat("Velocity", unitInfo.Agent.speed);
        unitInfo.Agent.isStopped = false;
        unitInfo.Agent.SetDestination(movePosition);
        this.allowSprite = allowSprite;
    }
    public void UpdateUnitInstruction(UnitInformation unitInfo)
    {
        const float GoalDistance = 0.7f;

        var distance = unitInfo.Agent.destination - unitInfo.Unit.transform.position;
        var sqeDist = distance.sqrMagnitude;
        if (sqeDist < GoalDistance * GoalDistance)
        {
            Finalize(unitInfo);
            unitInfo.InstrucitonQueue.Dequeue();
            return;
        }
        bool b = TouchInputManager.Instance
            .CompareToSelfTouchPhase(unitInfo.Unit.gameObject, TouchPhase.Ended);
        if (b)
        {
            unitInfo.ReleaseQueue();
            var newInstruction = new UnitStopInstruction();
            newInstruction.Initialize(unitInfo);
            unitInfo.InstrucitonQueue.Enqueue(newInstruction);
        }
    }
    public void Finalize(UnitInformation unitInfo)
    {
        if (allowSprite != null) { GameObject.Destroy(allowSprite.gameObject); }
    }
}
