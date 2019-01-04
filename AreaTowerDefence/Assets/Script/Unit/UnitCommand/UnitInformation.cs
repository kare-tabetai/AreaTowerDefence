using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// UnitInstructionに送る情報
/// Unitが持っているデータで
/// UnitInstructionが必要なものを全部送る
/// 1つずつ送ると面倒なのでPackしてるだけ
/// 入力をUnit  処理をUnitInstructionでさせる
/// </summary>
public struct UnitInformation
{
    public int PlayerNum;
    public int Hp;
    public Unit Unit;
    public bool Movable;
    public List<iUnitCommand> InstrucitonQueue;
    public Transform TargetTower;
    public NavMeshAgent Agent;
    public Animator Animator;
}
