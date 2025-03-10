using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NonRotatingBarrel : BarrelBase
{

    public Transform spriteTransform;

    protected override void Start()
    {
        base.Start();

        //spriteTransform = spriteRenderer.GetComponent<Transform>();
    }

    private void Update() {
        spriteTransform.rotation = Quaternion.identity;
    }
}
