using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTrajectoryWithAll : WeaponTrajectoryBase
{
    [SerializeField]
    protected float timeToRelease = 2.0f;
    protected float nextRelease;
    protected void Start()
    {
        if(trajectoryComponent.GetCount() <= 0)
        OnProjectileCountChange();
    }
    public override void SetFire(bool fire)
    {

        base.SetFire(fire);
        if (!fire)
        {
            ReleaseAllProjectiles();
            shootState = ShootState.ended;
        }

    }
    public override ShootState Shoot(Vector3 position, Vector3 direction)
    {
        switch (shootState)
        {
            case ShootState.initiated:
            case ShootState.process:
                shootState = ShootState.process;
                if (CanRelease() || projectiles.Count == 0)
                {
                    ReleaseAllProjectiles();
                    nextShoot = Time.time + timeToShoot;
                    shootState = ShootState.ended;
                }
                break;
            case ShootState.unready:
            case ShootState.ended:
                if (CanShoot() && projectiles.Count == 0)
                {
                    enabled = true;
                    PlayAudio();
                    if (animateScale)
                        trajectoryComponent.scale = minScale;                  
                    if (useRandomShift)
                        trajectoryComponent.RandomShift();
                    CreateProjectile(transform.position, Vector3.zero);
                    //trajectoryComponent.scale = 0.0f;
                    nextRelease = Time.time + timeToRelease;
                    shootState = ShootState.initiated;
                    break;
                }
                shootState = ShootState.unready;
                break;
            default:
                break;
        }
        return shootState;
    }

    protected override RaycastHit2D CreateProjectile(Vector3 position, Vector3 direction)
    {
        for (int i = 0; i < maxProjectileCount; i++)
        {
            IPoolProjectile ipp = GameInstance.Instance.PoolManager.Pop(projectile) as IPoolProjectile;
            ipp.SetPosition(position);
            ipp.SetRotation(Quaternion.AngleAxis(orientationShift,Vector3.forward));
            ipp.SetDamage(owner.gameObject, owner.Health.Team, damage * damageMultiply, power, speed);
            AddProjectile(ipp.Projectile);
        }
        OnProjectileCountChange();
        return default;
    }
    protected override void ReleaseAllProjectiles()
    {
        base.ReleaseAllProjectiles();
        enabled = false;
    }
    protected void FixedUpdate()
    {
        FollowTrajectory();
    }
    protected bool CanRelease()
    {
        if (Time.time >= nextRelease || IsAllIgnore())
        {
            nextRelease = Time.time + timeToRelease;
            return true;
        }
        return false;
    }
    protected override void OnProjectileCountChange()
    {
        trajectoryComponent.SetCount(maxProjectileCount);
    }
    protected void Update()
    {
        trajectoryComponent.OnUpdate();
        if (animateScale && trajectoryComponent.scale < maxScale)
        {
            trajectoryComponent.scale += Time.deltaTime * speedScale;
            if (trajectoryComponent.scale >= maxScale)
            {
                trajectoryComponent.scale = maxScale;

            }
        }
    }
    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        SaveSystem.TimerSave(jsonObject, "release", nextRelease);
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        SaveSystem.TimerLoad(jsonObject, "release", ref nextRelease);
        return jsonObject;
    }
}
