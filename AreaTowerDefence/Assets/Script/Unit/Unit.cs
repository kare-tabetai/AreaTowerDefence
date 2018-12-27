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
    protected class UnitInformation
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
    protected class HasRangeUnitInformation:UnitInformation
    {
        public List<Unit> UnitInRange = new List<Unit>();
    }

    protected interface iUnitInstruction
    {
        /// <summary>
        /// 毎フレームのアップデート
        /// </summary>
        /// <param name="unitInfo"></param>
        void UpdateUnitInstruction(UnitInformation unitInfo);
        /// <summary>
        /// Queueに追加されてから中止したい場合これを呼び出してからDeQueue
        /// </summary>
        void Finalize(UnitInformation unitInfo);
    }
    protected class UnitMoveInstruction : iUnitInstruction
    {
        /// <summary>
        /// この移動を示す矢印スプライトあれば終了時破棄する
        /// </summary>
        SpriteRenderer allowSprite;

        public void Initialize(UnitInformation unitInfo, Vector3 movePosition, SpriteRenderer allowSprite = null)
        {
            unitInfo.Animator.SetFloat("Velocity", unitInfo.Agent.speed);
            unitInfo.Agent.isStopped = false;
            unitInfo.Agent.SetDestination(movePosition);
            this.allowSprite = allowSprite;
        }
        public void UpdateUnitInstruction(UnitInformation unitInfo)
        {
            const float GoalDistance = 0.7f;

            var distance= unitInfo.Agent.destination - unitInfo.Unit.transform.position;
            var sqeDist = distance.sqrMagnitude;
            if (sqeDist < GoalDistance*GoalDistance)
            {
                Finalize(unitInfo);
                unitInfo.InstrucitonQueue.Dequeue();
                return;
            }
            bool b = TouchInputManager.Instance
                .CompareToSelfTouchPhase(unitInfo.Unit.gameObject, TouchPhase.Ended);
            if (b)
            {
                unitInfo.ReleaseQueue();
                var newInstruction = new UnitStopInstruction();
                newInstruction.Initialize(unitInfo);
                unitInfo.InstrucitonQueue.Enqueue(newInstruction);
            }
        }
        public void Finalize(UnitInformation unitInfo)
        {
            if (allowSprite != null) { Destroy(allowSprite.gameObject); }
        }
    }
    protected class UnitStopInstruction : iUnitInstruction
    {
        public void Initialize(UnitInformation unitInfo)
        {
            unitInfo.Animator.SetFloat("Velocity", 0);
            unitInfo.Agent.isStopped = true;
        }
        public void UpdateUnitInstruction(UnitInformation unitInfo)
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
        public void Finalize(UnitInformation unitInfo)
        {

        }
    }
    protected class UnitFightInstruction : iUnitInstruction
    {
        float attackTimer = 0;
        float attackRag = 0;
        int attackPower = -999;
        /// <summary>
        /// nullなら通常攻撃をする
        /// </summary>
        GameObject bulletPrefab;

        public void Initialize(UnitInformation unitInfo, float attackRag, int attackPower)
        {
            unitInfo.Animator.SetBool("Attack", true);
            unitInfo.Agent.isStopped = true;
            this.attackRag = attackRag;
            this.attackPower = attackPower;
        }
        public void Initialize(UnitInformation unitInfo,float attackRag,GameObject bulletPrefab)
        {
            unitInfo.Animator.SetBool("Attack",true);
            unitInfo.Agent.isStopped = true;
            this.attackRag = attackRag;
            this.bulletPrefab = bulletPrefab;
        }
        public void UpdateUnitInstruction(UnitInformation unitInfo)
        {
            //キャスト可能なはず
            var rangehasUnitInfo = (HasRangeUnitInformation)unitInfo;

            if (rangehasUnitInfo.UnitInRange.Count == 0)
            {
                Finalize(unitInfo);
                unitInfo.InstrucitonQueue.Dequeue();
                rangehasUnitInfo.Agent.isStopped = false;
                rangehasUnitInfo.Animator.SetBool("Attack",false);
                return;
            }
            Unit nearestUnit;
            var sqrDist = GetNearestUnit(rangehasUnitInfo.Unit, rangehasUnitInfo.UnitInRange, out nearestUnit);
            Attack(rangehasUnitInfo, nearestUnit);
        }
        void Attack(HasRangeUnitInformation unitInfo,Unit attackTarget)
        {
            const float BulletOffsetY = 1f;

            print("attack");

            attackTimer += Time.deltaTime;
            if (attackRag <= attackTimer)
            {
                attackTimer = 0f;
                if (bulletPrefab != null)
                {
                    var bullet =
                    Instantiate(bulletPrefab, MainSceneManager.Instance.ActorNode)
                    .GetComponent<Actor>();
                    var instPos = unitInfo.Unit.transform.position;
                    instPos.y += BulletOffsetY;
                    bullet.transform.position = instPos;
                    var targetPos = attackTarget.transform.position;
                    targetPos.y += BulletOffsetY;
                    bullet.transform.forward = targetPos - instPos;
                    bullet.Initialize(unitInfo.PlayerNum);
                }else
                {
                    Debug.Assert(0 < attackPower);
                    attackTarget.Damage(attackPower, unitInfo.Unit);
                }

            }
        }
        public void Finalize(UnitInformation unitInfo)
        {

        }
    }
    #endregion

    protected TextMeshPro debugText;

    protected bool isInitialized;//unitが召喚エリアに置かれたらtrueに
    protected Transform targetTower;
    protected NavMeshAgent agent;
    protected Animator animator;
    /// <summary>
    /// これがステートのQueue,
    /// Unitの子やiUnitInstructionの子でDequeue,Enqueue,Clearして管理する
    /// </summary>
    protected Queue<iUnitInstruction> instrucitonQueue=new Queue<iUnitInstruction>();

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

    protected virtual UnitInformation PackUnitInformation()
    {
        var unitInformation = new UnitInformation();
        unitInformation.PlayerNum = PlayerNumber;
        unitInformation.Unit = this;
        unitInformation.InstrucitonQueue = instrucitonQueue;
        unitInformation.TargetTower = targetTower;
        unitInformation.Agent = agent;
        unitInformation.Animator = animator;
        return unitInformation;
    }
    protected virtual HasRangeUnitInformation PackHasRangeUnitInformation(List<Unit> unitInRange)
    {
        var unitInformation = new HasRangeUnitInformation();
        unitInformation.PlayerNum = PlayerNumber;
        unitInformation.Unit = this;
        unitInformation.InstrucitonQueue = instrucitonQueue;
        unitInformation.TargetTower = targetTower;
        unitInformation.Agent = agent;
        unitInformation.Animator = animator;
        unitInformation.UnitInRange = unitInRange;
        return unitInformation;
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
