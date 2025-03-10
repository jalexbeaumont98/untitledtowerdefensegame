using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerMenuItem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TextMeshProUGUI towerNameText;
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] UnityEngine.UI.Image towerTile;
    [SerializeField] UnityEngine.UI.Image towerTurret;
    [SerializeField] Button towerItemButton;

    [Header("Attriubute")]
    [SerializeField] TowerPlaceData towerData;

    private int index;



    public void SetTowerMenuButton(TowerPlaceData towerData, int index)
    {
        this.index = index;

        this.towerData = towerData;

        towerNameText.text = towerData.name;
        if (towerData.price == 0) priceText.gameObject.SetActive(false);
        else priceText.text = "$ " + towerData.price;

        towerTile.sprite = towerData.tile.sprite;
        towerTile.SetNativeSize();

        if (towerData.prefab != null) {
            towerTurret.sprite = towerData.prefab.GetComponentInChildren<SpriteRenderer>().sprite;
            towerTurret.SetNativeSize();
        }
        
        else towerTurret.gameObject.SetActive(false);

        towerItemButton.onClick.AddListener(TaskOnClick);

    }

    void TaskOnClick()
    {
        
        TileClickHandler.Instance.SetCurrentPlacement(index);
        InputHandler.Instance.ChangeInputType(1);
        Debug.Log("You have clicked the button!");
    }

}
