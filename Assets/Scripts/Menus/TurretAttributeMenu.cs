using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurretAttributeMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameState gameState;
    [SerializeField] private TileClickHandler tileClickHandler;
    [SerializeField] TextMeshProUGUI TowerNameText;
    [SerializeField] TextMeshProUGUI TowerSellPriceText;
    [SerializeField] GridLayoutGroup upgradeGroup;
    [SerializeField] VerticalLayoutGroup vUpgradeGroup;
    [SerializeField] Animator anim;
    [SerializeField] TextMeshProUGUI closeOpenButtonText;
    [SerializeField] TMP_Dropdown targettingDropdown;
    [SerializeField] Toggle PrioritizeStealthToggle, PrioritizeFlyingToggle;

    [SerializeField] GameObject upgradeMenuPrefab;

    [SerializeField] RectTransform content;

    [SerializeField] GameObject menu;
    [SerializeField] TextMeshProUGUI descriptionText;



    private TurretBase turret;
    private TurretUpgradeMenu upgradeA;
    private TurretUpgradeMenu upgradeB;



    public delegate void TurretAttributeMenuEvent();

    public event TurretAttributeMenuEvent CloseMenu;
    public event TurretAttributeMenuEvent OpenMenu;


    void Start()
    {
        gameState = GameState.Instance;
        tileClickHandler = FindObjectOfType<TileClickHandler>();
        anim = GetComponent<Animator>();

        //Event Code


        tileClickHandler.onTileSelected += SetTurretAttributeMenu;

        //End Event Code
    }

    public void EnableMenu() {
        menu.SetActive(true);
    }

    public void DisableMenu() {
        menu.SetActive(false);
    }

    private void SetTurretAttributeMenu(TurretBase turret)
    {
        EnableMenu();

        OpenMenu?.Invoke();
        //todo menu animation code

        this.turret = turret;

        descriptionText.text = turret.towerData.description;

        targettingDropdown.value = turret.GetTargettingType();

        PrioritizeStealthToggle.isOn = turret.GetPrioritizeStealth();
        PrioritizeStealthToggle.interactable = turret.GetStealth();


        PrioritizeFlyingToggle.isOn = turret.GetPrioritizeFlying();
        PrioritizeFlyingToggle.interactable = turret.GetFlying();
 
        //Debug.Log(turret.towerData.name);

        TowerNameText.text = turret.towerData.name;

        TowerSellPriceText.text = "$" + turret.GetTotalSellPrice();


        if (upgradeA == null) upgradeA = Instantiate(upgradeMenuPrefab, vUpgradeGroup.transform).GetComponent<TurretUpgradeMenu>();
        upgradeA.SetUpgradeMenu(turret, 0);

        upgradeA.onTurretUpgraded -= SetTurretAttributeMenu;
        upgradeA.onTurretUpgraded += SetTurretAttributeMenu; //check if theres already a listener first

        if (upgradeB == null) upgradeB = Instantiate(upgradeMenuPrefab, vUpgradeGroup.transform).GetComponent<TurretUpgradeMenu>();
        upgradeB.SetUpgradeMenu(turret, 1);

        upgradeB.onTurretUpgraded -= SetTurretAttributeMenu;
        upgradeB.onTurretUpgraded += SetTurretAttributeMenu; //same here

        SetContentSize();

    }

    private void SetContentSize() {
        Vector2 sizeDelta = content.sizeDelta; // Get the current sizeDelta
        float newHeight = 225 + upgradeA.gameObject.GetComponent<RectTransform>().sizeDelta.y + upgradeB.gameObject.GetComponent<RectTransform>().sizeDelta.y;
        sizeDelta.y = newHeight; // Modify the height
        content.sizeDelta = sizeDelta; // Apply the new sizeDelta
    }


    public void SetTurretTargettingType()
    {
        if (turret == null) return;

        turret.SetTargettingType(targettingDropdown.value);
    }


    public void SellTurret()
    {
        if (turret == null) return;

        tileClickHandler.SellTile(turret.gridPos);

        CloseMenu?.Invoke();

    }


    public void ToggleStealth()
    {
        turret.SetPrioritizeStealth(PrioritizeStealthToggle.isOn);
    }

    public void ToggleFlying()
    {
        turret.SetPrioritizeFlying(PrioritizeFlyingToggle.isOn);
    }







}
