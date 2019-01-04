using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitMoveCommand : iUnitCommand
{
    /// <summary>
    /// この移動を示す矢印スプライトあれば終了時破棄する
    /// </summary>
    GameObject lineObj;

    public void Initialize(UnitInformation unitInfo, Vector3 destination,bool drawAllow = false)
    {
        if (!unitInfo.Movable) { return; }

        NavMeshPath path = new NavMeshPath();
        var b= unitInfo.Agent.CalculatePath(destination, path);
        if (!b)
        {
            unitInfo.InstrucitonQueue.Remove(this);
            return;
        }
        unitInfo.Animator.SetFloat("Velocity", unitInfo.Agent.speed);
        unitInfo.Agent.isStopped = false;
        unitInfo.Agent.SetDestination(destination);
        if (drawAllow)
        {
            var rootRendererObj = ResourceManager.Instance.RootRendererPrefab;
            lineObj = GameObject.Instantiate(rootRendererObj);
            var rootRenderer = lineObj.GetComponent<RootRenderer>();
            var startPos = unitInfo.Unit.transform.position;
            var corners = path.corners;
            var endPos = destination;
            rootRenderer.Initialize(startPos,corners,endPos);
        }
        initialized = true;
    }
    public override void UpdateUnitInstruction(UnitInformation unitInfo)
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
            unitInfo.InstrucitonQueue.RemoveAt(0);
            return;
        }
        
    }
    public override void Finalize(UnitInformation unitInfo)
    {
        if (lineObj != null)
        {
            GameObject.Destroy(lineObj.gameObject);
        }
    }
}
