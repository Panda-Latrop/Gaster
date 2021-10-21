using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DominatorDelegate(Pawn dominated);

public class WeaponDominator : WeaponBase
{
    [SerializeField]
    protected DynamicExecutor dominateEffect;
    [SerializeField]
    protected Vector3 areaSize;
    [SerializeField]
    protected ContactFilter2D filter = new ContactFilter2D();
    protected Collider2D[] collider2Ds = new Collider2D[5];

    protected DominatorDelegate OnDominate;


    protected override void Awake()
    {
        base.Awake();
        filter.useLayerMask = true;
        filter.layerMask = LayerMask.GetMask("Pawn");
    }

    public override ShootState Shoot(Vector3 position, Vector3 direction)
    {
        if (CanShoot())
        {
            CreateProjectile(position, direction);
            var ipo = GameInstance.Instance.PoolManager.Pop(dominateEffect);
            ipo.SetPosition(position);
            PlayAudio();
            shootState = ShootState.initiated;
            return shootState;
        }
        shootState = ShootState.unready;
        return shootState;
    }

    protected override RaycastHit2D CreateProjectile(Vector3 position, Vector3 direction)
    {
        int count = Physics2D.OverlapBox(position, areaSize, 0, filter, collider2Ds);
        for (int i = 0; i < count; i++)
        {
            IHealth health = collider2Ds[i].GetComponent<IHealth>();
            if (health != null && !owner.Health.Team.Equals(health.Team))
            {
                RaycastHit2D raycastHit = new RaycastHit2D();
                if (health.DownMorale(new DamageStruct(owner.gameObject, owner.Health.Team, damage * damageMultiply, direction, power), raycastHit).Equals(HurtResult.kill))
                {
                    CallOnDominate(collider2Ds[i].GetComponent<Pawn>());
                }
                return raycastHit;
            }
        }
        return default;
    }
    #region Delegate
    public void CallOnDominate(Pawn summoned)
    {
        OnDominate?.Invoke(summoned);
    }
    public void BindOnDominate(DominatorDelegate action)
    {
        //Debug.Log("BindOnDominate ");
        OnDominate += action;
    }
    public void UnbindOnDominate(DominatorDelegate action)
    {
        OnDominate -= action;
    }
    public void ClearOnDominate()
    {
        OnDominate = null;
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, areaSize);
        }
    }

    protected void OnDestroy()
    {
        OnDominate = null;
    }
}
