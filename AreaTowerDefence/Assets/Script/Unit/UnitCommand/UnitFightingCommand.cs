using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UnitFightingCommand : iUnitCommand
{
    float attackTimer = 0;
    float attackRag = 0;
    int attackPower = -999;
    /// <summary>
    /// nullなら通常攻撃をする
    /// </summary>
    GameObject bulletPrefab;
    List<Unit> rangeInUnitList;

    public void Initialize(UnitInformation unitInfo, List<Unit> rangeInUnitList, float attackRag, int attackPower)
    {
        unitInfo.Animator.SetBool("Attack", true);
        unitInfo.Agent.isStopped = true;
        this.rangeInUnitList = rangeInUnitList;
        this.attackRag = attackRag;
        this.attackPower = attackPower;
        initialized = true;
    }
    public void Initialize(UnitInformation unitInfo, List<Unit> rangeInUnitList, float attackRag, GameObject bulletPrefab)
    {
        unitInfo.Animator.SetBool("Attack", true);
        unitInfo.Agent.isStopped = true;
        this.rangeInUnitList = rangeInUnitList;
        this.attackRag = attackRag;
        this.bulletPrefab = bulletPrefab;
        initialized = true;
    }

    public override void UpdateUnitInstruction(UnitInformation unitInfo)
    {

        //キャスト可能なはず
        if (rangeInUnitList.Count == 0)
        {
            Finalize(unitInfo);
            Debug.Assert(unitInfo.InstrucitonQueue.First() == this);
            unitInfo.InstrucitonQueue.RemoveAt(0);
            unitInfo.Agent.isStopped = false;
            unitInfo.Animator.SetBool("Attack", false);
            return;
        }
        Unit nearestUnit;
        var sqrDist = Unit.GetNearestUnit(unitInfo.Unit, rangeInUnitList, out nearestUnit);
        Attack(unitInfo, nearestUnit);
    }

    void Attack(UnitInformation unitInfo, Unit attackTarget)
    {
        const float BulletOffsetY = 1f;

        attackTimer += Time.deltaTime;
        if (attackRag <= attackTimer)
        {
            attackTimer = 0f;
            if (bulletPrefab != null)
            {
                var bullet =
                GameObject.Instantiate(bulletPrefab, MainSceneManager.Instance.ActorNode)
                .GetComponent<Actor>();
                var instPos = unitInfo.Unit.transform.position;
                instPos.y += BulletOffsetY;
                bullet.transform.position = instPos;
                var targetPos = attackTarget.transform.position;
                targetPos.y += BulletOffsetY;
                bullet.transform.forward = targetPos - instPos;
                bullet.Initialize(unitInfo.PlayerNum);
            }
            else
            {
                Debug.Assert(0 < attackPower);
                attackTarget.Damage(attackPower, unitInfo.Unit);
            }

        }
    }

    public override void Finalize(UnitInformation unitInfo)
    {

    }
}