using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControllerAttacker : AIControllerBase
{
    protected CharacterAttacker character;

    protected bool hasTarget,needMoreTarget,inCombat;
    protected Pawn target;

    [SerializeField]
    protected List<Pawn> targets = new List<Pawn>();

    [SerializeField]
    protected float distanceToCommander = 10.0f;
    [SerializeField]
    protected bool hasCommander;
    [SerializeField]
    protected Pawn commander;

    protected float timeToGetClosest = 5.0f;
    protected float nextClosest;


    public override void Possess(Pawn pawn)
    {
        base.Possess(pawn);
        character = pawn as CharacterAttacker;
        character.Perception.BindOnPerceptionDetection(OnPerceptionDetection);
    }
    public override void Unpossess()
    {
        //Debug.Log(controlledPawn == null);
        controlledPawn.Health.UnbindOnHurt(OnHurt);
        controlledPawn.Health.UnbindOnDeath(OnDeath);
        controlledPawn.Health.UnbindOnMorale(OnMorale);
        character.Perception.UnbindOnPerceptionDetection(OnPerceptionDetection);
        targets.Clear();
        hasTarget = false;
        target = null;
        hasCommander = false;
        commander = null;
        character = null;
        InCombat(false);
        base.Unpossess();

    }
    protected void InCombat(bool inCombat)
    {
        if (this.inCombat != inCombat)
        {

            if (this.inCombat = inCombat)
                GameInstance.Instance.GameState.AddInCombat();
            else
                GameInstance.Instance.GameState.RemoveInCombat();
        }
    }
    protected override void OnHurt(DamageStruct ds, RaycastHit2D raycastHit)
    {
        //Debug.Log("Call");
        if (!hasTarget)
        {
            
            character.CharacterMovement.MoveTo(ds.causer.transform.position, true);
        }
    }
    protected override void OnDeath(DamageStruct ds, RaycastHit2D raycastHit)
    {
        InCombat(false);
        Unpossess();      
    }
    protected override void OnMorale(DamageStruct ds, RaycastHit2D raycastHit)
    {
        Pawn pawn = ds.causer.GetComponent<Pawn>();
        InCombat(false);
        hasTarget = false;
        commander = pawn;
        hasCommander = true;
    }
    protected virtual void Update()
    {
        if ((!needMoreTarget && !hasTarget) ||
            (hasTarget && (!target.Health.IsAlive || target.Health.Team.Equals(controlledPawn.Health.Team) || Time.time >= nextClosest)))        
        {
            ChangeTarget();
            nextClosest = Time.time + timeToGetClosest;
        }
        if (!hasTarget && hasCommander)
        {
            if (commander.Health.IsAlive)
            {
                float distance = (commander.transform.position - controlledPawn.transform.position).sqrMagnitude;
                float multiply = distance/(distanceToCommander * distanceToCommander);
                if (multiply > 5.0f)
                    multiply = 5.0f;
                if (distance > distanceToCommander * distanceToCommander)
                {
                    Vector3 direction = (commander.transform.position - controlledPawn.transform.position).normalized;


                    character.CharacterMovement.speedMultiply = multiply;
                    
                    
                    character.CharacterMovement.FollowTarget(commander, distance, direction);
                }
                else
                {
                    //multiply *= 0.25f;
                    //character.CharacterMovement.Stop();
                    character.CharacterMovement.speedMultiply = multiply;
                }
            }
            else
            {
                hasCommander = false;
                commander = null;
            }
        }
        else
        {
            character.CharacterMovement.speedMultiply = 1.0f;
        }
    }
    protected void LateUpdate()
    {
        if(hasTarget && !target.Health.IsAlive)
        {
            target = null;
            hasTarget = false;
        }
        for (int i = 0; i < targets.Count; i++)
        {
            if(!targets[i].Health.IsAlive)
            {
                targets.RemoveAt(i);
                i--;
            }
            if (needMoreTarget)
                needMoreTarget = controlledPawn.Health.Team.Equals(targets[i].Health.Team);

        }       
    }
    protected void ChangeTarget()
    {
        
        if (!hasTarget)
        {
            if (!ClosestTarget(ref target))
            {
                hasTarget = false;
                needMoreTarget = true;

                return;
            }
        }
        else
        {
            if(target.Health.Team.Equals(controlledPawn.Health.Team))
            {
                if (!ClosestTarget(ref target))
                {
                    hasTarget = false;
                    needMoreTarget = true;
                    return;
                }
            }
        }
        while (!target.Health.IsAlive)
        {
            targets.Remove(target);
            if (!ClosestTarget(ref target))
            {
                hasTarget = false;
                needMoreTarget = true;
                return;
            }
        }
        hasTarget = true;
        return;
    }
    protected bool ClosestTarget(ref Pawn pawn)
    {
        float minM = float.MaxValue;
        float minTmp;
        bool hasTarget = false;
        for (int i = 0; i < targets.Count; i++)
        {
            if (!controlledPawn.Health.Team.Equals(targets[i].Health.Team) && targets[i].Health.Team != Team.world)
            {
                minTmp = (targets[i].transform.position - controlledPawn.transform.position).sqrMagnitude;
                if (minM > minTmp)
                {
                    hasTarget = true;
                    minM = minTmp;
                    pawn = targets[i];
                }
            }
        }
        InCombat(hasTarget);
        return hasTarget;
    }
    protected virtual void OnPerceptionDetection(StimulusStruct stimulus)
    {
        if (stimulus.enter)
        {
            if (!targets.Contains(stimulus.target))
            {
                targets.Add(stimulus.target);
                needMoreTarget = false;              
            }
        }
        else
        {
            if (hasTarget && target.Equals(stimulus.target))
            {
                targets.Remove(target);
                hasTarget = false;            
                target = null;
            }
            else
            {
                if (targets.Contains(stimulus.target))
                {
                    targets.Remove(stimulus.target);
                }
            }
        }
    }
    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        jsonObject.Add("hasTarget", new JSONBool(hasTarget));
        if (hasTarget)
            SaveSystem.ComponentReferenceSave(jsonObject, "current", target);
        jsonObject.Add("needMoreTarget", new JSONBool(needMoreTarget));
        jsonObject.Add("hasCommander", new JSONBool(hasCommander));
        if (hasCommander)
            SaveSystem.ComponentReferenceSave(jsonObject, "commander", commander);
        SaveSystem.TimerSave(jsonObject, "closest", nextClosest);
        JSONArray targetsJArray = new JSONArray();
        {
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i].Health.IsAlive)
                {
                    JSONObject targetJObject = new JSONObject();
                    SaveSystem.ComponentReferenceSave(targetJObject, "target", targets[i]);
                    targetsJArray.Add(targetJObject);
                }
            }
        }
        jsonObject.Add("targets", targetsJArray);
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        hasTarget = jsonObject["hasTarget"].AsBool;
        if (hasTarget)
        {
            inCombat = hasTarget = SaveSystem.ComponentReferenceLoad(jsonObject, "current", ref target);
        }         
        hasCommander = jsonObject["hasCommander"].AsBool;
        needMoreTarget = jsonObject["needMoreTarget"].AsBool;
        if (hasCommander)
        {
            
            hasCommander = SaveSystem.ComponentReferenceLoad(jsonObject, "commander", ref commander);
            Debug.Log("Has " + commander.name);
        }
        SaveSystem.TimerLoad(jsonObject, "closest", ref nextClosest);
        targets.Clear();
        JSONArray targetsJArray = jsonObject["targets"].AsArray;
        {
            for (int i = 0; i < targetsJArray.Count; i++)
            {
                JSONObject targetJObject = targetsJArray[i].AsObject;
                Pawn tar = default;
                SaveSystem.ComponentReferenceLoad(targetJObject, "target", ref tar);
                if(tar != null)
                    targets.Add(tar);
            }
        }
        return jsonObject;
    }
}
