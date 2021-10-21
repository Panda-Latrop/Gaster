using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pawn : Actor, IPerceptionTarget
{
    [SerializeField]
    protected AIControllerBase autoAIController;
    protected bool hasController;
    protected ControllerBase controller;
    [SerializeField]
    protected HealthComponent health;
    [SerializeField]
    protected DeathBaseComponent death;

    public Pawn GetPawn()
    {
        return this;
    }
    public bool HasController { get => hasController; }
    public ControllerBase Controller { get => controller; }
    public HealthComponent Health { get => health; }
    public DeathBaseComponent Death { get => death; }

    protected virtual void Awake()
    {
        health.BindOnHurt(OnHurt);
        health.BindOnDeath(OnDeath);
        health.BindOnResurrect(OnResurrect);
        health.BindOnMorale(OnMorale);
    }

    protected virtual void Start()
    {
        if (!hasController && autoAIController != null && health.IsAlive)
        {
            (GameInstance.Instance.PoolManager.Pop(autoAIController) as AIControllerBase).Possess(this);
        }
    }

    public virtual void OnPossess(ControllerBase controller)
    {
        hasController = true;
        this.controller = controller;
        return;
    }
    public virtual void OnUnpossess()
    {
        if (hasController)
        {
            hasController = false;
            controller = null;
        }
        return;
    }

    protected virtual void OnHurt(DamageStruct ds, RaycastHit2D raycastHit)
    {
        return;
    }
    protected virtual void OnDeath(DamageStruct ds, RaycastHit2D raycastHit)
    {
        death.Execute(ds, raycastHit);
        //death.CreateDeathDynamic(transform.position, transform.rotation);
        //hasController = false;
        //controller = null;
    }
    protected virtual void OnMorale(DamageStruct ds, RaycastHit2D raycastHit)
    {
        return;
    }
    protected virtual void OnResurrect()
    {
        health.Health = health.MaxHealth;
        if (!hasController)
        {
            (GameInstance.Instance.PoolManager.Pop(autoAIController) as AIControllerBase).Possess(this);
        }
        return;
    }
    #region Save
    public override JSONObject Save(JSONObject jsonObject)
    {
        base.Save(jsonObject);
        jsonObject.Add("hasController", new JSONBool(hasController));
        if (hasController)
            SaveSystem.ComponentReferenceSave(jsonObject, "", controller);
        jsonObject.Add("health", health.Save(new JSONObject()));
        return jsonObject;
    }
    public override JSONObject Load(JSONObject jsonObject)
    {
        base.Load(jsonObject);
        if (hasController = jsonObject["hasController"].AsBool)
        {
            hasController = SaveSystem.ComponentReferenceLoad(jsonObject, "", ref controller);
        }
        health.Load(jsonObject["health"].AsObject);
        return jsonObject;
    }
    #endregion

    protected virtual void OnDestroy()
    {

        health.UnbindOnHurt(OnHurt);
        health.UnbindOnDeath(OnDeath);
        health.UnbindOnResurrect(OnResurrect);

        if (!GameInstance.ApplicationIsQuit && !GameInstance.ChangeScene && hasController)
        {
            controller.Unpossess();
        }
    }
}
