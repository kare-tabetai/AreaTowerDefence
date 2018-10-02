using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 継承してActorの生成と管理をやらせる．
/// Playerの入力でコントロールするのとAIで勝手にコントロールするのを作る
/// </summary>
public abstract class ActorController : MonoBehaviour {
    [SerializeField]
    int PlayerNumber;
    [SerializeField]
    public Transform OwnTower;
    [SerializeField]
    protected CostCounter costCounter;
    [SerializeField]
    public Color ThemeColor;
}
