using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControllerBase : Actor
{
    protected bool hasPawn;
    protected Pawn controlledPawn;
    public bool HasPawn { get => hasPawn; }
    public Pawn ControlledPawn { get => controlledPawn; }
    public virtual void Possess(Pawn pawn)
    {
        gameObject.SetActive(true);
        enabled = true;
        controlledPawn = pawn;
        pawn.OnPossess(this);
        hasPawn = true;
        controlledPawn.Health.BindOnHurt(OnHurt);
        controlledPawn.Health.BindOnDeath(OnDeath);
        controlledPawn.Health.BindOnMorale(OnMorale);
        //if(!pawn.SaveTag.Equals(string.Empty))
        //saveTag = pawn.SaveTag;
    }
    public virtual void Unpossess()
    {
            gameObject.SetActive(false);
            enabled = false;
            if (hasPawn)
            {
                controlledPawn.OnUnpossess();
                controlledPawn = null;
                hasPawn = false;
            }
    }
    protected virtual void OnHurt(DamageStruct ds, RaycastHit2D raycastHit) { }
    protected virtual void OnDeath(DamageStruct ds, RaycastHit2D raycastHit) { }
    protected virtual void OnMorale(DamageStruct ds, RaycastHit2D raycastHit) { }
    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        jsonObject.Add("hasPawn", new JSONBool(hasPawn));
        if (hasPawn)
            SaveSystem.ComponentReferenceSave(jsonObject, string.Empty, controlledPawn);
        return jsonObject;
    }

    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        if(hasPawn)
            return jsonObject;
        hasPawn = jsonObject["hasPawn"].AsBool;
        if(hasPawn)
        {
            Pawn pawn = default;
            if(hasPawn = SaveSystem.ComponentReferenceLoad(jsonObject, "",ref pawn))
            {
                Possess(pawn);
            }
        }
        return jsonObject;
    }

    protected virtual void OnDestroy()
    {
        if (!GameInstance.ApplicationIsQuit && !GameInstance.ChangeScene && hasPawn)
        {
                Unpossess();
        }
    }
}
