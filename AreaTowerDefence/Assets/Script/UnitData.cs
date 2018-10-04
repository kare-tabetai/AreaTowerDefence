using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyGame/Create UnitData", fileName = "UnitData")]
public class UnitData : ScriptableObject {
    [SerializeField,Tooltip("表示するユニットの名前")]
    public string UnitName;
    [SerializeField,Tooltip("実際に生成するオブジェクト")]
    public GameObject InstantiateUnit;
    [SerializeField, Tooltip("ドラッグ中に表示するオブジェクト")]
    public GameObject DraggingUnit;
}
