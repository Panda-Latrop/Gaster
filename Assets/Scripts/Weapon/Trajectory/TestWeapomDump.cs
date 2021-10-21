using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWeapomDump : Pawn
{
    public WeaponBase weapon;
    protected override void Start()
    {
        weapon.SetOwner(this);
    }
    public bool shoot;
    public void Update()
    {
        weapon.SetFire(shoot);
        if (shoot)
            weapon.Shoot(transform.position,Vector3.right);
    }
}
