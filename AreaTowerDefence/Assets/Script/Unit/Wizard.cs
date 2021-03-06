﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Wizard : Unit
{
    [SerializeField]
    GameObject magicBullet;
    [SerializeField]
    float attackRag=1;

    List<Unit> rangeInUnitList = new List<Unit>();

    public override void Initialize(int playerNum)
    {
        base.Initialize(playerNum);
        var unitMoveCommand = new UnitMoveCommand();
        unitMoveCommand
            .Initialize(PackUnitInformation(), targetTower.transform.position);
        CommandQueue.Add(unitMoveCommand);
    }

    void Start () {

    }

    void Update()
    {
        if (!isInitialized) { return; }
        rangeInUnitList.RemoveAll(item => item == null);//nullを削除

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
        if (CommandQueue.Count == 0) { return; }

        debugText.text = CommandQueue.First().ToString();

        var currentCommand = CommandQueue.First();
        var CommandType = currentCommand.GetType();

        if (CommandType != typeof(UnitFightingCommand))
        {
            if (rangeInUnitList.Count != 0)
            {
                var unitInfo = PackUnitInformation();
                unitInfo.Unit.ReleaseCommandQueue();
                var newInstruction = new UnitFightingCommand();
                newInstruction.Initialize(unitInfo,rangeInUnitList, attackRag, magicBullet);
                CommandQueue.Add(newInstruction);
            }
        }

        if (CommandQueue.Count != 0)
        {
            CommandQueue.First().UpdateUnitInstruction(PackUnitInformation());
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
            rangeInUnitList.Add(unit);
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
            rangeInUnitList.Remove(unit);
        }
    }
}
