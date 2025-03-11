using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;



public class TowerMenuHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameState gameState;
    [SerializeField] TextMeshProUGUI currencyUI;
    [SerializeField] Animator anim;
    [SerializeField] TextMeshProUGUI closeOpenButtonText;

    [SerializeField] GameObject closeOpenButton;
    [SerializeField] GameObject TowerShopUIPrefab;
    [SerializeField] RectTransform towerShopContent;

    [SerializeField] List<GameObject> towerShopUIPrefabs;

    [SerializeField] TextMeshProUGUI deselectButton;

    private bool isMenuOpen = false;

    private void Start()
    {
 

        gameState = GameState.Instance;
        anim = GetComponent<Animator>();

        EventHandler.Instance.OnRightClickEvent += HandleRightClick;
        //GameState.Instance.OnTowerListUpdated += OpenMenu;
        GameState.Instance.OnTowerListUpdated += SetTowerList;

        EventHandler.Instance.OnInputToggledEvent += SetDeselectButton;

        EventHandler.Instance.StartGameEvent += ToggleMenuButtonVisible;
        EventHandler.Instance.OnCloseRoundPopupEvent += OpenMenu;
        
    }

    private void SetDeselectButton(int value)
    {
        if (value == 1)
        {
            deselectButton.text = "Deselect Turret";
            deselectButton.GetComponentInParent<Button>().interactable = true;
        }

        else if (value == 0)
        {
            deselectButton.text = "Select a Turret";
            deselectButton.GetComponentInParent<Button>().interactable = false;
        }
    }

    private void SetTowerList()
    {
        int index = 0;
        foreach (TowerPlaceData towerPlaceData in GameState.Instance.Towers)
        {
            // Instantiate the prefab and get the Button
            GameObject newTowerShopItem = Instantiate(TowerShopUIPrefab, towerShopContent);
            newTowerShopItem.GetComponent<TowerMenuItem>().SetTowerMenuButton(towerPlaceData, index);

            towerShopUIPrefabs.Add(newTowerShopItem);

            index++;
        }

        Vector2 newSizeDelta = towerShopContent.sizeDelta;
        newSizeDelta.y = (110 + 25) * towerShopUIPrefabs.Count;

        towerShopContent.sizeDelta = newSizeDelta;
        //towerShopContent.GetComponent<VerticalLayoutGroup>().spacing = -(newSizeDelta.y / 2);
    }

    public void ToggleMenu()
    {
        if (isMenuOpen) CloseMenu();

        else OpenMenu();

    }

    public void OpenMenu()
    {
        if (isMenuOpen) return;

        isMenuOpen = true;
        anim.SetBool("MenuOpen", isMenuOpen);

        SetMenuButtonText();
    }

    public void CloseMenu()
    {

        if (!isMenuOpen) return;

        isMenuOpen = false;
        anim.SetBool("MenuOpen", isMenuOpen);

        SetMenuButtonText();
    }

    private void OnGUI()
    {
        currencyUI.text = "$" + gameState.GetMoney().ToString();
    }

    public void SetSelected()
    {

    }

    private void HandleRightClick(int value)
    {

        if (value != 9) return;

        if (isMenuOpen) ToggleMenu();

        else EventHandler.Instance.InvokeRightClickEvent(8);
    }

    private void SetMenuButtonText()
    {
        if (isMenuOpen) closeOpenButtonText.text = "X";
        else closeOpenButtonText.text = "<";
    }

    private void ToggleMenuButtonVisible()
    {
        closeOpenButton.SetActive(!closeOpenButton.gameObject.activeSelf);

    }
}
