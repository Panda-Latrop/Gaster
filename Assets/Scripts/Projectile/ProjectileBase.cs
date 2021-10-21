using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileBase : Actor , IPoolProjectile
{
    [SerializeField]
    protected string specifier = "base";
    [SerializeField]
    protected new Rigidbody2D rigidbody;
    //[SerializeField]
    protected DamageStruct ds;
    public string PoolTag => this.GetType().Name+ specifier;
    public ProjectileBase Projectile => this;
    public Rigidbody2D Rigidbody => rigidbody;
    public virtual void SetDamage(GameObject owner, Team team, float damage, float power, float speed)
    {
        ds = new DamageStruct(owner, team, damage, Vector3.zero, power);
    }
    public abstract void SetDirection(Vector3 direction, float angle);
    protected virtual HurtResult ApplyDamage(IHealth target,Collider2D collider, RaycastHit2D raycastHit)
    {
        ds.direction = transform.right;
        HurtResult hr = target.Hurt(ds, raycastHit);
        HurtResultProcess(hr,collider,raycastHit);
        return hr;
    }
    protected abstract void HurtResultProcess(HurtResult hurtResult, Collider2D collider, RaycastHit2D raycastHit);

    public abstract void CheckTarget(Collider2D collider);

    #region Pool
    public abstract void OnPush();
    public abstract void OnPop();
    public Transform Transform => transform;
    public virtual void SetPosition(Vector3 position) => transform.position = position;
    public void SetRotation(Quaternion rotation) => transform.rotation = rotation;
    public void SetScale(Vector3 scale) => transform.localScale = scale;
    public void SetParent(Transform parent) => transform.parent = parent;
    #endregion
    #region Save
    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        JSONObject dsJObject = new JSONObject();
        bool hasCauser = ds.causer.activeSelf;
        dsJObject.Add("hasCauser", new JSONBool(hasCauser));
        if(hasCauser)
            SaveSystem.GameObjectReferenceSave(dsJObject, "causer", ds.causer);
        dsJObject.Add("team", new JSONNumber((int)ds.team));
        dsJObject.Add("damage", new JSONNumber(ds.damage));
        dsJObject.Add("power", new JSONNumber(ds.power));
        jsonObject.Add("damageS", dsJObject);
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        JSONObject dsJObject = jsonObject["damageS"].AsObject;
        if (dsJObject["hasCauser"].AsBool)
        {
            GameObject go = null;
            if (SaveSystem.GameObjectReferenceLoad(dsJObject, "causer", ref go))
            {
                ds.causer = go;
            }
        }
        else
        {
            ds.causer = gameObject;
        }     
        ds.team = (Team)dsJObject["team"].AsInt;
        ds.damage = dsJObject["damage"].AsFloat;
        ds.power = dsJObject["power"].AsFloat;
        return jsonObject;
    }
    #endregion
}
