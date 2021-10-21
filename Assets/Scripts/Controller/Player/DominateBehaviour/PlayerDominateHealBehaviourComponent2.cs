using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDominateHealBehaviourComponent2 : PlayerDominateBehaviourBaseComponent2
{
    [SerializeField]
    protected float heal = 25.0f;
    [SerializeField]
    protected float distanceToHeal = 3.0f;
    protected CharacterAttacker character;
    protected Pawn target;
    [SerializeField]
    protected float timeToKill = 10.0f;
    protected float nextKill;

    public override void SetCharacter(CharacterAttacker character)
    {
        hasCharacter = true;
        this.character = character;
        this.character.Health.FullResistance = 0.5f;
        this.character.SpriteRendererHolder.OnHeal();
        this.character.CharacterMovement.Stop();
        nextKill = Time.time + timeToKill;
    }
    public override void SetTarget(Pawn target)
    {
        hasTarget = true;
        this.target = target;

    }
    public override bool GetCharacter(ref CharacterAttacker character)
    {
        if (hasCharacter)
        {
            character = this.character;
            return true;
        }
        else
        {
            return false;
        }
    }
    public override void SetTarget(Vector3 target)
    {
        return;
    }
    public override bool Execute()
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
                character.Health.Kill();
                hasCharacter = false;
                hasTarget = false;
                character = null;
                return true;
            }

        }
        return false;
    }
    public override bool LateExecute()
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
    public override JSONObject Save( JSONObject jsonObject)
    {
        jsonObject.Add("hasCharacter", new JSONBool(hasCharacter));
        if (hasCharacter)
            SaveSystem.ComponentReferenceSave(jsonObject, "character", character);
        jsonObject.Add("hasTarget", new JSONBool(hasTarget));
        if (hasTarget)
            SaveSystem.ComponentReferenceSave(jsonObject, "target", target);
        SaveSystem.TimerSave(jsonObject, "kill", nextKill);
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
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
