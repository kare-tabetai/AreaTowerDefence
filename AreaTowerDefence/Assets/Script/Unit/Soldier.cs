using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Soldier : Unit {

    [SerializeField]
    float attackRag = 1;
    [SerializeField]
    int attackPower = 80;

    List<Unit> rangeInUnitList = new List<Unit>();

    public override void Initialize(int playerNum)
    {
        base.Initialize(playerNum);
        var unitMoveCommand = new UnitMoveCommand();
        unitMoveCommand
            .Initialize(PackUnitInformation(), targetTower.transform.position);
        CommandQueue.Enqueue(unitMoveCommand);
    }

    void Start()
    {

    }

    void Update()
    {
        if (!isInitialized) { return; }
        rangeInUnitList.RemoveAll(item => item == null);//nullを削除

        CommandCheck();
    }

    void CommandCheck()
    {
        if (CommandQueue.Count == 0) { return; }

        debugText.text = CommandQueue.Peek().ToString();

        var currentCommand = CommandQueue.Peek();
        var CommandType = currentCommand.GetType();

        if (CommandType != typeof(UnitFightingCommand))
        {
            if (rangeInUnitList.Count != 0)
            {
                UnitInformation unitInfo = PackUnitInformation();
                unitInfo.ReleaseQueue();
                var newInstruction = new UnitFightingCommand();
                newInstruction.Initialize(unitInfo, rangeInUnitList, attackRag,attackPower);
                CommandQueue.Enqueue(newInstruction);
            }
        }

        //currentCommandが更新されているかもしれないからPeek
        if (CommandQueue.Count != 0)
        {
            CommandQueue.Peek().UpdateUnitInstruction(PackUnitInformation());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isInitialized) { return; }

        if (other.tag == "Actor")
        {
            print("actor");
            var unit = other.GetComponent<Unit>();
            if (unit==null) { return; }
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
