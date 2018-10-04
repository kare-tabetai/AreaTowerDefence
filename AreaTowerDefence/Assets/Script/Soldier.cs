using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Soldier : Unit {

    enum SoldierState
    {
        Progress,
        Fight,
    }

    List<HpActor> hpActorsInRange = new List<HpActor>();
    SoldierState state;
    Transform targetTower;
    NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        targetTower = MainSceneManager.Instance.GetNearestOtherPlayerTower(PlayerNumber,transform.position);
        agent.SetDestination(targetTower.position);

    }

    void Update()
    {
        StateCheck();
        switch (state)
        {
            case SoldierState.Progress:
                break;

            case SoldierState.Fight:
                Fight();
                break;
        }
    }

    void StateCheck()
    {
        hpActorsInRange.RemoveAll(item => item == null);//nullを削除
        if (hpActorsInRange.Count == 0)
        {
            state = SoldierState.Progress;
            agent.isStopped = false;
            agent.SetDestination(targetTower.position);
        }
        else
        {
            state = SoldierState.Fight;
        }
    }

    void Fight()
    {
        const float attackDist = 1.5f;//これ以内の距離ならAttack

        HpActor nearestUnit;
        var sqrDist = GetNearestUnit(out nearestUnit);
        if(sqrDist <= attackDist * attackDist)
        {
            agent.isStopped = true;
            Attack(nearestUnit);
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(nearestUnit.transform.position);
        }
    }

    float attackTimer = 0;
    void Attack(HpActor attackTarget)
    {
        const float AttackRag = 1.0f;
        const int AttackPower = 80;
        print("attack");

        attackTimer += Time.deltaTime;
        if (AttackRag <= attackTimer)
        {
            attackTimer = 0f;
            attackTarget.Damage(AttackPower, this);
        }
    }

    float GetNearestUnit(out HpActor nearestUnit)
    {
        Debug.Assert(hpActorsInRange.Count != 0);

        float nearestSqrDist = Mathf.Infinity;
        nearestUnit = null;
        foreach (var hpActor in hpActorsInRange)
        {
            Debug.Assert(hpActor.PlayerNumber != PlayerNumber);
            float sqrDist = Vector3.SqrMagnitude(hpActor.transform.position - transform.position);
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

    private void OnTriggerEnter(Collider other)
    {
            print("hit");
        if (other.tag == "Actor")
        {
            print("actor");
            var unit = other.GetComponent<Unit>();
            if (unit==null) { return; }
            if (unit.PlayerNumber == PlayerNumber) { return; }
            hpActorsInRange.Add(unit);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Actor")
        {
            var unit = other.GetComponent<Unit>();
            if (unit == null) { return; }
            if (unit.PlayerNumber == PlayerNumber) { return; }
            hpActorsInRange.Remove(unit);
        }
    }

    void EnemyCheck()
    {

    }
}
