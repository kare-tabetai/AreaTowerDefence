using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBullet : Actor {
    [SerializeField, Tooltip("ブレットの飛び上がる最高点")]
    float bulletYMax = 5f;
    [SerializeField]
    int damage = 100;

    public void Initialize(int playerNum, Transform target)
    {
        base.Initialize(playerNum);
        GetComponent<Rigidbody>().velocity = OrbitCalculations(transform.position,target.position, bulletYMax);
    }

    void Start () {
		
	}
	
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Stage"
            && other.tag != "Actor") { return; }
        if (other.tag == "Stage")
        {
            Destroy(gameObject);
            return;
        }
        var hpActor = other.GetComponent<HpActor>();
        if (hpActor == null) { return; }
        if (hpActor.PlayerNumber == PlayerNumber) { return; }

        hpActor.Damage(damage, this);
        Destroy(gameObject);
    }

    /// <summary>
    /// 弾の軌道を計算する
    /// </summary>
    /// <param name="targetPosition">あてる敵</param>
    /// <returns>撃つ距離とvelocity</returns>
    static Vector3 OrbitCalculations(Vector3 selfPosition,Vector3 targetPosition,float yMax)
    {
        float gravity = -Physics.gravity.y;
        Vector3 targetVec = targetPosition - selfPosition;
        Vector3 horizontalVec = new Vector3(targetPosition.x, 0, targetPosition.z) - new Vector3(selfPosition.x, 0, selfPosition.z);

        if (yMax < targetVec.y)
        {
            Debug.LogAssertion("MagicBulletの高さが低くて到達できません");
            return Vector3.zero;
        }

        Vector2 v;
        v.y = Mathf.Sqrt(2 * gravity * yMax);//√2gy
        v.x = GetvX(horizontalVec.magnitude, targetVec.y, gravity, v.y);

        return horizontalVec.normalized * v.x + new Vector3(0, v.y, 0);
    }
    /// <summary>
    /// 上方向の速度から横方向の速度を求める
    /// </summary>
    static float GetvX(float x, float y, float g, float vY)
    {
        var result = QuadraticEquation(y, -vY * x, 0.5f * g * x * x);

        if (result.Length == 1) { return result[0]; }
        if (y < 0) return Mathf.Max(result[0], result[1]);
        else return Mathf.Min(result[0], result[1]);
    }
    /// <summary>
    /// ax^2+bx+cを求める
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <returns>答えは二つ</returns>
    static float[] QuadraticEquation(float a, float b, float c)
    {
        float[] answer;

        if (a == 0)
        {
            answer = new float[1];
            answer[0] = -c / b;
            return answer;
        }

        answer = new float[2];
        float topFormula = -b + Mathf.Sqrt(b * b - 4 * a * c);//-b+root(b^2-4ac)/2a
        float bottomFormula = 2 * a;
        answer[0] = topFormula / bottomFormula;

        topFormula = -b - Mathf.Sqrt(b * b - 4 * a * c);//-b-root(b^2-4ac)/2a
        answer[1] = topFormula / bottomFormula;
        return answer;
    }

    
}
