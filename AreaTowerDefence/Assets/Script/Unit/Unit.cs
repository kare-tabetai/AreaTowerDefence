﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

/// <summary>
/// 動く駒を作るときはこれを継承
/// </summary>
public abstract class Unit : HpActor {

    protected TextMeshPro debugText;

    protected bool isActive;
    protected Transform targetTower;
    protected NavMeshAgent agent;
    protected Animator animator;

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
        isActive = true;
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