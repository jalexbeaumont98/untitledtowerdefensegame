using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class TowerPlaceData
{

    public string name;
    public int price;
    public GameObject prefab;
    public Tile tile;
    public int sellPrice;
    public string description;

    public List<TurretUpgradeData> upgradesA;
    public List<TurretUpgradeData> upgradesB;

    public TowerPlaceData()
    {
    }

    //todo add bullet

    public TowerPlaceData (string _name, int _price, GameObject _prefab, Tile _tile, int _sellPrice, string _description, List<TurretUpgradeData> _upgradesA, List<TurretUpgradeData> _upgradesB)
    {
        name = _name;
        price = _price;
        prefab = _prefab;
        tile = _tile;
        sellPrice = _sellPrice;
        description = _description; 

        upgradesA = _upgradesA;
        upgradesB = _upgradesB;
        
    }

    public TowerPlaceData CreateImmutableCopy()
    {
        return new TowerPlaceData()
        {
            name = this.name,
            price = this.price,
            prefab = this.prefab,
            tile = this.tile,
            sellPrice = this.sellPrice,
            description = this.description,
            upgradesA = this.upgradesA,
            upgradesB = this.upgradesB

        };
    }

    
}
