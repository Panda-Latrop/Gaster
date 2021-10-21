using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDominateDamageBehaviourComponent : PlayerDominateBehaviourBaseComponent
{
    [SerializeField]
    protected float damageMultiply = 2.0f;
    [SerializeField]
    protected float distanceToDamage = 3.0f;
    protected CharacterAttacker character;
    protected CharacterPlayer target;
    [SerializeField]
    protected DynamicExecutor dynamicDamage;
    [SerializeField]
    protected float TimeToKeepMultiply = 5.0f, timeToDamage = 5.0f,timeToKill = 10.0f;
    protected float nextDamage,nextKill;
    protected bool isDamageSet = false;

    public override bool Ready()
    {
        return controller.TargetCount >= 1 && Time.time >= nextDamage && !hasCharacter && !hasTarget;
    }

    public override void Execute(int action)
    {
        if (action == 0 && Ready())
        {
            target = controller.Player;
            if (controller.ClosestCharacter(ref character, target.transform.position) && character.HasController)
            {
                hasCharacter = true;
                hasTarget = true;
                controller.RemoveTarget(character);
                character.Controller.Unpossess();
                character.Health.FullResistance = 0.5f;
                character.SpriteRendererHolder.OnRage();
                character.CharacterMovement.Stop();
                nextKill = Time.time + timeToKill;
            }
            else
            {
                hasCharacter = false;
                hasTarget = false;
            }
        }
    }
    public override bool CanUpdate()
    {
        return (hasCharacter && hasTarget) || isDamageSet;
    }
    public override bool OnUpdate()
    {
        if (hasCharacter && character.Health.IsAlive && hasTarget && target.Health.IsAlive && !isDamageSet)
        {
            //Debug.Log(hasCharacter && character.Health.IsAlive && hasTarget && target.Health.IsAlive && !isDamageSet);
            float distance = (target.transform.position - character.transform.position).sqrMagnitude;
            Vector3 direction = (target.transform.position - character.transform.position).normalized;
            if (distance > distanceToDamage * distanceToDamage)
            {
                character.CharacterMovement.speedMultiply = 4.0f;
                character.CharacterMovement.FollowTarget(target, distance, direction);
            }
            else
            {
                target.WeaponHolderComponent.SetDamageMultiply(damageMultiply);
                IPoolObject ipo = GameInstance.Instance.PoolManager.Pop(dynamicDamage);
                ipo.SetParent(target.transform);
                ipo.Transform.localPosition = Vector3.zero;
                character.Health.Kill();
                hasCharacter = false;
                character = null;
                isDamageSet = true;
                nextDamage = Time.time + TimeToKeepMultiply;
                return true;
            }


        }
        else
        {
           // Debug.Log(Time.time + " " + nextDamage);
            if (isDamageSet && Time.time >= nextDamage && hasTarget)
            {
                target.WeaponHolderComponent.SetDamageMultiply(1.0f);
                isDamageSet = false;
                nextDamage = Time.time + timeToDamage;
                hasTarget = false;
                return true;
            }
        }
        return false;
    }
    public override bool OnLateUpdate()
    {
        if (((hasTarget && !target.Health.IsAlive) || (hasCharacter && !character.Health.IsAlive) || Time.time >= nextKill) && !isDamageSet)
        {
            hasTarget = false;
            hasCharacter = false;
            hasTarget = false;
            character.Health.Kill();
            character = null;
            return true;
        }
        return false;
    }
    public override void Stop()
    {
        hasTarget = false;
        hasCharacter = false;
        character.Health.Kill();
        character = null;
    }
    public override JSONObject Save(JSONObject jsonObject)
    {
        base.Save(jsonObject);
        jsonObject.Add("hasCharacter", new JSONBool(hasCharacter));
        if (hasCharacter)
            SaveSystem.ComponentReferenceSave(jsonObject, "character", character);
        jsonObject.Add("hasTarget", new JSONBool(hasTarget));
        if (hasTarget)
            SaveSystem.ComponentReferenceSave(jsonObject, "target", target);
        jsonObject.Add("isDamageSet", new JSONBool(isDamageSet));
        SaveSystem.TimerSave(jsonObject, "damage", nextDamage);
        SaveSystem.TimerSave(jsonObject, "kill", nextKill);
        return jsonObject;
    }
    public override JSONObject Load(JSONObject jsonObject)
    {
        base.Load(jsonObject);
        hasCharacter = jsonObject["hasCharacter"].AsBool;
        if (hasCharacter)
            hasCharacter = SaveSystem.ComponentReferenceLoad(jsonObject, "character", ref character);
        hasTarget = jsonObject["hasTarget"].AsBool;
        if (hasTarget)
            hasTarget = SaveSystem.ComponentReferenceLoad(jsonObject, "target", ref target);
        isDamageSet = jsonObject["isDamageSet"].AsBool;
        SaveSystem.TimerLoad(jsonObject, "damage",ref nextDamage);
        SaveSystem.TimerLoad(jsonObject, "kill", ref nextKill);
        return jsonObject;
    }
}
