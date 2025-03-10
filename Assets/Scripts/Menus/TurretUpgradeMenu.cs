using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class TurretUpgradeMenu : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private GameState gameState;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private GameObject turretObject;
    [SerializeField] private Image tileImage;
    [SerializeField] private Image turretImage;
    [SerializeField] private GameObject bulletObject;
    [SerializeField] private Image bulletImage;
    [SerializeField] private TextMeshProUGUI upgradeButtonText;
    [SerializeField] private TurretUpgradeData upgradeData;
    [SerializeField] private TurretBase turret;
    [SerializeField] private GameObject statBlock;
    [SerializeField] private GameObject statTextPrefab;
    [SerializeField] private RectTransform content;

    [Header("Attribute")]
    [SerializeField] private int upgradePath;
    [SerializeField] private int cost;
    [SerializeField] private int mode;


    public delegate void OnTurretUpgraded(TurretBase turret);

    public event OnTurretUpgraded onTurretUpgraded;


    private void Start()
    {
        gameState = GameState.Instance;


    }

    // Sets up the upgrade menu for a specific turret and upgrade path.
    public void SetUpgradeMenu(TurretBase turret, int upgradePath)
    {

        if (!turretObject.activeSelf) turretObject.SetActive(true);

        if (bulletObject.activeSelf) bulletObject.SetActive(false);

        mode = 0;


        // Determine the upgrade path string ("A" or "B") for display purposes.
        string upgradePathString;
        if (upgradePath == 0)
            upgradePathString = "A";
        else
            upgradePathString = "B";

        // Store the turret being upgraded and its path.
        this.turret = turret;
        this.upgradePath = upgradePath;

        int pathProgress; // Tracks the current progress in the selected upgrade path.

        // Enable the upgrade button initially; it may be disabled later if conditions aren't met.
        upgradeButton.enabled = true;

        // Destroy all old stat line text objects
        foreach (Transform child in statBlock.transform)
        {
            Destroy(child.gameObject); // Destroy the child GameObject
        }

        // Check if the turret has reached the maximum upgrades for the selected path.
        if (turret.HasReachedMaximumUpgradesAtPath(upgradePath) || turret.GetTurretUpgradeInfo()[upgradePath] >= 3)
        {

            GameObject newStatText = Instantiate(statTextPrefab, statBlock.transform);
            newStatText.GetComponent<TextMeshProUGUI>().text = "This path has reached the max amount of upgrades.";


            // Disable the upgrade menu for this path if upgrades are no longer possible.
            SetUpgradeMenuDisabled(upgradePathString);
            pathProgress = GameState.maxUpgrade;
            //return;
        }
        else
        {

            // Get the upgrade data for the selected path based on the current progress.
            if (upgradePath == 0)
                upgradeData = turret.towerData.upgradesA[turret.GetTurretUpgradeInfo()[upgradePath]];
            else
                upgradeData = turret.towerData.upgradesB[turret.GetTurretUpgradeInfo()[upgradePath]];

            pathProgress = turret.GetTurretUpgradeInfo()[upgradePath]; // Update the path progress.

            // Update the upgrade button text to reflect the next upgrade level.
            upgradeButtonText.text = "Upgrade " + upgradePathString + (pathProgress + 1) + " $" + upgradeData.price;

            SetUpgradeTextTurret();
        }



        try
        {
            // Set the tile image to the new tile's sprite if a new tile is unlocked.
            if (upgradeData.isNewTile && !turret.GetFinalTile())
            {
                print("first block tile");
                if (upgradeData.newTile.sprite != null)
                    tileImage.sprite = upgradeData.newTile.sprite;
            }
            else
            {
                // Otherwise, use the turret's default tile sprite.
                tileImage.sprite = turret.towerData.tile.sprite;
                print("second block tile");

            }

            tileImage.SetNativeSize();

            // Set the turret image to the new sprite if one is unlocked through the upgrade.
            if (upgradeData.isNewSprite && !turret.GetFinalTurret())
            {
                if (upgradeData.newSprite != null)
                    turretImage.sprite = upgradeData.newSprite.GetComponent<BarrelBase>().GetSprite();

                print("first block sprite");
            }
            else
            {
                // Otherwise, use the turret's current sprite.
                turretImage.sprite = turret.GetBarrel().GetSprite();
                print("second block sprite");
            }

            turretImage.SetNativeSize();

        }
        catch (Exception e)
        {
            // Log any exceptions that occur during the setup process.
            print(e);
        }





        // Display the cost of the next upgrade.




    }

    private void SetUpgradeTextTurret()
    {
        string statSign = "";

        if (upgradeData.fireRate != 0)
        {
            statSign = "";
            if (upgradeData.fireRate > 0) statSign = "+";

            GameObject newStatText = Instantiate(statTextPrefab, statBlock.transform);

            newStatText.GetComponent<TextMeshProUGUI>().text = "Firerate: " + statSign + upgradeData.fireRate.ToString();


        }

        if (upgradeData.rotationSpeed != 0)
        {

            statSign = "";
            if (upgradeData.rotationSpeed > 0) statSign = "+";

            GameObject newStatText = Instantiate(statTextPrefab, statBlock.transform);

            newStatText.GetComponent<TextMeshProUGUI>().text = "Rotation Speed: " + statSign + upgradeData.rotationSpeed.ToString();

        }

        if (upgradeData.targetingRange != 0)
        {

            statSign = "";
            if (upgradeData.targetingRange > 0) statSign = "+";

            GameObject newStatText = Instantiate(statTextPrefab, statBlock.transform);

            newStatText.GetComponent<TextMeshProUGUI>().text = "Targeting Range: " + statSign + upgradeData.targetingRange.ToString();

        }

        if (upgradeData.canTargetStealth && !turret.GetStealth())
        {

            GameObject newStatText = Instantiate(statTextPrefab, statBlock.transform);

            newStatText.GetComponent<TextMeshProUGUI>().text = "Can target Stealth enemies.";

        }

        if (upgradeData.canTargetFlying && !turret.GetFlying())
        {

            GameObject newStatText = Instantiate(statTextPrefab, statBlock.transform);

            newStatText.GetComponent<TextMeshProUGUI>().text = "Can target Flying enemies.";

        }

        // Store the cost of the current upgrade for later use.
        cost = upgradeData.price;

        if (statBlock.transform.childCount == 0)
        {
            Instantiate(statTextPrefab, statBlock.transform).GetComponent<TextMeshProUGUI>().text = "No changes to towers stats."; ;
            Instantiate(statTextPrefab, statBlock.transform).GetComponent<TextMeshProUGUI>().text = "See projectile stats."; ;

        }

        SetTextContentSize();
    }

    // Attempts to upgrade the turret when the player clicks the upgrade button.
    public void UpgradeTurret()
    {
        // Ensure a turret is selected before attempting the upgrade.
        if (turret != null)
        {
            // Attempt to deduct the upgrade cost from the player's currency.
            if (gameState.AttemptPurchase(cost))
            {
                // Apply the upgrade to the turret and invoke any listeners for the upgrade event.
                turret.UpgradeTurret(upgradeData);
                onTurretUpgraded?.Invoke(turret);
            }
        }
    }

    // Disables the upgrade menu and updates the button text to indicate the path is locked.
    private void SetUpgradeMenuDisabled(string path)
    {
        upgradeButton.enabled = false; // Disable the upgrade button.
        upgradeButtonText.text = "Path " + path + " locked"; // Display a locked message.
    }


    public void ToggleUpgradeStats()
    {
        if (mode == 1)
        {
            print("were not gaming :(");
            SetUpgradeMenu(turret, upgradePath);
        }

        else
        {
            print("we gaming");

            mode = 1;

            if (turretObject.activeSelf) turretObject.SetActive(false);

            if (!bulletObject.activeSelf) bulletObject.SetActive(true);

            int pathProgress; // Tracks the current progress in the selected upgrade path.

            print(MaxValue(upgradePath, 2));

            // Get the upgrade data for the selected path based on the current progress.
            if (upgradePath == 0)
                upgradeData = turret.towerData.upgradesA[MaxValue(turret.GetTurretUpgradeInfo()[upgradePath], 2)];
            else
                upgradeData = turret.towerData.upgradesB[MaxValue(turret.GetTurretUpgradeInfo()[upgradePath], 2)];

            pathProgress = turret.GetTurretUpgradeInfo()[upgradePath]; // Update the path progress.


            try
            {
                // Set the bullet image to the new bullet's sprite if a new bullet is unlocked.
                if (upgradeData.isNewBullet)
                {
                    if (upgradeData.newBullet.gameObject.GetComponentInChildren<SpriteRenderer>().sprite != null)
                        bulletImage.sprite = upgradeData.newBullet.gameObject.GetComponentInChildren<SpriteRenderer>().sprite;
                }
                else
                {
                    // Otherwise, use the turret's default bullet sprite.
                    bulletImage.sprite = turret.bullet.GetComponentInChildren<SpriteRenderer>().sprite;
                }

            }
            catch (Exception e)
            {
                // Log any exceptions that occur during the setup process.
                print(e);
            }




            // Destroy all old stat line text objects
            foreach (Transform child in statBlock.transform)
            {
                Destroy(child.gameObject); // Destroy the child GameObject
            }

            // Determine the upgrade path string ("A" or "B") for display purposes.
            string upgradePathString;
            if (upgradePath == 0)
                upgradePathString = "A";
            else
                upgradePathString = "B";

            if (turret.HasReachedMaximumUpgradesAtPath(upgradePath) || turret.GetTurretUpgradeInfo()[upgradePath] >= 3)
            {

                GameObject newStatText = Instantiate(statTextPrefab, statBlock.transform);
                newStatText.GetComponent<TextMeshProUGUI>().text = "This path has reached the max amount of upgrades.";

                // Disable the upgrade menu for this path if upgrades are no longer possible.
                SetUpgradeMenuDisabled(upgradePathString);
                return;
            }

            if (!upgradeData.isNewBullet)
            {
                GameObject newStatText = Instantiate(statTextPrefab, statBlock.transform);

                newStatText.GetComponent<TextMeshProUGUI>().text = "No changes to towers projectile.";

            }

            else
            {

                string statSign = "";


                float[] oldStats = turret.bullet.GetComponent<ProjectileBase>().GetStats();

                float[] newStats = upgradeData.newBullet.GetComponent<ProjectileBase>().GetStats();

                string upgradeNotes = upgradeData.newBullet.GetComponent<ProjectileBase>().GetUpgradeNotes();

                if (upgradeNotes != "")
                {
                    GameObject newStatText = Instantiate(statTextPrefab, statBlock.transform);
                    newStatText.GetComponent<TextMeshProUGUI>().text = upgradeNotes;
                }

                if (newStats[0] != oldStats[0])
                {
                    statSign = "";
                    if (newStats[0] > 0) statSign = "+";

                    GameObject newStatText = Instantiate(statTextPrefab, statBlock.transform);

                    newStatText.GetComponent<TextMeshProUGUI>().text = "Damage: " + statSign + newStats[0];

                }

                if (newStats[1] != oldStats[1])
                {
                    statSign = "";
                    if (newStats[1] > 0) statSign = "+";

                    GameObject newStatText = Instantiate(statTextPrefab, statBlock.transform);

                    newStatText.GetComponent<TextMeshProUGUI>().text = "Speed: " + statSign + newStats[1];


                }

                if (newStats[2] != oldStats[2])
                {
                    GameObject newStatText = Instantiate(statTextPrefab, statBlock.transform);

                    if (newStats[2] == 0)
                    {
                        newStatText.GetComponent<TextMeshProUGUI>().text = "Projectile no longer auto tracks.";
                    }

                    else if (newStats[2] == 1)
                    {
                        if (newStats[2] == 0)
                        {
                            newStatText.GetComponent<TextMeshProUGUI>().text = "Projectile will now auto track.";
                        }
                    }

                }

            }



            SetTextContentSize();




        }
    }

    private void SetTextContentSize()
    {
        Vector2 sizeDelta = content.sizeDelta; // Get the current sizeDelta
        float newHeight = statBlock.transform.childCount * content.GetComponent<GridLayoutGroup>().cellSize.y;
        sizeDelta.y = newHeight; // Modify the height
        content.sizeDelta = sizeDelta; // Apply the new sizeDelta
    }

    private int MaxValue(int value, int max)
    {
        if (value > max) return max;
        else return value;
    }







}
