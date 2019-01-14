using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 持ち主のプレイヤーがあるもの
/// PlayerNumberが必要なもの
/// </summary>
public abstract class Actor : MonoBehaviour {
    [SerializeField]
    int playerNumber;//prefabごとに変えたい場合もあるのでDisableにはしない
    public int PlayerNumber
    {
        get { return playerNumber; }
        private set { playerNumber = value; }
    }

    /// <summary>
    /// 初期化したければ呼ぶ
    /// 呼ばなくていいなら呼ばなくていい
    /// </summary>
    public virtual void Initialize(int playerNum)
    {
        PlayerNumber = playerNum;
    }

    
}
