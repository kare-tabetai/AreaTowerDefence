using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitMoveCommand : iUnitCommand
{
    /// <summary>
    /// この移動を示す矢印スプライトあれば終了時破棄する
    /// </summary>
    Transform[] allows;

    public void Initialize(UnitInformation unitInfo, Vector3 destination,bool drawAllow = false)
    {
        const float InstOffsetY = 0.02f;

        if (!unitInfo.Movable) { return; }
        NavMeshPath path = new NavMeshPath();
        var b= unitInfo.Agent.CalculatePath(destination, path);
        if (!b)
        {
            unitInfo.InstrucitonQueue.Dequeue();
            return;
        }
        unitInfo.Animator.SetFloat("Velocity", unitInfo.Agent.speed);
        unitInfo.Agent.isStopped = false;
        unitInfo.Agent.SetDestination(destination);
        if (drawAllow)
        {
            var allowTop = ResourceManager.Instance.MoveAllowTopPrefab;
            var allowBody = ResourceManager.Instance.MoveAllowBodyPrefab;
            var corners = path.corners;
            var previousPoint = unitInfo.Unit.transform.position;
            allows = new Transform[corners.Length + 1];
            for (int i = 0; i < corners.Length; i++)
            {

                Vector3 vec = corners[i] - previousPoint;
                if (vec == Vector3.zero)
                {
                    previousPoint = corners[i];
                    continue;
                }
                float magnitude = vec.magnitude;
                Vector3 instPos = previousPoint + vec / 2+Vector3.up*InstOffsetY;
                allows[i] = GameObject.Instantiate(allowBody, instPos, allowBody.transform.rotation).transform;
                allows[i].transform.forward = vec;
                allows[i].transform.Rotate(Vector3.right*(-90));
                Vector3 tmp = allows[i].transform.localScale;
                tmp.y *= magnitude;
                allows[i].transform.localScale = tmp;
                previousPoint = corners[i];
            }

            Vector3 way = destination - previousPoint;
            allows[allows.Length-1] = GameObject.Instantiate(allowTop, destination+Vector3.up*InstOffsetY, allowTop.transform.rotation).transform;
            allows[allows.Length - 1].transform.forward = way;
            allows[allows.Length - 1].transform.Rotate(Vector3.right * (-90));
        }
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
        if (allows != null)
        {
            foreach (var allow in allows)
            {
                if (allow != null)
                {
                    GameObject.Destroy(allow.gameObject);
                }
            }
        }
    }
}
