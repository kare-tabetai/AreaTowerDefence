using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wizard : Unit
{
    enum WizardState
    {
        Progress,
        Fight,
    }

    [SerializeField]
    GameObject magicBullet;

    List<Unit> unitInRange = new List<Unit>();
    WizardState state;
    Transform targetTower;
    NavMeshAgent agent;
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        targetTower = MainSceneManager.Instance.GetNearestOtherPlayerTower(PlayerNumber, transform.position);
        agent.SetDestination(targetTower.position);
    }

    void Update()
    {
        StateCheck();
        switch (state)
        {
            case WizardState.Progress:
                break;

            case WizardState.Fight:
                Fight();
                break;
        }
    }

    void StateCheck()
    {
        unitInRange.RemoveAll(item => item == null);//nullを削除
        if (unitInRange.Count == 0)
        {
            state = WizardState.Progress;
            agent.isStopped = false;
            agent.SetDestination(targetTower.position);
        }
        else
        {
            state = WizardState.Fight;
        }
    }

    void Fight()
    {
        Unit nearestUnit;
        var sqrDist = GetNearestUnit(this,unitInRange,out nearestUnit);
        agent.isStopped = true;
        Attack(nearestUnit);
    }

    float attackTimer = 0;
    void Attack(Unit attackTarget)
    {
        const float AttackRag = 1.0f;
        print("attack");

        attackTimer += Time.deltaTime;
        if (AttackRag <= attackTimer)
        {
            attackTimer = 0f;
            var bullet = Instantiate(magicBullet,MainSceneManager.Instance.ActorNode).GetComponent<MagicBullet>();
            bullet.transform.position = transform.position;
            bullet.Initialize(PlayerNumber, attackTarget.transform);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print("hit");
        if (other.tag == "Actor")
        {
            print("actor");
            var unit = other.GetComponent<Unit>();
            if (unit == null) { return; }
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
