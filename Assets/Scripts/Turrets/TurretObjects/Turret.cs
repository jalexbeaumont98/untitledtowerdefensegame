using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Turret
{
    public string name;
    public int price;
    public int sellPrice;
    public string prefab;
    public string tile;
    public string description;
    public bool unlocked;


    public List<TurretUpgrade> upgradePath1;
    public List<TurretUpgrade> upgradePath2;
}
