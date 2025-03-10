using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncherBarrel : NonRotatingBarrel
{

    public override void Shoot()
    {
        animator.SetInteger("state", 1);
        //call first animation
        //on finish call idleanim
    }

    public override void FinalShoot()
    {
        base.FinalShoot();
        animator.SetInteger("state", 2);
        //call end animation
    }

}
