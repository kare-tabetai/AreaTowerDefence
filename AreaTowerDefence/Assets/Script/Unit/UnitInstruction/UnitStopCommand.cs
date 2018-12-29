using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStopCommand : iUnitCommand
{
    public void Initialize(UnitInformation unitInfo)
    {
        unitInfo.Animator.SetFloat("Velocity", 0);
        unitInfo.Agent.isStopped = true;
    }
    public void UpdateUnitInstruction(UnitInformation unitInfo)
    {
        bool b = TouchInputManager.Instance
            .CompareToSelfTouchPhase(unitInfo.Unit.gameObject, TouchPhase.Ended);
        if (b)
        {
            unitInfo.ReleaseQueue();
            var newInstruction = new UnitMoveCommand();
            newInstruction.Initialize(unitInfo, unitInfo.TargetTower.position);
            unitInfo.InstrucitonQueue.Enqueue(newInstruction);
        }
    }
    public void Finalize(UnitInformation unitInfo)
    {

    }
}
