﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 継承してActorの生成と管理をやらせる．
/// Playerの入力でコントロールするのとAIで勝手にコントロールするのを作る
/// PlayerNumが欲しいのでActorから継承
/// </summary>
public abstract class ActorController : Actor {
    
    [SerializeField]
    public Transform OwnTower;
    [SerializeField]
    protected CostCounter costCounter;
    [SerializeField]
    public Color ThemeColor;
}
