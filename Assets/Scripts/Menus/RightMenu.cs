using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RightMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Animator anim;
    [SerializeField] TextMeshProUGUI closeOpenButtonText;

    [SerializeField] GameObject defaultMenu;
    [SerializeField] TurretAttributeMenu turretAttributeMenu;
    [SerializeField] EnemyAttributeMenu enemyAttributeMenu;
    [SerializeField] GameObject closeOpenButton;
    


    private bool isMenuOpen = false;


    void Start()
    {
        ResetMenu();
        //Event Code

        EventHandler.Instance.OnRightClickEvent += HandleRightClick;

        turretAttributeMenu.OpenMenu += () => OpenMenu();
        turretAttributeMenu.OpenMenu += () => SetMenuTurretAttribute();
        turretAttributeMenu.CloseMenu += () => CloseMenu();

        enemyAttributeMenu.OpenMenu += () => OpenMenu();
        enemyAttributeMenu.OpenMenu += () => SetMenuEnemyAttribute();
        enemyAttributeMenu.CloseMenu += () => CloseMenu();
        enemyAttributeMenu.ResetMenu += () => ResetMenu();

        EventHandler.Instance.StartGameEvent += ToggleMenuButtonVisible;

        //End Event Code
    }



    public void SetMenuTurretAttribute() {

       
        defaultMenu.SetActive(false);
        enemyAttributeMenu.DisableMenu();

        turretAttributeMenu.EnableMenu();
    }

    public void SetMenuEnemyAttribute() {

        defaultMenu.SetActive(false);
        turretAttributeMenu.DisableMenu();

        enemyAttributeMenu.EnableMenu();

        
    }

    public void ResetMenu()
    {
        defaultMenu.SetActive(true);

        turretAttributeMenu.DisableMenu();
        enemyAttributeMenu.DisableMenu();

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

        EventHandler.Instance.InvokeDeselectTurretEvent();

        EventHandler.Instance.InvokeDeselectEnemyEvent();

    }

    public void ToggleMenu()
    {
        if (isMenuOpen) CloseMenu();

        else OpenMenu();

    }

    private void SetMenuButtonText()
    {
        if (isMenuOpen) closeOpenButtonText.text = "X";
        else closeOpenButtonText.text = "<";
    }

    private void HandleRightClick(int value)
    {

        if (value != 10) return;

        if (isMenuOpen) CloseMenu();

        else if (!isMenuOpen) EventHandler.Instance.InvokeRightClickEvent(9);
    }

    private void ToggleMenuButtonVisible()
    {
        closeOpenButton.SetActive(!closeOpenButton.gameObject.activeSelf);

    }


}
