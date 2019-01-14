using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forest : MonoBehaviour
{
    [SerializeField]
    Vector3 size=new Vector3(1,0,1);
    [SerializeField]
    GameObject[] treePrefabs;
    [SerializeField]
    int createNum = 10;
    [SerializeField,Tooltip("スケールをばらさせたいときの範囲")]
    int scaleRundomRange = 0;
    [SerializeField]
    bool randomRotation = true;

	void Start ()
	{
	}

    [ContextMenu("CreateForest")]
    public void CreateForest()
    {
        for(int i = 0; i < createNum; i++)
        {
            var instPos = transform.position;
            instPos += transform.right * Random.Range(-size.x/2,size.x/2);
            instPos += transform.up * Random.Range(-size.y/2,size.y/2);
            instPos += transform.forward * Random.Range(-size.z/2,size.z/2);
            CreateTree(instPos);
        }
    }

    void CreateTree(Vector3 pos)
    {
        int treePrefabNum = Random.Range(0, treePrefabs.Length);
        var instPrefab = treePrefabs[treePrefabNum];
        var tree = Instantiate(instPrefab, transform);
        tree.transform.position = pos;
        tree.transform.up = transform.up;
        if (randomRotation)
        {
            tree.transform.Rotate(Vector3.up * Random.Range(0, 360));
        }
        var treeScale = tree.transform.localScale + Random.Range(-scaleRundomRange / 2, scaleRundomRange / 2) * Vector3.one;
        tree.transform.localScale = treeScale;
    }

	void Update ()
	{
		
	}
}
