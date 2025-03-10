using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Camera mainCamera;
    [SerializeField] protected GameObject impact_explosion;


    [Header("Attributes")]
    [SerializeField] private float bulletSpeed;
    [SerializeField] private int damage;
    [SerializeField] private bool autoTargeting;

    [SerializeField] private bool canHitFlying;
    [SerializeField] private string upgradeNotes = "";

    private Transform target;
    private bool velocitySet;
    private Vector2 direction;


    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    public virtual void SetTarget(Transform _target, Vector2 _direction)
    {
        target = _target;

        if (!autoTargeting)
        {
            direction = _direction;
        }
    }

    public virtual float[] GetStats()
    {
        float[] stats = new float[3];

        stats[0] = damage;
        stats[1] = bulletSpeed;
        stats[2] = 0;

        if (autoTargeting) stats[2] = 1;


        return stats;
    }

    public virtual string GetUpgradeNotes()
    {
        return upgradeNotes;
    }

    public virtual float GetSpeed()
    {
        return bulletSpeed;
    }

    public virtual bool GetAutoTargeting()
    {
        return autoTargeting;
    }

    public virtual void Update()
    {
        CheckInsideBounds();

    }


    protected virtual void FixedUpdate()
    {
        if (autoTargeting)
        {
            if (!target) return;

            direction = (target.position - transform.position).normalized;

            rb.velocity = direction * bulletSpeed;

        }

        else if (!velocitySet)
        {
            rb.velocity = direction * bulletSpeed;
            velocitySet = true;
        }

    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null) return;

        EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();

        if (enemy)
        {
            if (enemy.GetFlying() && !canHitFlying) return;

            if (impact_explosion)
            {
                Vector3 position = transform.position;
                Instantiate(impact_explosion, position, quaternion.identity); 
            }

            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }

    }

    protected virtual void CheckInsideBounds()
    {

        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        bool isVisible = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

        if (!isVisible) Destroy(gameObject);
    }
}
