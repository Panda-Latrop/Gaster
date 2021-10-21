using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShooter : WeaponBase
{
    [SerializeField]
    protected ProjectileBase projectile;
    [SerializeField]
    protected float maxSpread, minSpread, spreadByShoot, spreadByTime;
    protected float spread;

    public override ShootState Shoot(Vector3 position, Vector3 direction)
    {       
        if (CanShoot())
        {
            enabled = true;
            CreateProjectile(position, direction);
            IncreaseSpread();
            PlayMuzzleFlash();
            PlayAudio();
            shootState = ShootState.initiated;
            return shootState;
        }
        shootState = ShootState.unready;
        return shootState;
    }
    protected override RaycastHit2D CreateProjectile(Vector3 position, Vector3 direction)
    {
        IPoolProjectile ipp = GameInstance.Instance.PoolManager.Pop(projectile) as IPoolProjectile;
        ipp.SetPosition(position);
        ipp.SetDamage(owner.gameObject, owner.Health.Team, damage * damageMultiply, power, speed);
        direction = Quaternion.AngleAxis(Random.Range(-spread, spread), Vector3.forward) * direction;
        ipp.SetDirection(direction, 0);
        return default;
    }
    protected bool IncreaseSpread()
    {
        spread += spreadByShoot;
        if (spread > maxSpread)
        {
            spread = maxSpread;
            return true;
        }
        return false;
    }
    protected bool DecreaseSpread()
    {
        spread -= spreadByTime * Time.deltaTime;
        if (spread < minSpread)
        {
            spread = minSpread;
            return true;
        }
        return false;
    }
    protected virtual void Update()
    {
        if (!isFire && Time.time >= nextShoot)
        {
            if (DecreaseSpread())
                enabled = false;
        }
    }
    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        jsonObject.Add("spread", new JSONNumber(spread));
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load(jsonObject);
        spread = jsonObject["spread"].AsFloat;
        return jsonObject;
    }
}
