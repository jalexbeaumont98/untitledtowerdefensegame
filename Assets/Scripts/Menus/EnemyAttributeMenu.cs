using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAttributeMenu : MonoBehaviour
{
    [Header("References")]

    [SerializeField] GameObject menu;
    [SerializeField] EnemyBase enemy;
    [SerializeField] Image enemyImage;
    [SerializeField] TextMeshProUGUI enemyNameText;
    [SerializeField] Slider healthBar;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] Toggle stealthIndicator;
    [SerializeField] Toggle flyingIndicator;



    public delegate void EnemyAttributeMenuEvent();

    public event EnemyAttributeMenuEvent CloseMenu;
    public event EnemyAttributeMenuEvent OpenMenu;

    public event EnemyAttributeMenuEvent ResetMenu;



    void Start()
    {

        EnemyClickHandler.Instance.onEnemySelected += SetMenuEnemyAttribute;

        //Event Code


        //tileClickHandler.onTileSelected += SetTurretAttributeMenu;

        //End Event Code
    }

    public void SetMenuEnemyAttribute(EnemyBase enemy)
    {
        EnableMenu();

        OpenMenu?.Invoke();

        if (this.enemy) enemy.onEnemySelectedChange -= HandleEnemyStatusChange;

        this.enemy = enemy;
        enemy.onEnemySelectedChange += HandleEnemyStatusChange;

        Dictionary<string, string> info = enemy.GetInfo();

        enemyNameText.text = info["name"];


        int health = 0, maxHealth = 0;


        try
        {

            health = Int32.Parse(info["health"]);

            maxHealth = Int32.Parse(info["maxHealth"]);



        }
        catch (FormatException)
        {
            Debug.Log($"Unable to parse");
        }

        healthBar.maxValue = maxHealth;
        healthBar.value = health;

        healthText.text = health + "/" + maxHealth + " HP";

        description.text = info["description"];

        stealthIndicator.isOn = enemy.GetStealth();
        flyingIndicator.isOn = enemy.GetFlying();


        SetEnemyImage(enemy.GetSprite(), enemy.GetStealth());



        //PrintDictionary(enemy.GetInfo());
    }

    void HandleEnemyStatusChange(int newHealth)
    {
        if (newHealth <= 0) ResetMenu?.Invoke();

        else
        {
            healthBar.value = enemy.GetHP();
            healthText.text = enemy.GetHP() + "/" + enemy.GetMaxHP() + " HP";

        }

    }

    void PrintDictionary(Dictionary<string, string> dictionary)
    {
        foreach (KeyValuePair<string, string> kvp in dictionary)
        {
            Debug.Log($"Key: {kvp.Key}, Value: {kvp.Value}");
        }
    }


    public void SetEnemyImage(Sprite sprite, bool isStealth)
    {
        if (enemyImage == null || sprite == null)
        {
            Debug.LogError("Image or Sprite is null!");
            return;
        }

        // Default size for 8x8 sprite
        float defaultWidth = 100f;
        float defaultHeight = 100f;

        // Get the sprite's size
        Vector2 spriteSize = sprite.rect.size;

        // Calculate scale factor
        float scaleFactorX = defaultWidth / 8f;
        float scaleFactorY = defaultHeight / 8f;

        // Adjust the image size based on the sprite size
        enemyImage.rectTransform.sizeDelta = new Vector2(
            spriteSize.x * scaleFactorX,
            spriteSize.y * scaleFactorY
        );

        // Set the sprite and apply native size
        enemyImage.sprite = sprite;
        if (isStealth)
        {

            Color color = enemyImage.color;
            // Set the alpha (transparency) value
            color.a = Mathf.Clamp01(0.8f); // Ensure transparency is between 0 and 1
            // Apply the new color
            enemyImage.color = color;
        }

        else
        {
            Color color = enemyImage.color;
            // Set the alpha (transparency) value
            color.a = 1; // Ensure transparency is 1
            // Apply the new color
            enemyImage.color = color;
        }

        enemyImage.SetNativeSize();
    }

    public void EnableMenu()
    {
        menu.SetActive(true);
    }

    public void DisableMenu()
    {
        menu.SetActive(false);
    }

    private void SetTurretAttributeMenu(TurretBase turret)
    {
        EnableMenu();

        OpenMenu?.Invoke();
    }

}
