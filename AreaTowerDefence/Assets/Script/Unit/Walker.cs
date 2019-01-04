using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Walker : Unit
{

    [SerializeField]
    GameObject instantiatePrefab;

    GameObject instantiateArea;

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

        bool isTouched = TouchInputManager.Instance
            .CompareToSelfTouchPhase(gameObject, TouchPhase.Ended);
        if (isTouched&& instantiateArea==null)
        {
            movable = false;
            InstantiateArea();
        }

        if (CommandQueue.Count == 0) { return; }
        CommandQueue.First().UpdateUnitInstruction(PackUnitInformation());
    }

    void InstantiateArea()
    {
        const float OffsetY = 0.05f;//重なってちらつくのを防ぐため
        var pos = transform.position;
        pos.y += OffsetY;
        instantiateArea = Instantiate(instantiatePrefab, pos, instantiatePrefab.transform.rotation);
        instantiateArea.GetComponent<Actor>().Initialize(PlayerNumber);
    }
}
