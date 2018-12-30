using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Prefabを持たせたいけどシリアライズできない場合はここに置いておいて参照する
/// </summary>
public class ResourceManager : MonoSingleton<ResourceManager>
{
    [SerializeField]
    public GameObject MoveAllowTopPrefab;
    [SerializeField]
    public GameObject MoveAllowBodyPrefab;

	void Start ()
	{
		
	}

	void Update ()
	{
		
	}
}
