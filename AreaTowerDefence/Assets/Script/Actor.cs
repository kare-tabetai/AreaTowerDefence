using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 動くコマを作るときはこれを継承
/// </summary>
public abstract class Actor : MonoBehaviour {
    [SerializeField]
    public int PlayerNum = 0;//getter,setterを使うとInspectorから見えないから使わん
}
