using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBullet : ProjectileBase
{
    // Start is called before the first frame update

    protected override void Start()
    {
        base.Start();
    }

    public override void SetTarget(Transform _target, Vector2 _direction)
    {
        base.SetTarget(_target, _direction);

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

    }

    public override void Update()
    {
        base.Update();
    }

    protected override void CheckInsideBounds()
    {
        base.CheckInsideBounds();
    }
}
