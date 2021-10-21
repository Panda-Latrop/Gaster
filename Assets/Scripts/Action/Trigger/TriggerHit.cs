using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerHit : TriggerExecutor, IHealth
{
    public Team Team { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public HurtResult DownMorale(DamageStruct ds, RaycastHit2D raycastHit)
    {
        return HurtResult.miss;
    }
    public void Heal(float health)
    {
        return;
    }
    public HurtResult Hurt(DamageStruct ds, RaycastHit2D raycastHit)
    {
        Execute();
        return HurtResult.kill;
    }
    public void UpMorale(float morale)
    {
        return;
    }
}
