using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyData
{
    public string name;
    public GameObject prefab;

    public EnemyData(string name, GameObject prefab)
    {
        this.name = name; 
        this.prefab = prefab;
    }
}
