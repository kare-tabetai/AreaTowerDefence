using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiActorController : ActorController
{
    [SerializeField,Tooltip("生成するアクター,添え字の順に優先して呼ばれる")]
    GameObject[] actors;

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
            InstantiateActor();
        }
	}

    void InstantiateActor()
    {
        const float OffsetY = 0.05f;//重なりを防ぐため

        foreach (var actor in actors)
        {
            var actorComponent = actor.GetComponent<Actor>();
            if (costCounter.Pay(actorComponent.InstantiateCost))
            {
                var instantiatedObject = Instantiate(actor);
                var pos = OwnTower.position;
                pos.y = OffsetY;
                instantiatedObject.transform.position = pos;
                return;
            }
        }
    }
}
