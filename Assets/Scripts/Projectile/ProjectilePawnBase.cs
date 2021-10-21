using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectilePawnBase : ProjectileBase
{
    [SerializeField]
    protected new Collider2D collider;
    [SerializeField]
    protected HealthComponent health;
    [SerializeField]
    protected DeathComponent death;
    [SerializeField]
    protected SpriteRendererHolderComponent spriteRendererHolder;
    public HealthComponent Health => health;
    public SpriteRendererHolderComponent SpriteRendererHolder => spriteRendererHolder;

    protected virtual void Awake()
    {
        health.BindOnHurt(OnHurt);
        health.BindOnDeath(OnDeath);
        health.BindOnResurrect(OnResurrect);
    }
    public override void OnPop()
    {
        gameObject.SetActive(true);
        enabled = true;
        //collider.enabled = true;
        health.Resurrect();
    }
    public override void OnPush()
    {
        gameObject.SetActive(false);
        enabled = false;
        //collider.enabled = false;      
        health.Kill();
    }
    public override void SetDamage(GameObject owner, Team team, float damage, float power, float speed)
    {
        base.SetDamage(owner, team, damage, power, speed);
        health.Team = team;
    }
    public override void SetDirection(Vector3 direction, float angle)
    {
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
    }
    public abstract void Move(Vector2 direction);
    public abstract void SetFire(bool fire);
    public abstract void Shoot(Vector2 position, Vector2 direction);
    protected virtual void OnHurt(DamageStruct ds, RaycastHit2D raycastHit)
    {
        spriteRendererHolder.OnHurt();
    }
    protected virtual void OnDeath(DamageStruct ds, RaycastHit2D raycastHit)
    {
        collider.enabled = false;
        rigidbody.velocity = Vector3.zero;
        rigidbody.rotation = 0.0f;
        rigidbody.simulated = false;
        spriteRendererHolder.Clear();
        death.CreateDeathDynamic(transform.position, transform.rotation);
        GameInstance.Instance.PoolManager.Push(this);
    }
    protected virtual void OnResurrect()
    {
        collider.enabled = true;
        rigidbody.simulated = true;
        health.Health = health.MaxHealth;

    }

    public override JSONObject Save(JSONObject jsonObject)
    {
        base.Save(jsonObject);
        SaveSystem.ColliderSave(jsonObject, "physics", collider);
        SaveSystem.RigidbodySave(jsonObject, rigidbody);
        jsonObject.Add("health", health.Save(new JSONObject()));
        jsonObject.Add("spriteHolder", spriteRendererHolder.Save(new JSONObject()));
        return jsonObject;
    }
    public override JSONObject Load(JSONObject jsonObject)
    {
        base.Load(jsonObject);
        SaveSystem.ColliderLoad(jsonObject, "physics", collider);
        SaveSystem.RigidbodyLoad(jsonObject, rigidbody);
        health.Load(jsonObject["health"].AsObject);
        spriteRendererHolder.Load(jsonObject["spriteHolder"].AsObject);
        return jsonObject;
    }

    protected virtual void OnDestroy()
    {
        health.UnbindOnHurt(OnHurt);
        health.UnbindOnDeath(OnDeath);
        health.UnbindOnResurrect(OnResurrect);
    }
}
