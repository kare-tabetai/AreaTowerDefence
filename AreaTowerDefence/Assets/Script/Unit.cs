using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 動く駒を作るときはこれを継承
/// </summary>
public abstract class Unit : HpActor {
    [SerializeField]
    int instantiateCost = 100;
    public int InstantiateCost
    {
        get { return instantiateCost; }
    }
}
