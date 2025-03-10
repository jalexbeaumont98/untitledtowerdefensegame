using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;



public class TurretController : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Transform turretRotationField;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform firePoint;


    [Header("Attribute")]
    [SerializeField] private float targetingRange = 3f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private bool bulletAutoAim;
    [SerializeField] private float bulletSpeed;

    private Transform target;
    private float timeUntilFire;
    private NavMeshAgent targetAgent;
    private Quaternion targetRotation;


    private void Start()
    {
        if (bullet != null)
        {

            bulletSpeed = bullet.GetComponent<ProjectileBase>().GetSpeed();
            bulletAutoAim = bullet.GetComponent<ProjectileBase>().GetAutoTargeting();
        }
    }


    private void Update()
    {
        
        if (target == null)
        {
            FindTarget();
            return;
        }

        

        if (target != null)
        {
            RotateTowardsTarget();
        }

        if (!CheckTargetIsInRange()) target = null;

        else
        {
            timeUntilFire += Time.deltaTime;

            if (timeUntilFire >= 1f / fireRate)
            {
                Shoot();
                timeUntilFire = 0;
            }
        }
    }

    private void FindTarget()
    {

        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, (Vector2)transform.position, 0f, enemyMask);

        if (hits.Length > 0 )
        {
            target = hits[0].transform;
            targetAgent = target.GetComponent<NavMeshAgent>();
        }
    }

    private bool CheckTargetIsInRange()
    {
        return Vector2.Distance(target.position, transform.position) <= targetingRange;
    }

    private void RotateTowardsTarget()
    {
        float angle = Mathf.Atan2((target.position.y + (targetAgent.velocity.y)) - transform.position.y, (target.position.x + (targetAgent.velocity.x)) - transform.position.x) * Mathf.Rad2Deg + 90f;

        targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        turretRotationField.rotation = Quaternion.RotateTowards(turretRotationField.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void Shoot()
    {
        //Debug.Log("shoot!");

        GameObject _bullet = Instantiate(bullet, firePoint.position, turretRotationField.rotation);
        ProjectileBase bulletCon = _bullet.GetComponent<ProjectileBase>();

        bulletCon.SetTarget(target, -firePoint.up);
    }





}
