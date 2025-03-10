using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;

public class TurretBase : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Transform turretRotationField;
    [SerializeField] protected BarrelBase barrel;
    [SerializeField] protected LayerMask enemyMask;
    [SerializeField] public GameObject bullet; //todo change back to protected.
    [SerializeField] protected Transform firePoint;
    [SerializeField] public TowerPlaceData towerData;


    [Header("Attribute")]
    [SerializeField] protected bool bulletAutoAim;
    [SerializeField] protected int targetingType = 0;
    [SerializeField] protected float targetingAngleRange = 5f;
    [SerializeField] protected float leadingRange;
    [SerializeField] protected bool canTargetStealth;
    [SerializeField] protected bool prioritizeStealth;
    [SerializeField] protected bool canTargetFlying;
    [SerializeField] protected bool prioritizeFlying;
    [SerializeField] private int sellPrice = 0;
    [SerializeField] private int tileSellPrice = 0;
    [SerializeField] public Vector3Int gridPos;

    //upgrade info
    [SerializeField] protected string upgradeInfoPath;
    [SerializeField] protected int upgradeAPathCount;
    [SerializeField] protected int upgradeBPathCount;
    [SerializeField] protected bool finalTile;
    [SerializeField] protected bool finalTurret;

    [SerializeField] protected float fireRate = 1f;
    [SerializeField] protected float rotationSpeed = 5f;
    [SerializeField] protected float targetingRange = 3f;

    [SerializeField]
    private enum TargetingTypes
    {
        first = 0,
        closest = 1,
        farthest = 2,
        strongest = 3,
        weakest = 4
    }

    public Transform target;
    private float timeUntilFire;

    private Quaternion targetRotation;
    private bool isWithinTargettingAngleRange = false, updateTarget = false;

    private GameObject unitCircle, towerHighlight;


    protected virtual void Update()
    {

        if (updateTarget)
        {
            target = null;
            updateTarget = false;
        }
        
        if (target == null)
        {
            FindTarget();
            return;
        }

        if (target != null)
        {

            RotateTowardsTarget();

            timeUntilFire += Time.deltaTime;

            if (timeUntilFire >= 1f / fireRate)
            {
                if (isWithinTargettingAngleRange)
                {
                    Shoot();
                    FindTarget();
                }

                timeUntilFire = 0;
            }
        }

        if (!CheckTargetIsInRange()) target = null;

    }

    protected virtual void FindTarget()
    {

        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, (Vector2)transform.position, 0f, enemyMask);

        if (hits.Length <= 0) return;

        if (prioritizeStealth)
        {
            RaycastHit2D[] stealthHits = PrioritizeStealth(hits);
            if (stealthHits.Length > 0) hits = stealthHits;
        }

        if (prioritizeFlying)
        {
            RaycastHit2D[] flyingHits = PrioritizeFlying(hits);
            if (flyingHits.Length > 0) hits = flyingHits;
        }

        switch (targetingType)
        {
            case 0: //first
                target = GetFirst(hits);
                break;
            case 1: //closest
                target = GetClosest(hits);
                break;
            case 2: //farthest
                target = GetFarthest(hits);
                break;
            case 3: //strongest
                target = GetStrongest(hits);
                break;
            case 4: //weakest
                target = GetWeakest(hits);
                break;
            default:
                break;

        }


    }

    protected virtual bool CheckTargetIsInRange()
    {
        return Vector2.Distance(target.position, transform.position) <= targetingRange;
    }

    protected virtual bool EnsureTargetCanBeTargeted(Transform targetedEnemy)
    {
        if (canTargetFlying && canTargetStealth) return true;

        try
        {
            if (targetedEnemy.GetComponent<EnemyBase>().GetStealth())
            {
                if (!canTargetStealth) return false;
            }

            if (targetedEnemy.GetComponent<EnemyBase>().GetFlying())
            {
                if (!canTargetFlying) return false;
            }

            return true;
        }

        catch (Exception e)
        {
            Debug.Log("While checking if an enemy could be targeted its enemy controller was null.");
            Debug.Log(e);
        }

        return false;

    }

    protected RaycastHit2D[] PrioritizeStealth(RaycastHit2D[] hits)
    {

        List<RaycastHit2D> tempHits = new List<RaycastHit2D>();

        foreach (RaycastHit2D hit in hits)
        {
            try
            {
                if (hit.transform.GetComponent<EnemyBase>().GetStealth()) tempHits.Add(hit);

            }

            catch (Exception e)
            {
                Debug.Log("While checking if an enemy should be prioritized as stealth -> null enemycontroller.");
                Debug.Log(e);
            }

        }

        if (tempHits.Count <= 0) return Array.Empty<RaycastHit2D>(); ;

        RaycastHit2D[] newList = new RaycastHit2D[tempHits.Count];

        for (int i = 0; i < newList.Length; i++)
        {
            newList[i] = tempHits[i];
        }

        return newList;

    }

    protected RaycastHit2D[] PrioritizeFlying(RaycastHit2D[] hits)
    {
        List<RaycastHit2D> tempHits = new List<RaycastHit2D>();

        foreach (RaycastHit2D hit in hits)
        {
            try
            {
                if (hit.transform.GetComponent<EnemyBase>().GetFlying()) tempHits.Add(hit);

            }

            catch (Exception e)
            {
                Debug.Log("While checking if an enemy should be prioritized as flying -> null enemycontroller.");
                Debug.Log(e);
            }

        }

        if (tempHits.Count <= 0) return Array.Empty<RaycastHit2D>();

        RaycastHit2D[] newList = new RaycastHit2D[tempHits.Count];

        for (int i = 0; i < newList.Length; i++)
        {
            newList[i] = tempHits[i];
        }

        return newList;

    }

    protected virtual void RotateTowardsTarget()
    {

        float angle;

        /*

        if (Vector2.Distance(target.position, transform.position) <= leadingRange) angle = Mathf.Atan2((target.position.y + (targetAgent.velocity.y)) - transform.position.y, (target.position.x + (targetAgent.velocity.x)) - transform.position.x) * Mathf.Rad2Deg + 90f;

        else angle = Mathf.Atan2((target.position.y) - transform.position.y, (target.position.x) - transform.position.x) * Mathf.Rad2Deg + 90f;

        */

        angle = Mathf.Atan2((target.position.y) - transform.position.y, (target.position.x) - transform.position.x) * Mathf.Rad2Deg + 90f;

        targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        turretRotationField.rotation = Quaternion.RotateTowards(turretRotationField.rotation, targetRotation, rotationSpeed * Time.deltaTime);



        if (Mathf.Abs(targetRotation.z - turretRotationField.transform.rotation.z) * Mathf.Rad2Deg < targetingAngleRange)
        {
            isWithinTargettingAngleRange = true;
        }

        else isWithinTargettingAngleRange = false;


    }

    protected virtual void Shoot()
    {
        barrel.Shoot();

    }

    public virtual void ShootFromBarrel(Transform fp)
    {
        if (!target) return;

        GameObject _bullet = Instantiate(bullet, fp.position, turretRotationField.rotation);
        ProjectileBase bulletCon = _bullet.GetComponent<ProjectileBase>();

        bulletCon.SetTarget(target, -firePoint.up);
    }


    public virtual void SetPreviewMode()
    {
        if (barrel != null) barrel.SetPreview();

        this.enabled = false;
    }



    public virtual void SellSelf()
    {

        print("turret sold for " + sellPrice);
    }

    public virtual void DestroySelf()
    {
        SellSelf();
        Destroy(gameObject);
    }

    public virtual int GetTotalSellPrice()
    {
        return sellPrice + tileSellPrice;
    }

    public virtual int GetRealSellPrice()
    {
        return sellPrice;
    }

    public virtual void SetTileSellPrice(int price)
    {
        tileSellPrice = price;
    }

    public virtual void SetSelected()
    {

        SpawnRangeCircle();

        SpawnHighlight();

    }

    private void SpawnRangeCircle()
    {
        if (unitCircle) Destroy(unitCircle);

        unitCircle = Instantiate(GameState.Instance.rangeCirclePrefab, transform);

        SpriteRenderer spriteRenderer = unitCircle.GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("Range circle does not have a SpriteRenderer component.");
            return;
        }

        // Adjust scale to match the turret's range
        float scale = targetingRange * 2; // Multiply by 2 because range is the radius, and we scale by diameter
        this.unitCircle.transform.localScale = new Vector3(scale, scale, 1);
    }

    private void SpawnHighlight()
    {
        this.towerHighlight = Instantiate(GameState.Instance.towerHighlightPrefab, transform);

    }

    public virtual void DeselectTurret()
    {
        Destroy(towerHighlight);
        Destroy(unitCircle);

    }


    protected virtual Transform GetFirst(RaycastHit2D[] hits)
    {

        foreach (RaycastHit2D hit in hits)
        {
            if (EnsureTargetCanBeTargeted(hit.transform)) return hit.transform;
        }

        return null;
    }


    protected virtual Transform GetClosest(RaycastHit2D[] hits)
    {
        //print("GetClosest");

        float closestDistance = Vector2.Distance(hits[0].transform.position, firePoint.position);
        Transform closestTransform = hits[0].transform;

        foreach (RaycastHit2D hit in hits)
        {
            if (Vector2.Distance(hit.transform.position, firePoint.position) < closestDistance)
            {

                if (EnsureTargetCanBeTargeted(hit.transform))
                {
                    closestDistance = Vector2.Distance(hit.transform.position, firePoint.position);
                    closestTransform = hit.transform;
                }

            }
        }

        if (EnsureTargetCanBeTargeted(closestTransform)) return closestTransform;
        else return null;

    }

    protected virtual Transform GetFarthest(RaycastHit2D[] hits)
    {
        //print("GetFarthest");

        float farthestDistance = Vector2.Distance(hits[0].transform.position, firePoint.position);
        Transform farthestTransform = hits[0].transform;

        foreach (RaycastHit2D hit in hits)
        {
            if (Vector2.Distance(hit.transform.position, transform.position) > farthestDistance)
            {

                if (EnsureTargetCanBeTargeted(hit.transform))
                {
                    farthestDistance = Vector2.Distance(hit.transform.position, firePoint.position);
                    farthestTransform = hit.transform;
                }


            }
        }

        if (EnsureTargetCanBeTargeted(farthestTransform)) return farthestTransform;
        return null;

    }

    protected virtual Transform GetStrongest(RaycastHit2D[] hits)
    {
        //print("GetStrongest");

        Transform strongest = hits[0].transform;
        float mostHP = hits[0].transform.GetComponent<EnemyBase>().GetHP();

        foreach (RaycastHit2D hit in hits)
        {
            try
            {
                if (hit.transform.GetComponent<EnemyBase>().GetHP() > mostHP)
                {
                    if (EnsureTargetCanBeTargeted(hit.transform)) strongest = hit.transform;

                }
            }

            catch (Exception e)
            {
                Debug.Log("A potential turret target didn't have an enemybase script");
                Debug.Log(e);
            }


        }
        if (EnsureTargetCanBeTargeted(strongest)) return strongest;
        return null;

    }

    protected virtual Transform GetWeakest(RaycastHit2D[] hits)
    {
        //print("GetWeakest");

        Transform weakest = hits[0].transform;
        float leastHP = hits[0].transform.GetComponent<EnemyBase>().GetHP();

        foreach (RaycastHit2D hit in hits)
        {
            try
            {
                if (hit.transform.GetComponent<EnemyBase>().GetHP() < leastHP)
                {
                    if (EnsureTargetCanBeTargeted(hit.transform)) weakest = hit.transform;
                }
            }

            catch (Exception e)
            {
                Debug.Log("A potential turret target didn't have an enemybase script");
                Debug.Log(e);
            }


        }

        if (EnsureTargetCanBeTargeted(weakest)) return weakest;
        return null;
    }

    public void SetTargettingType(int type)
    {
        targetingType = type;
        UpdateTarget();
    }

    public int GetTargettingType()
    {
        return targetingType;
    }

    public virtual bool GetStealth()
    {
        return canTargetStealth;
    }

    public virtual bool GetFlying()
    {
        return canTargetFlying;
    }

    public virtual int[] GetTurretUpgradeInfo()
    {
        return new int[] { upgradeAPathCount, upgradeBPathCount };
    }


    public virtual bool HasReachedMaximumUpgradesAtPath(int path)
    {

        if (path == 0)
        {
            if (upgradeBPathCount > 1 && upgradeAPathCount > 0) return true;


        }

        if (path == 1)
        {
            if (upgradeAPathCount > 1 && upgradeBPathCount > 0) return true;
        }

        return false;
    }

    private bool HasReachedMaximumUpgradesStandalone(int path)
    {
        if (path == 0)
        {
            if (upgradeAPathCount > 2) return true;
            return false;
        }

        if (path == 1)
        {
            if (upgradeBPathCount > 2) return true;
            return false;
        }

        return false;

    }


    public virtual void UpgradeTurret(TurretUpgradeData upgrade)
    {
        if (upgrade.upgradePath == 0) upgradeAPathCount++;
        if (upgrade.upgradePath == 1) upgradeBPathCount++;

        sellPrice += upgrade.price * 1 / 3;

        if (upgrade.newBullet != null)
        {
            bullet = upgrade.newBullet;
        }

        if (upgrade.newSprite != null && !finalTurret)
        {
            Destroy(barrel.gameObject);

            barrel = Instantiate(upgrade.newSprite, turretRotationField).GetComponent<BarrelBase>();

            if (HasReachedMaximumUpgradesStandalone(upgrade.upgradePath)) finalTurret = true;
        }

        if (upgrade.newTile != null && !finalTile)
        {
            towerData.tile = upgrade.newTile;
            FindObjectOfType<TileClickHandler>().ReplaceTile(upgrade.newTile, gridPos);

            if (HasReachedMaximumUpgradesStandalone(upgrade.upgradePath)) finalTile = true;
        }

        if (upgrade.canTargetStealth) canTargetStealth = upgrade.canTargetStealth;

        if (upgrade.canTargetFlying) canTargetFlying = upgrade.canTargetFlying;

        fireRate += upgrade.fireRate;
        rotationSpeed += upgrade.rotationSpeed;
        targetingRange += upgrade.targetingRange;

        if (upgrade.targetingRange != 0) SpawnRangeCircle();


    }

    public void UpdateTarget()
    {
        updateTarget = true;
    }

    public virtual void SetPrioritizeStealth(bool input)
    {
        prioritizeStealth = input;
        UpdateTarget();
    }

    public virtual bool GetPrioritizeStealth()
    {
        return prioritizeStealth;
    }

    public virtual void SetPrioritizeFlying(bool input)
    {
        prioritizeFlying = input;
        UpdateTarget();
    }

    public virtual bool GetPrioritizeFlying()
    {
        return prioritizeFlying;
    }

    public virtual bool GetFinalTurret()
    {
        return finalTurret;

    }

    public virtual bool GetFinalTile()
    {
        return finalTile;
    }

    public virtual BarrelBase GetBarrel()
    {
        return barrel;
    }



}
