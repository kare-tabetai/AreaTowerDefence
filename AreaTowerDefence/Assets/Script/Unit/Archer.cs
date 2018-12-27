using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Unit
{
    [SerializeField]
    GameObject allow;
    [SerializeField]
    float attackRag = 1;

    List<Unit> unitInRange = new List<Unit>();

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
        var unitMoveInstruction = new UnitMoveInstruction();
        unitMoveInstruction.Initialize(PackUnitInformation(),targetTower.transform.position);
        instrucitonQueue.Enqueue(unitMoveInstruction);
    }

    void Start ()
	{
    }

    void Update()
    {
        if (!isInitialized) { return; }
        if (instrucitonQueue.Count==0) { return; }
        var currentInstruction = instrucitonQueue.Peek();
        debugText.text = currentInstruction.ToString();

        if (unitInRange.Count != 0)
        {
            if (currentInstruction.GetType()==typeof(UnitFightInstruction))
            { return; }
            HasRangeUnitInformation unitInfo = PackHasRangeUnitInformation(unitInRange);
            unitInfo.UnitInRange = unitInRange;
            unitInfo.ReleaseQueue();
            var newInstruction = new UnitFightInstruction();
            newInstruction.Initialize(unitInfo, attackRag, allow);
            instrucitonQueue.Enqueue(newInstruction);
        }else
        {
            currentInstruction.UpdateUnitInstruction(PackUnitInformation());
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
}
