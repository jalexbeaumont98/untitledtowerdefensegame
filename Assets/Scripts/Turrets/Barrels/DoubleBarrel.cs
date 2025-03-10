using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleBarrel : BarrelBase
{

    public List<Transform> firepoints;
    
    private int index = 0;

    protected override void Start()
    {
        base.Start();
        firepoint = firepoints[index];
    }
    public override Transform GetFirepoint()
    {
        return firepoint;
    }

    public override void Shoot()
    {
        base.Shoot();

        index++;

        if (index >= firepoints.Count) index = 0;
        firepoint = firepoints[index];
    }
}
