using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneManager : MonoSingleton<MainSceneManager>
{
    [SerializeField]
    public ActorController[] controllers;
    
	void Start () {

	}

	void Update () {
        if (Input.GetButtonDown("Escape"))
        {
            Application.Quit();
        }
    }

    public Transform GetNearestOtherPlayerTower(int playerNum,Vector3 comparedPosition)
    {
        Debug.Assert(controllers.Length != 0);
        Transform nearestTower = null;
        float sqrDist = float.PositiveInfinity;
        for(int pNum = 0; pNum < controllers.Length; pNum++)
        {
            if (pNum == playerNum) { continue; }
            float dist = Vector3.SqrMagnitude(controllers[pNum].OwnTower.position - comparedPosition);
            if (dist < sqrDist)
            {
                sqrDist = dist;
                nearestTower = controllers[pNum].OwnTower;
            }
        }
        Debug.Assert(nearestTower != null);
        return nearestTower;
    }
}
