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
    [Disable, SerializeField]
    WizardState state;
    Transform targetTower;
    NavMeshAgent agent;
    Animator animator;
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        targetTower = MainSceneManager.Instance.GetNearestOtherPlayerTower(PlayerNumber, transform.position);
        agent.SetDestination(targetTower.position);
        animator.SetFloat("Velocity", agent.speed);
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
            if (state == WizardState.Progress) { return; }
            state = WizardState.Progress;
            agent.isStopped = false;
            agent.SetDestination(targetTower.position);
            animator.SetBool("Attack",false);
            animator.SetFloat("Velocity", agent.speed);
        }
        else
        {
            if (state == WizardState.Fight) { return; }
            state = WizardState.Fight;
            agent.isStopped = true;
            animator.SetBool("Attack",true);
        }
    }

    void Fight()
    {
        Unit nearestUnit;
        var sqrDist = GetNearestUnit(this,unitInRange,out nearestUnit);
        Attack(nearestUnit);
    }

    float attackTimer = 0;
    void Attack(Unit attackTarget)
    {
        const float AttackRag = 1.0f;
        const float MagicBulletOffsetY = 0.5f;
        print("attack");

        attackTimer += Time.deltaTime;
        if (AttackRag <= attackTimer)
        {
            attackTimer = 0f;
            var bullet = Instantiate(magicBullet,MainSceneManager.Instance.ActorNode).GetComponent<MagicBullet>();
            var instPos = transform.position;
            instPos.y += MagicBulletOffsetY;
            bullet.transform.position = instPos;
            bullet.Initialize(PlayerNumber, attackTarget.transform);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Actor")
        {
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
