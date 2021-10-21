using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDominateHealBehaviourComponent : PlayerDominateBehaviourBaseComponent
{
    [SerializeField]
    protected float heal = 25.0f;
    [SerializeField]
    protected float distanceToHeal = 3.0f;
    protected CharacterAttacker character;
    protected Pawn target;
    [SerializeField]
    protected DynamicExecutor dynamicHeal;
    [SerializeField]
    protected float timeToKill = 10.0f;
    protected float nextKill;

    public override bool Ready()
    {
        return controller.TargetCount >= 1 && !hasCharacter && !hasTarget;
    }
    public override void Execute(int action)
    {
        if (action == 0 && controller.TargetCount >= 1)
        {
            target = controller.ControlledPawn;
            if (controller.ClosestCharacter(ref character, target.transform.position) && character.HasController)
            {
                hasCharacter = true;
                hasTarget = true;
                controller.RemoveTarget(character);
                character.Controller.Unpossess();
                character.Health.FullResistance = 0.5f;
                character.SpriteRendererHolder.OnHeal();
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
    public override bool OnUpdate()
    {
        if (hasCharacter && character.Health.IsAlive && hasTarget && target.Health.IsAlive)
        {
            float distance = (target.transform.position - character.transform.position).sqrMagnitude;
            Vector3 direction = (target.transform.position - character.transform.position).normalized;
            if (distance > distanceToHeal * distanceToHeal)
            {
                character.CharacterMovement.speedMultiply = 4.0f;
                character.CharacterMovement.FollowTarget(target, distance, direction);
            }
            else
            {
                target.Health.Heal(heal);
                IPoolObject ipo =  GameInstance.Instance.PoolManager.Pop(dynamicHeal);
                ipo.SetParent(target.transform);
                ipo.Transform.localPosition = Vector3.zero;
                character.Health.Kill();
                hasCharacter = false;
                hasTarget = false;
                character = null;
                return true;
            }

        }
        return false;
    }
    public override bool OnLateUpdate()
    {
        if ((hasTarget && !target.Health.IsAlive) || (hasCharacter && !character.Health.IsAlive) || Time.time >= nextKill)
        {
            hasTarget = false;
            target = null;
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
        SaveSystem.TimerLoad(jsonObject, "kill", ref nextKill);
        return jsonObject;
    }
}
