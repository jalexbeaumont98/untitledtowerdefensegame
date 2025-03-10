using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : ProjectileBase
{
    
    public override bool GetAutoTargeting()
    {
        return base.GetAutoTargeting();
    }

    public override float GetSpeed()
    {
        return base.GetSpeed();
    }

    public override void SetTarget(Transform _target, Vector2 _direction)
    {
        base.SetTarget(_target, _direction);
    }

    public override void Update()
    {
        base.Update();
    }

    protected override void CheckInsideBounds()
    {
        base.CheckInsideBounds();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
    }

    protected override void Start()
    {
        base.Start();
    }
}
