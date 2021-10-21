using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePawnShield : ProjectilePawnBase
{
    public override void CheckTarget(Collider2D collider)
    {
        return;
    }

    public override void Move(Vector2 direction)
    {
        return;
    }

    public override void SetFire(bool fire)
    {
        return;
    }

    public override void Shoot(Vector2 position, Vector2 direction)
    {
        return;
    }

    protected override void HurtResultProcess(HurtResult hurtResult, Collider2D collider, RaycastHit2D raycastHit)
    {
        return;
    }
}
