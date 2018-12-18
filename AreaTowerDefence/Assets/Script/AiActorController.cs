using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiActorController : ActorController
{
    [SerializeField,Tooltip("生成するアクター,添え字の順に優先して呼ばれる")]
    UnitData[] units;

    [SerializeField]
    float instantiateRug = 2f;

    float timer = 0;
	void Start () {
		
	}
	
	void Update () {
        timer += Time.deltaTime;
        if (instantiateRug <= timer)
        {
            timer = 0f;
            InstantiateUnit();
        }
	}

    void InstantiateUnit()
    {
        const float OffsetY = 0.05f;//重なりを防ぐため

        foreach (var unit in units)
        {
            if (costCounter.Pay(unit.InstantiateCost))
            {
                var instantiatedObject = Instantiate(unit.InstantiateUnit, MainSceneManager.Instance.ActorNode);
                var pos = OwnTower.position;
                pos.y = OffsetY;
                instantiatedObject.transform.position = pos;
                instantiatedObject.GetComponent<Unit>().Initialize(PlayerNumber);
                return;
            }
        }
    }
}
