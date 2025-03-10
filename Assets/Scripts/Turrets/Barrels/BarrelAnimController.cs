using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BarrelAnimController : MonoBehaviour
{
    [SerializeField] private BarrelBase barrelBase;

    private void Start() {
        barrelBase = GetComponentInParent<BarrelBase>();
    }

    public void Shoot() {
        barrelBase.FinalShoot();
    }
}
