using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void SummonDelegate(ProjectilePawnBase summoned);
public class WeaponSummoner : WeaponBase
{
    [SerializeField]
    protected ProjectilePawnBase summonPrefab;

    protected SummonDelegate OnSummoned;
    public override ShootState Shoot(Vector3 position, Vector3 direction)
    {
        if (CanShoot())
        {
            CreateProjectile(position, direction);
            PlayAudio();
            shootState = ShootState.initiated;
            return shootState;
        }
        shootState = ShootState.unready;
        return shootState;
    }

    protected override RaycastHit2D CreateProjectile(Vector3 position, Vector3 direction)
    {
        ProjectilePawnBase summoned = GameInstance.Instance.PoolManager.Pop(summonPrefab) as ProjectilePawnBase;
        summoned.SetPosition(position);
        summoned.SetDamage(owner.gameObject, owner.Health.Team, damage * damageMultiply, power, speed);
        summoned.SetDirection(Quaternion.AngleAxis(Random.Range(-180,180),Vector3.forward)* direction, 0.0f);
        CallOnSummoned(summoned);
        return default;
    }
    #region Delegate
    public void CallOnSummoned(ProjectilePawnBase summoned)
    {
        OnSummoned?.Invoke(summoned);
    }
    public void BindOnSummoned(SummonDelegate action)
    {
        OnSummoned += action;
    }
    public void UnbindOnSummoned(SummonDelegate action)
    {
        OnSummoned -= action;
    }
    public void ClearOnSummoned()
    {
        OnSummoned = null;
    }
    #endregion
    protected void OnDestroy()
    {
        OnSummoned = null;
    }
}

    