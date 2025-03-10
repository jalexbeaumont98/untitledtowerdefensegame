using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class TurretUpgradeData
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

    public GameObject newSprite;
    public Tile newTile;

    public GameObject newBullet;

    public string description;

    
    
    public TurretUpgradeData(int upgradePath, int price, float fireRate, float rotationSpeed, float targetingRange, bool isNewSprite, bool isNewTile, bool isNewBullet, bool canTargetStealth, bool canTargetFlying, GameObject newSprite, Tile newTile, GameObject newBullet, string description)
    {
        this.upgradePath = upgradePath;

        this.price = price;

        this.fireRate = fireRate;
        this.rotationSpeed = rotationSpeed; 
        this.targetingRange = targetingRange;

        this.newSprite = newSprite;
        this.newTile = newTile;
        this.isNewBullet = newBullet;

        this.canTargetStealth = canTargetStealth;
        this.canTargetFlying = canTargetFlying;

        this.newBullet = newBullet;

        this.isNewSprite = isNewSprite;
        this.isNewTile = isNewTile;
        this.isNewBullet = newBullet;
        
        this.description = description;


    }
    
}
