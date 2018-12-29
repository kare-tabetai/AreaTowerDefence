using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System;

/// <summary>
/// 動く駒を作るときはこれを継承
/// </summary>
public abstract class Unit : HpActor,iTouchBegin,iTouchMoved {

    protected TextMeshPro debugText;

    protected bool isInitialized;//unitが召喚エリアに置かれたらtrueに
    protected Transform targetTower;
    protected NavMeshAgent agent;
    protected Animator animator;
    /// <summary>
    /// これがステートのQueue,
    /// Unitの子やiUnitInstructionの子でDequeue,Enqueue,Clearして管理する
    /// </summary>
    protected Queue<iUnitCommand> instrucitonQueue=new Queue<iUnitCommand>();

    /// <summary>
    /// ドラッグ開始時に呼ばれる.
    /// AiActorCOntrollerでStartが呼ばれる前に呼びたいので
    /// 明示的呼び出しできるようにしている
    /// </summary>
    public void UnitStart()
    {
        debugText = GetComponentInChildren<TextMeshPro>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        var colliders = GetComponentsInChildren<Collider>();//タッチのレイキャストに反応しないように
        foreach(var col in colliders)
        {
            col.enabled = false;
        }
        agent.enabled = false;
    }

    /// <summary>
    /// 召喚時に外部から呼び出される.独自に処理したければoverridce
    /// </summary>
    public override void Initialize(int playerNum)
    {
        base.Initialize(playerNum);
        isInitialized = true;
        var colliders = GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
        {
            col.enabled = true;
        }
        agent.enabled = true;
        targetTower = MainSceneManager.Instance.GetNearestOtherPlayerTower(PlayerNumber, transform.position);
        agent.SetDestination(targetTower.position);
        animator.SetFloat("Velocity", agent.speed);
    }

    public void TouchBegin(TouchInputManager.TouchInfo touchInfo)
    {
        if (!isInitialized) { return; }
        PlayerActorController playerActorController 
            = (PlayerActorController)MainSceneManager.Instance
            .Controllers[touchInfo.PlayerNum];
        playerActorController.InstantiatedUnitBeginDrag(this);
    }

    public void TouchMoved(TouchInputManager.TouchInfo touchInfo)
    {
        if (!isInitialized) { return; }
    }

    protected virtual UnitInformation PackUnitInformation()
    {
        var unitInformation = new UnitInformation();
        unitInformation.PlayerNum = PlayerNumber;
        unitInformation.Unit = this;
        unitInformation.InstrucitonQueue = instrucitonQueue;
        unitInformation.TargetTower = targetTower;
        unitInformation.Agent = agent;
        unitInformation.Animator = animator;
        return unitInformation;
    }
    protected virtual HasRangeUnitInformation PackHasRangeUnitInformation(List<Unit> unitInRange)
    {
        var unitInformation = new HasRangeUnitInformation();
        unitInformation.PlayerNum = PlayerNumber;
        unitInformation.Unit = this;
        unitInformation.InstrucitonQueue = instrucitonQueue;
        unitInformation.TargetTower = targetTower;
        unitInformation.Agent = agent;
        unitInformation.Animator = animator;
        unitInformation.UnitInRange = unitInRange;
        return unitInformation;
    }

    /// <summary>
    /// リストからselfに一番近いunitを取り出す
    /// </summary>
    /// <param name="self">自分</param>
    /// <param name="unitList">探すunitのリスト</param>
    /// <param name="nearestUnit">一番近かったunit</param>
    /// <returns>一番近いunitの距離の2乗</returns>
    public static float GetNearestUnit(Unit self,List<Unit> unitList,out Unit nearestUnit)
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
