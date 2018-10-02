using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 動く駒を作るときはこれを継承
/// </summary>
public abstract class Actor : MonoBehaviour {
    [SerializeField]
    int ownPlayerNum = 0;
    public int PlayerNum
    {
        get { return ownPlayerNum; }
        private set { ownPlayerNum = value; }
    }
    [SerializeField]
    int instantiateCost = 100;
    public int InstantiateCost
    {
        get { return instantiateCost; }
    }
}
