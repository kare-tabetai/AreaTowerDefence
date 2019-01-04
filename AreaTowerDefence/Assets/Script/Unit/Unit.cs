using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System;

/// <summary>
/// 動く駒を作るときはこれを継承
/// </summary>
public abstract class Unit : HpActor {

    protected TextMeshPro debugText;

    protected bool isInitialized;//unitが召喚エリアに置かれたらtrueに
    public bool IsInitialized { get { return isInitialized; } }
    protected Transform targetTower;
    protected NavMeshAgent agent;
    protected Animator animator;

    /// <summary>
    /// Dequeue,Enqueue,Clearして管理する
    /// </summary>
    public Queue<iUnitCommand> CommandQueue=new Queue<iUnitCommand>();

    /// <summary>
    /// 移動できるかtrueでも攻撃時などは移動できません
    /// </summary>
    protected bool movable = true;

    /// <summary>
    /// ドラッグ開始時に呼ばれる.
    /// AiActorCOntrollerでStartが呼ばれる前に呼びたいので
    /// 明示的呼び出しできるようにしている
    /// </summary>
    public void Active()
    {
        debugText = GetComponentInChildren<TextMeshPro>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        var colliders = GetComponentsInChildren<Collider>();//タッチのレイキャストに反応しないように
        foreach(var col in colliders)
        {
            col.enabled = false;
        }
        agent.enabled = false;
    }

    /// <summary>
    /// 召喚時に外部から呼び出される.独自に処理したければoverridce
    /// </summary>
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
        agent.SetDestination(targetTower.position);
        animator.SetFloat("Velocity", agent.speed);
    }

    /// <summary>
    /// コマンドキューをFinalizeしてClearする
    /// </summary>
    public void ReleaseCommandQueue()
    {
        var info = PackUnitInformation();
        foreach (var item in this.CommandQueue)
        {
            item.Finalize(info);
        }
        this.CommandQueue.Clear();
    }

    /// <summary>
    /// 引数で渡したコマンドのみFinalizeしてClearする
    /// </summary>
    public void ReleaseCommandQueue<T>()
    {
        var info = PackUnitInformation();
        var command = CommandQueue.Where(item => item.GetType() == typeof(T));
        foreach (var item in command)
        {
            item.Finalize(info);
        }
        //wip
        //CommandQueue.
    }

    public UnitInformation PackUnitInformation()
    {
        var unitInformation = new UnitInformation();
        unitInformation.PlayerNum = PlayerNumber;
        unitInformation.Unit = this;
        unitInformation.Movable = movable;
        unitInformation.InstrucitonQueue = CommandQueue;
        unitInformation.TargetTower = targetTower;
        unitInformation.Agent = agent;
        unitInformation.Animator = animator;
        return unitInformation;
    }

    [ContextMenu("DebugPrintCommandQueue")]
    public void DebugPrintCommandQueue()
    {
        foreach(var item in CommandQueue)
        {
            print(item.ToString());
        }
    }

    /// <summary>
    /// リストからselfに一番近いunitを取り出す
    /// </summary>
    /// <param name="self">自分</param>
    /// <param name="unitList">探すunitのリスト</param>
    /// <param name="nearestUnit">一番近かったunit</param>
    /// <returns>一番近いunitの距離の2乗</returns>
    public static float GetNearestUnit(Unit self,List<Unit> unitList,out Unit nearestUnit)
    {
        Debug.Assert(unitList.Count != 0);

        float nearestSqrDist = Mathf.Infinity;
        nearestUnit = null;
        foreach (var hpActor in unitList)
        {
            Debug.Assert(hpActor.PlayerNumber != self.PlayerNumber);
            float sqrDist = Vector3.SqrMagnitude(hpActor.transform.position - self.transform.position);
            if (sqrDist < nearestSqrDist)
            {
                nearestSqrDist = sqrDist;
                nearestUnit = hpActor;
            }
        }
        Debug.Assert(nearestUnit != null);
        Debug.Assert(nearestUnit.gameObject != null);
        Debug.Assert(nearestSqrDist != Mathf.Infinity);
        return nearestSqrDist;
    }
}
