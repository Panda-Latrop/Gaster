using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTrajectoryStepByStep : WeaponTrajectoryBase
{
    protected Vector3 direction,target;
    [SerializeField]
    protected float timeToCreate = 0.25f, timeToPrepare = 1.0f, timeToRelease = 0.25f;
    protected float nextCreate, nextPrepare, nextRelease;
    protected int count;

    public override void SetTarget(Vector3 target)
    {
        this.target = target;
    }
    protected void Start()
    {
        if (trajectoryComponent.GetCount() <= 0)
            OnProjectileCountChange();
    }
    public override void SetFire(bool fire)
    {
        base.SetFire(fire);
        if (!fire)
        {
            ReleaseAllProjectiles();
            nextShoot = Time.time + timeToShoot;
            shootState = ShootState.ended;
            stage = StageState.stage0;
            enabled = false;
        }
    }
    public override ShootState Shoot(Vector3 position, Vector3 direction)
    {
        switch (shootState)
        {
            case ShootState.initiated:
            case ShootState.process:
                shootState = ShootState.process;
                switch (stage)
                {
                    case StageState.stage0:
                        if (CanCreate())
                        {
                            CreateProjectile(transform.position, Vector3.zero);
                            if (count >= maxProjectileCount)
                            {
                                nextPrepare = Time.time + timeToPrepare;
                                stage = StageState.stage1;
                            }
                        }
                        break;
                    case StageState.stage1:
                        if (CanPrepare())
                            stage = StageState.stage2;
                        break;
                    case StageState.stage2:
                        if (CanRelease())
                        {
                            if (!ReleaseProjectile(direction))
                            {
                                ReleaseAllProjectiles();
                                nextShoot = Time.time + timeToShoot;
                                stage = StageState.stage0;
                                shootState = ShootState.ended;
                                enabled = false;

                            }
                        }
                        break;
                    default:
                        break;
                }
                break;
            case ShootState.unready:
            case ShootState.ended:
                if (CanShoot() && projectiles.Count == 0)
                {
                    enabled = true;
                    PlayAudio();
                    this.direction = direction;
                    if (animateScale)
                        trajectoryComponent.scale = minScale;
                    if (useRandomShift)
                        trajectoryComponent.RandomShift();
                    //transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
                    //PlayAudio();
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

        IPoolProjectile ipp = GameInstance.Instance.PoolManager.Pop(projectile) as IPoolProjectile;
        ipp.SetPosition(position);
        ipp.SetRotation(Quaternion.AngleAxis(orientationShift, Vector3.forward));
        ipp.SetDamage(owner.gameObject, owner.Health.Team, damage * damageMultiply, power,speed);
        AddProjectile(ipp.Projectile);
        count++;
        return default;
    }
    protected override bool ReleaseProjectile(Vector3 direction)
    {
        int last = projectiles.Count - 1;
        while (projectiles.Count > 0 && ignore[last])
        {
            projectiles.RemoveAt(last);
            last--;
        }
        if (projectiles.Count > 0)
        {
            ProjectileBase pb = projectiles[last];
            direction = (target - pb.transform.position).normalized;
            pb.SetDirection(direction, 0);
            projectiles.RemoveAt(last);
            return projectiles.Count > 0;
        }
        return false;
    }
    protected override void ReleaseAllProjectiles()
    {
        base.ReleaseAllProjectiles();
        count = 0;
    }
    protected bool CanPrepare()
    {
        if (Time.time >= nextPrepare || projectiles.Count == 0)
        {
            nextPrepare = Time.time + timeToPrepare;
            return true;
        }
        return false;
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
    protected bool CanCreate()
    {
        if (Time.time >= nextCreate)
        {
            nextCreate = Time.time + timeToCreate;
            return true;
        }
        return false;
    }
    protected override void OnProjectileCountChange()
    {
        trajectoryComponent.SetCount(maxProjectileCount);
    }
    protected void FixedUpdate()
    {
        FollowTrajectory();
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
        JSONArray dirJArray = new JSONArray();
        {
            dirJArray.Add(new JSONNumber(direction.x));
            dirJArray.Add(new JSONNumber(direction.y));
            dirJArray.Add(new JSONNumber(direction.z));
        }
        jsonObject.Add("direction", dirJArray);
        JSONArray targetJArray = new JSONArray();
        {
            targetJArray.Add(new JSONNumber(target.x));
            targetJArray.Add(new JSONNumber(target.y));
            targetJArray.Add(new JSONNumber(target.z));
        }
        jsonObject.Add("target", targetJArray);
        SaveSystem.TimerSave(jsonObject, "create", nextCreate);
        SaveSystem.TimerSave(jsonObject, "prepare", nextPrepare);
        SaveSystem.TimerSave(jsonObject, "release", nextRelease);
        jsonObject.Add("count", new JSONNumber(count));
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        JSONArray dirJArray = jsonObject["direction"].AsArray;
        direction.Set(dirJArray[0].AsFloat, dirJArray[1].AsFloat, dirJArray[2].AsFloat);
        JSONArray targetJArray = jsonObject["target"].AsArray;
        target.Set(targetJArray[0].AsFloat, targetJArray[1].AsFloat, targetJArray[2].AsFloat);
        SaveSystem.TimerLoad(jsonObject, "create", ref nextCreate);
        SaveSystem.TimerLoad(jsonObject, "prepare", ref nextPrepare);
        SaveSystem.TimerLoad(jsonObject, "release", ref nextRelease);
        count = jsonObject["count"].AsInt;
        return jsonObject;
    }

}
