using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HpActor : Actor {
    [SerializeField]
    int hp = 100;
    public int Hp
    {
        get { return hp; }
        protected set { hp = value; }
    }


    /// <summary>
    /// ここからダメージを与える
    /// </summary>
    /// <param name="damage">ダメージ量</param>
    /// <param name="unit">ダメージを与てくるUnit</param>
    public virtual void Damage(int damage, Unit unit)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Die();
        }
        print("damage");
    }

    public virtual void Damage(int damage, Actor unit)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Die();
        }
        print("damage");
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
