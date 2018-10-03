using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 動く駒を作るときはこれを継承
/// </summary>
public abstract class Unit : Actor {
    [SerializeField]
    int instantiateCost = 100;
    public int InstantiateCost
    {
        get { return instantiateCost; }
    }

    [SerializeField]
    int hp = 100;
    public int Hp { get { return hp; } }


    /// <summary>
    /// ここからダメージを与える
    /// </summary>
    /// <param name="damage">ダメージ量</param>
    /// <param name="unit">ダメージを与てくるUnit</param>
    public virtual void Damage(int damage,Unit unit)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Die();
        }
        print("damage");
    }

    protected virtual void  Die()
    {
        Destroy(gameObject);
    }
}
