using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wizard : Unit
{
    [SerializeField]
    GameObject magicBullet;
    [SerializeField]
    float attackRag;

    List<Unit> unitInRange = new List<Unit>();

    public override void Initialize(int playerNum)
    {
        base.Initialize(playerNum);
        var unitMoveCommand = new UnitMoveCommand();
        unitMoveCommand
            .Initialize(PackUnitInformation(), targetTower.transform.position);
        commandQueue.Enqueue(unitMoveCommand);
    }

    void Start () {

    }

    void Update()
    {
        if (!isInitialized) { return; }
        unitInRange.RemoveAll(item => item == null);//nullを削除

        bool isTouched = TouchInputManager.Instance
            .CompareToSelfTouchPhase(gameObject, TouchPhase.Ended);
        if (isTouched)
        {
            movable = !movable;
        }

        CommandCheck();
    }

    void CommandCheck()
    {
        if (commandQueue.Count == 0) { return; }

        debugText.text = commandQueue.Peek().ToString();

        var currentCommand = commandQueue.Peek();
        var CommandType = currentCommand.GetType();

        if (CommandType != typeof(UnitFightingCommand))
        {
            if (unitInRange.Count != 0)
            {
                HasRangeUnitInformation unitInfo = PackHasRangeUnitInformation(unitInRange);
                unitInfo.UnitInRange = unitInRange;
                unitInfo.ReleaseQueue();
                var newInstruction = new UnitFightingCommand();
                newInstruction.Initialize(unitInfo, attackRag, magicBullet);
                commandQueue.Enqueue(newInstruction);
            }
        }

        if (commandQueue.Count != 0)
        {
            commandQueue.Peek().UpdateUnitInstruction(PackUnitInformation());
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
