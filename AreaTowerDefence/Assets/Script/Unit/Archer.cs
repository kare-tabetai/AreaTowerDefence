using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Unit, iTouchEnd
{
    enum ArcherState
    {
        Progress,
        Fight,
    }

    [SerializeField]
    GameObject allow;
    [SerializeField]
    float speed = 5;

    List<Unit> unitInRange = new List<Unit>();
    [Disable, SerializeField]
    ArcherState state;

    void Start ()
	{
		
	}

    void Update()
    {
        if (!isInitialized) { return; }

        StateCheck();
        debugText.text = state.ToString();
        switch (state)
        {
            case ArcherState.Progress:
                break;

            case ArcherState.Fight:
                Fight();
                break;
        }
    }

    void StateCheck()
    {
        unitInRange.RemoveAll(item => item == null);//nullを削除
        if (unitInRange.Count == 0)
        {
            if (state == ArcherState.Progress) { return; }
            state = ArcherState.Progress;
            agent.isStopped = false;
            agent.SetDestination(targetTower.position);
            animator.SetBool("Attack", false);
            animator.SetFloat("Velocity", agent.speed);
            return;
        }
        else
        {
            if (state == ArcherState.Fight) { return; }
            state = ArcherState.Fight;
            agent.isStopped = true;
            animator.SetBool("Attack", true);
        }
    }

    void Fight()
    {
        Unit nearestUnit;
        var sqrDist = GetNearestUnit(this, unitInRange, out nearestUnit);
        Attack(nearestUnit);
    }

    float attackTimer = 0;
    void Attack(Unit attackTarget)
    {
        const float AttackRag = 1.0f;
        const float AllowOffsetY = 1f;
        print("attack");

        attackTimer += Time.deltaTime;
        if (AttackRag <= attackTimer)
        {
            attackTimer = 0f;
            var allow = Instantiate(this.allow, MainSceneManager.Instance.ActorNode).GetComponent<Allow>();
            var instPos = transform.position;
            instPos.y += AllowOffsetY;
            allow.transform.position = instPos;
            var targetPos = attackTarget.transform.position;
            targetPos.y += AllowOffsetY;
            allow.transform.forward = targetPos - instPos;
            allow.Initialize(PlayerNumber);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isInitialized) { return; }

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
        if (!isInitialized) { return; }

        if (other.tag == "Actor")
        {
            var unit = other.GetComponent<Unit>();
            if (unit == null) { return; }
            if (unit.PlayerNumber == PlayerNumber) { return; }
            unitInRange.Remove(unit);
        }
    }

    public void TouchEnd(TouchInputManager.TouchInfo touchInfo)
    {
        if (agent.speed == 0)
        {
            agent.speed = speed;
            animator.SetFloat("Velocity", agent.speed);
        }
        else
        {
            agent.speed = 0;
            animator.SetFloat("Velocity", agent.speed);
        }
    }
}
