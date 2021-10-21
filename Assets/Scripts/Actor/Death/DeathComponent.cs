using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathComponent : DeathBaseComponent
{
    [SerializeField]
    protected DynamicExecutor deathDynamic;
    [SerializeField]
    protected bool rotateTowardPower = true;

    protected void Start()
    {
        enabled = deathDynamic != null;
    }
    public override void Execute(DamageStruct ds, RaycastHit2D raycastHit)
    {
        if (enabled)
        {
            Quaternion rotation = Quaternion.identity;
            if (rotateTowardPower)
                rotation = Quaternion.AngleAxis(Mathf.Atan2(ds.direction.y, ds.direction.x) * Mathf.Rad2Deg, Vector3.forward);
            CreateDeathDynamic(transform.position, rotation); 
        }
    }
    public void CreateDeathDynamic(Vector3 position, Quaternion rotation, bool attach = false, Transform parent = default)
    {

        IPoolObject po;
        po = GameInstance.Instance.PoolManager.Pop(deathDynamic);
        po.SetPosition(position);
        po.SetRotation(rotation);
        if (attach)
            po.SetParent(parent);

    }
}
