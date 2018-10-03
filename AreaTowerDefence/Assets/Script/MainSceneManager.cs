using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneManager : MonoSingleton<MainSceneManager>
{
    [SerializeField]
    ActorController[] controllers;
    public ActorController[] Controllers
    {
        get { return controllers; }
        private set { controllers = value; }
    }
    [SerializeField]
    Transform actoresNode;
    public Transform Actornode
    {
        get { return actoresNode; }
    }

	void Start () {
        Debug.Assert(2 <= controllers.Length);
        for(int i = 0; i < Controllers.Length; i++)
        {
            Debug.Assert(Controllers[i].PlayerNumber == i);
        }
    }

    void Update () {
        if (Input.GetButtonDown("Escape"))
        {
            Application.Quit();
        }
    }

    public Transform GetNearestOtherPlayerTower(int playerNum,Vector3 comparedPosition)
    {
        Transform nearestTower = null;
        float nearestSqrDist = Mathf.Infinity;
        for(int pNum = 0; pNum < controllers.Length; pNum++)
        {
            if (pNum == playerNum) { continue; }
            float sqrDist = Vector3.SqrMagnitude(controllers[pNum].OwnTower.position - comparedPosition);
            if (sqrDist < nearestSqrDist)
            {
                nearestSqrDist = sqrDist;
                nearestTower = controllers[pNum].OwnTower;
            }
        }
        Debug.Assert(nearestTower != null);
        return nearestTower;
    }
}
