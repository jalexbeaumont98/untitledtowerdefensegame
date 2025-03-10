using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelBase : MonoBehaviour
{

    [Header("References")]
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected Transform firepoint;
    [SerializeField] protected TurretBase turret;
    [SerializeField] protected Animator animator;

    // Start is called before the first frame update

    protected virtual void Start()
    {
        turret = GetComponentInParent<TurretBase>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (!spriteRenderer) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }


    public virtual Transform GetFirepoint()
    {
        return firepoint;
    }

    public virtual void Shoot()
    {

        FinalShoot();
    }

    public virtual void FinalShoot()
    {
        turret.ShootFromBarrel(firepoint);
    }

    public virtual Sprite GetSprite()
    {
        return spriteRenderer.sprite;
    }

    public virtual void SetPreview()
    {
        spriteRenderer.color = new Color(1f, 1f, 1f, 180f / 255f);
    }
}
