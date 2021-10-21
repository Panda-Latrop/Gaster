using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDominateBombBehaviourComponent2 : PlayerDominateBehaviourBaseComponent2
{
    [SerializeField]
    protected float damage = 100.0f, power = 200.0f, radius = 5.0f;
    [SerializeField]
    protected float distanceToBomb = 2.0f;
    [SerializeField]
    protected DynamicExecutor dynamicExplosion;
    protected CharacterAttacker character;
    protected Vector3 target;
    [SerializeField]
    protected float timeToKill = 5.0f;
    protected float nextKill;

    public override void SetCharacter(CharacterAttacker character)
    {
        hasCharacter = true;
        this.character = character;
        this.character.Health.FullResistance = 0.5f;
        this.character.useHitEvent = true;
        this.character.BindOnOnHit(OnHit);
        this.character.SpriteRendererHolder.OnBomb();
        this.character.CharacterMovement.Stop();
        nextKill = Time.time + timeToKill;
    }
    public override void SetTarget(Pawn target)
    {
        return;
    }
    public override void SetTarget(Vector3 target)
    {
        hasTarget = true;
        this.target = target;
        return;
    }
    public override bool GetCharacter(ref CharacterAttacker character)
    {
        if(hasCharacter){
            character = this.character;
            return true;
        }
        else
        {
            return false;
        }
    }
    public override bool Execute()
    {
        if (hasCharacter && character.Health.IsAlive)
        {
            float distance = (target - character.transform.position).sqrMagnitude;
            if (distance > distanceToBomb * distanceToBomb)
            {
                character.CharacterMovement.speedMultiply = 4.0f;
                character.CharacterMovement.MoveTo(target);
            }
            else
            {
                Explosion();
                return true;
            }

        }
        return false;
    }
    public override bool LateExecute()
    {
        if (hasCharacter && !character.Health.IsAlive || Time.time >= nextKill)
        {
            Explosion();
            return true;
        }
        return false;
    }

    protected void Explosion()
    {
        hasTarget = false;
        hasCharacter = false;
        IPoolObject ipp = GameInstance.Instance.PoolManager.Pop(dynamicExplosion);
        ipp.SetPosition(character.transform.position);
        new ExplosionObject(character.transform.position, radius, character.gameObject,character.Health.Team,damage, power);
        character.Health.Kill();
        character = null;
    }

    protected void OnHit(Collision2D collision)
    {
        if (hasCharacter && character.Health.IsAlive && collision.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Pawn")))
        {
            IHealth health = collision.collider.GetComponent<IHealth>();
            if (health != null && !health.Team.Equals(character.Health.Team))
            {
                Explosion();
            }
        }
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
        {
            JSONArray targetJArray = new JSONArray();
            targetJArray.Add(new JSONNumber(target.x));
            targetJArray.Add(new JSONNumber(target.y));
            targetJArray.Add(new JSONNumber(target.z));
            jsonObject.Add("target", targetJArray);
        }
        SaveSystem.TimerSave(jsonObject, "kill", nextKill);  
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        hasCharacter = jsonObject["hasCharacter"].AsBool;
        if (hasCharacter)
        {
            if(hasCharacter = SaveSystem.ComponentReferenceLoad(jsonObject, "character", ref character))
            {
                character.BindOnOnHit(OnHit);
            }
        }
        hasTarget = jsonObject["hasTarget"].AsBool;
        if (hasTarget)
        {
            JSONArray targetJArray = jsonObject["target"].AsArray;
            target.Set(targetJArray[0].AsFloat, targetJArray[1].AsFloat, targetJArray[2].AsFloat);           
        }
        SaveSystem.TimerLoad(jsonObject, "kill", ref nextKill);
        return jsonObject;
    }
}