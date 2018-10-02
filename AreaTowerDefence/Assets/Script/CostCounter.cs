using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unitの生成コストを管理するクラス
/// </summary>
public class CostCounter : MonoBehaviour {

    [SerializeField]
    float count = 100;
    public float Count
    {
        get { return count; }
        private set { count = value; }
    }

    [SerializeField]
    int interestRate = 1;
    public int InterestRate
    {
        get { return interestRate; }
        private set { interestRate = value; }
    }

    void Start () {
		
	}

    /// <summary>
    /// 支払えそうなら支払い,trueを
    /// 支払えなければ支払わず,falseを
    /// </summary>
    /// <param name="cost">支払う額</param>
    /// <returns>支払えればtrue</returns>
    public bool Pay(int cost)
    {
        if (count < cost)
        {
            print("予算不足");
            return false;
        }
        count -= cost;
        return true;
    }
	
	void Update () {
        count += Time.deltaTime * interestRate;
	}
}
