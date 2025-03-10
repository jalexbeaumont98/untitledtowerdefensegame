using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TurretUpgrade
{
    public int upgradePath;

    public int price;

    public float fireRate;
    public float rotationSpeed;
    public float targetingRange;

    public bool isNewSprite;
    public bool isNewTile;
    public bool isNewBullet;

    public bool canTargetStealth;
    public bool canTargetFlying;

    public string newSprite;
    public string newTile;

    public string newBullet;

    public string description;

}
