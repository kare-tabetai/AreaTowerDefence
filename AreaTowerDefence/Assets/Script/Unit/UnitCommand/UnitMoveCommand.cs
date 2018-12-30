using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitMoveCommand : iUnitCommand
{
    /// <summary>
    /// この移動を示す矢印スプライトあれば終了時破棄する
    /// </summary>
    MeshRenderer[] allows;

    public void Initialize(UnitInformation unitInfo, Vector3 destination,bool drawAllow = false)
    {
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
            allows = new MeshRenderer[corners.Length];
            Debug.Log(corners.Length);
            for (int i = 0; i < corners.Length; i++)
            {
                //const float MinLength = 0.01f;

                Vector3 vec = corners[i] - previousPoint;
                if (vec == Vector3.zero)
                {
                    previousPoint = corners[i];
                    continue;
                }
                float magnitude = vec.magnitude;
                //if (vec.sqrMagnitude < MinLength)
                //{
                //    previousPoint = corners[i];
                //    continue;
                //}
                Vector3 instPos = previousPoint + vec / 2;
                allows[i] = GameObject.Instantiate(allowBody, instPos, allowBody.transform.rotation).GetComponent<MeshRenderer>();
                allows[i].transform.forward = vec;
                Vector3 tmp = allows[i].transform.localScale;
                tmp.z *= magnitude;
                allows[i].transform.localScale = tmp;
                previousPoint = corners[i];
            }

            allows[allows.Length-1] = GameObject.Instantiate(allowBody, destination, allowTop.transform.rotation).GetComponent<MeshRenderer>();
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
