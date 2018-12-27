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
public abstract class Unit : HpActor,iTouchBegin,iTouchMoved {

    #region AI部分
    /// <summary>
    /// UnitInstructionに送る情報
    /// Unitが持っているデータで
    /// UnitInstructionが必要なものを全部送る
    /// Unitを継承した先で新たに必要なものができればこれを継承したものを送る
    /// 入力をUnit  処理,出力をUnitInstructionでさせる
    /// </summary>
    protected class Unitinformation
    {
        public int PlayerNum;
        public int Hp;
        public Unit Unit;
        public Queue<iUnitInstruction> InstrucitonQueue;
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
    protected interface iUnitInstruction
    {
        /// <summary>
        /// 毎フレームのアップデート
        /// </summary>
        /// <param name="unitInfo"></param>
        void UpdateUnitInstruction(Unitinformation unitInfo);
        /// <summary>
        /// Queueに追加されてから中止したい場合これを呼び出してからDeQueue
        /// </summary>
        void Finalize(Unitinformation unitInfo);
    }
    protected class UnitMoveInstruction : iUnitInstruction
    {
        /// <summary>
        /// この移動を示す矢印スプライトあれば終了時破棄する
        /// </summary>
        SpriteRenderer allowSprite;

        public void Initialize(Unitinformation unitInfo, Vector3 movePosition, SpriteRenderer allowSprite = null)
        {
            unitInfo.Animator.SetFloat("Speed", unitInfo.Agent.speed);
            unitInfo.Agent.isStopped = false;
            unitInfo.Agent.SetDestination(movePosition);
            this.allowSprite = allowSprite;
        }
        public void UpdateUnitInstruction(Unitinformation unitInfo)
        {
            const float GoalDistance = 0.7f;

            if (unitInfo.Agent.remainingDistance < GoalDistance)
            {
                Finalize(unitInfo);
                unitInfo.InstrucitonQueue.Dequeue();
            }
        }

        public void Finalize(Unitinformation unitInfo)
        {
            if (allowSprite != null) { Destroy(allowSprite.gameObject); }
        }
    }
    protected class UnitStopInstruction : iUnitInstruction
    {
        public void Initialize(Unitinformation unitInfo, Vector3 movePosition, SpriteRenderer allowSprite = null)
        {
            unitInfo.Animator.SetFloat("Speed", 0);
            unitInfo.Agent.isStopped = true;
        }
        public void UpdateUnitInstruction(Unitinformation unitInfo)
        {
            bool b = TouchInputManager.Instance
                .CompareToSelfTouchPhase(unitInfo.Unit.gameObject, TouchPhase.Ended);
            if (b)
            {
                unitInfo.ReleaseQueue();
                var newInstruction = new UnitMoveInstruction();
                newInstruction.Initialize(unitInfo,unitInfo.TargetTower.position);
                unitInfo.InstrucitonQueue.Enqueue(newInstruction);
            }
        }

        public void Finalize(Unitinformation unitInfo)
        {

        }
    }
    #endregion

    protected TextMeshPro debugText;

    protected bool isInitialized;//unitが召喚エリアに置かれたらtrueに
    protected Transform targetTower;
    protected NavMeshAgent agent;
    protected Animator animator;
    Queue<iUnitInstruction> instrucitonQueue;

    /// <summary>
    /// ドラッグ開始時に呼ばれる.
    /// AiActorCOntrollerでStartが呼ばれる前に呼びたいので
    /// 明示的呼び出しできるようにしている
    /// </summary>
    public void UnitStart()
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

    public void TouchBegin(TouchInputManager.TouchInfo touchInfo)
    {
        if (!isInitialized) { return; }
        PlayerActorController playerActorController 
            = (PlayerActorController)MainSceneManager.Instance
            .Controllers[touchInfo.PlayerNum];
        playerActorController.InstantiatedUnitBeginDrag(this);
    }

    public void TouchMoved(TouchInputManager.TouchInfo touchInfo)
    {
        if (!isInitialized) { return; }

    }

    /// <summary>
    /// リストからselfに一番近いunitを取り出す
    /// </summary>
    /// <param name="self">自分</param>
    /// <param name="unitList">探すunitのリスト</param>
    /// <param name="nearestUnit">一番近かったunit</param>
    /// <returns>一番近いunitの距離の2乗</returns>
    protected static float GetNearestUnit(Unit self,List<Unit> unitList,out Unit nearestUnit)
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
