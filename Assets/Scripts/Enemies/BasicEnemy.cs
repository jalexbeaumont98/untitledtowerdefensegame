using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : EnemyBase
{
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }

    public override NavMeshAgent GetAgent()
    {
        return base.GetAgent();
    }

    public override void Die(bool goal = false)
    {
        base.Die();
    }

    protected override void Move()
    {
        base.Move();
    }

    protected override void Update()
    {
        base.Update();
    }
}
