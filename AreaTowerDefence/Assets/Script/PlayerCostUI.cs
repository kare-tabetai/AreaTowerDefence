using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerCostUI : MonoBehaviour {

    [SerializeField]
    CostCounter counter;
    TextMeshProUGUI text;
	void Start () {
        text = GetComponent<TextMeshProUGUI>();
        text.text = counter.Count.ToString();
	}
	
	void Update () {
        text.text = counter.Count.ToString();
    }
}
