using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Soldier : Unit {

    enum SoldierState
    {
        Progress,
        Fight,
    }

    List<Unit> unitInRange = new List<Unit>();
    [Disable,SerializeField]
    SoldierState state;
    Transform targetTower;
    NavMeshAgent agent;
    Animator animator;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        targetTower = MainSceneManager.Instance.GetNearestOtherPlayerTower(PlayerNumber,transform.position);
        agent.SetDestination(targetTower.position);
        animator.SetFloat("Velocity", agent.speed);
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
        unitInRange.RemoveAll(item => item == null);//nullを削除
        if (unitInRange.Count == 0)
        {
            if(state == SoldierState.Progress) { return; }
            state = SoldierState.Progress;
            agent.isStopped = false;
            agent.SetDestination(targetTower.position);
            animator.SetFloat("Velocity", agent.speed);
            animator.SetBool("Attack", false);
        }
        else
        {
            if (state == SoldierState.Fight) { return; }
            state = SoldierState.Fight;
            agent.isStopped = true;
            animator.SetBool("Attack", true);
        }
    }

    void Fight()
    {
        Unit nearestUnit;
        var sqrDist = GetNearestUnit(this, unitInRange, out nearestUnit);
        if(unitInRange.Any(item=>item==nearestUnit))
        {
            Attack(nearestUnit);
        }
    }

    float attackTimer = 0;
    void Attack(Unit attackTarget)
    {
        const float AttackRag = 1.0f;
        const int AttackPower = 80;
        attackTimer += Time.deltaTime;
        if (AttackRag <= attackTimer)
        {
            attackTimer = 0f;
            attackTarget.Damage(AttackPower, this);
        }
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
            unitInRange.Add(unit);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Actor")
        {
            var unit = other.GetComponent<Unit>();
            if (unit == null) { return; }
            if (unit.PlayerNumber == PlayerNumber) { return; }
            unitInRange.Remove(unit);
        }
    }
}
