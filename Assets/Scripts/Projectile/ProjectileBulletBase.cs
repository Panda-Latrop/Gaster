using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBulletBase : ProjectileBase
{
    [SerializeField]
    protected Collider2D detector;
    [SerializeField]
    protected ProjectileImpactCreatorComponent impactCreator;
    [SerializeField]
    protected ProjectileMovementComponent projectileMovement;

    public Collider2D Detector => detector;
    public ProjectileImpactCreatorComponent ImpactCreator => impactCreator;
    public ProjectileMovementComponent ProjectileMovement => projectileMovement;
    public override void OnPop()
    {
        gameObject.SetActive(true);
        enabled = true;
        detector.enabled = true;
    }
    public override void OnPush()
    {
        gameObject.SetActive(false);
        enabled = false;       
        detector.enabled = false;
        projectileMovement.Stop();
    }
    /* public override void SetPosition(Vector3 position)
     {
         projectileMovement.SetPosition(position);
     }*/
    public override void SetDamage(GameObject owner, Team team, float damage, float power, float speed)
    {
        base.SetDamage(owner, team, damage, power, speed);
        projectileMovement.Speed = speed;
    }
    public override void SetDirection(Vector3 direction, float angle)
    {
        //transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);
        projectileMovement.Move(direction);
    }

    protected override void HurtResultProcess(HurtResult hurtResult, Collider2D collider, RaycastHit2D raycastHit)
    {
        if (!hurtResult.Equals(HurtResult.friend) && !hurtResult.Equals(HurtResult.none))
        {
            if(!hurtResult.Equals(HurtResult.kill))
            {
                impactCreator.SetPhysicsMaterial(collider.sharedMaterial);
                impactCreator.CreateImpact(transform.position, transform.rotation);
            }
            GameInstance.Instance.PoolManager.Push(this);
        }
    }
    public override void CheckTarget(Collider2D collider)
    {
        //Debug.Log(collider.name + " " + collider.sharedMaterial.name);
        if (detector.enabled)
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
    #region Save
    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        SaveSystem.ColliderSave(jsonObject, "detector", detector);
        SaveSystem.RigidbodySave(jsonObject, rigidbody);
        jsonObject.Add("movement", projectileMovement.Save( new JSONObject()));
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load(jsonObject);
        SaveSystem.ColliderLoad(jsonObject, "detector", detector);
        SaveSystem.RigidbodyLoad(jsonObject, rigidbody);
        projectileMovement.Load( jsonObject["movement"].AsObject);      
        return jsonObject;
    }
    #endregion
}
