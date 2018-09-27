using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 作成したプレイヤーごとに必要なものはこれを継承
/// </summary>
public abstract class Actor : MonoBehaviour {
    [SerializeField]
    public int PlayerNum = 0;
}
