using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// UnitInstructionに送る情報
/// Unitが持っているデータで
/// UnitInstructionが必要なものを全部送る
/// Unitを継承した先で新たに必要なものができればこれを継承したものを送る
/// 入力をUnit  処理,出力をUnitInstructionでさせる
/// </summary>
public class UnitInformation
{
    public int PlayerNum;
    public int Hp;
    public Unit Unit;
    public bool Movable;
    public Queue<iUnitCommand> InstrucitonQueue;
    public Transform TargetTower;
    public NavMeshAgent Agent;
    public Animator Animator;

    public void ReleaseQueue()
    {
        foreach (var item in this.InstrucitonQueue)
        {
            item.Finalize(this);
        }
        this.InstrucitonQueue.Clear();
    }
}
