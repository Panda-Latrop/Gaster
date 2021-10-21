using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePawnKamikaze : ProjectilePawnBase
{
    [SerializeField]
    protected Collider2D detector;
    [SerializeField]
    protected ProjectileImpactCreatorComponent impactCreator;
    [SerializeField]
    protected ProjectileMovementComponent projectileMovement;
    public ProjectileImpactCreatorComponent ImpactCreator => impactCreator;
    public ProjectileMovementComponent ProjectileMovement => projectileMovement;
    public override void Move(Vector2 direction)
    {
        projectileMovement.Move(direction);
    }
    public override void SetFire(bool fire)
    {
        return;
    }
    public override void Shoot(Vector2 position, Vector2 direction)
    {
        return;
    }
    public override void SetDamage(GameObject owner, Team team, float damage, float power, float speed)
    {
        base.SetDamage(owner, team, damage, power, speed);
        projectileMovement.Speed = speed;
    }
    protected override void HurtResultProcess(HurtResult hurtResult, Collider2D collider, RaycastHit2D raycastHit)
    {
        if (!hurtResult.Equals(HurtResult.friend) && !hurtResult.Equals(HurtResult.miss))
        {
            if (!hurtResult.Equals(HurtResult.kill))
            {
                impactCreator.SetPhysicsMaterial(collider.sharedMaterial);
                impactCreator.CreateImpact(transform.position, transform.rotation);
            }
            health.Kill();
        }
    }
    protected override void OnDeath(DamageStruct ds, RaycastHit2D raycastHit)
    {
        detector.enabled = false;
        projectileMovement.enabled = false;
        base.OnDeath(ds,raycastHit);
    }
    protected override void OnResurrect()
    {
        detector.enabled = true;
        projectileMovement.enabled = true;
        base.OnResurrect();
    }
    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        jsonObject.Add("movement", projectileMovement.Save(new JSONObject()));
        SaveSystem.ColliderSave(jsonObject, "detector", detector);
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load(jsonObject);
        projectileMovement.Load(jsonObject["movement"].AsObject);
        SaveSystem.ColliderLoad(jsonObject, "detector", detector);
        return jsonObject;
    }
    public override void CheckTarget(Collider2D collider)
    {
        if (collider.enabled)
        {
            RaycastHit2D rh = new RaycastHit2D();
            rh.point = collider.transform.position;
            rh.normal = (-collider.transform.position + transform.position).normalized;
            if (collider.gameObject.layer.Equals(LayerMask.NameToLayer("Pawn")))
            {
               
                IHealth target = collider.GetComponent<IHealth>();
                if (target != null)
                {
                    ApplyDamage(target, collider, rh);                   
                }
            }
            else
            {
                HurtResultProcess(HurtResult.miss, collider, rh);
            }
        }
    }
}
