using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDominateLightningBehaviourComponent2 : PlayerDominateBehaviourBaseComponent2
{

    [SerializeField]
    protected float damage = 100.0f;
    [SerializeField]
    protected float distanceToTarget = 2.0f;
    [SerializeField]
    protected Vector2 lightningSize;
    protected CharacterAttacker[] character = new CharacterAttacker[2];
    protected Vector3[] target = new Vector3[2];
    protected int currentCharacter, currentTarget;
    [SerializeField]
    protected DynamicExecutor dynamicLightning;
    [SerializeField]
    protected float timeToLightning, timeToKill = 5.0f;
    protected float nextLightning, nextKill;

    public override void SetCharacter(CharacterAttacker character)
    {
        if( currentCharacter == 0)
        {
            this.character[0] = character;
            currentCharacter = 1;
        }
        else
        {
            if(currentCharacter == 1)
            {
                this.character[1] = character;
                this.character[0].Health.FullResistance = 0.75f;
                this.character[1].Health.FullResistance = 0.75f;
                this.character[0].SpriteRendererHolder.OnLightning();
                this.character[1].SpriteRendererHolder.OnLightning();
                this.character[0].CharacterMovement.Stop();
                this.character[1].CharacterMovement.Stop();
                currentCharacter = 0;
                //nextLightning = Time.time + timeToLightning;
                nextKill = Time.time + timeToKill;
                hasCharacter = true;
            }
        }
    }

    public override void SetTarget(Pawn target)
    {
        return;
    }

    public override void SetTarget(Vector3 target)
    {
        {
            if (currentTarget == 0)
            {
                this.target[0] = target;
                currentTarget = 1;
            }
            else
            {
                if (currentTarget == 1)
                {
                    this.target[1] = target;
                    currentTarget = 0;
                    hasTarget = true;
                }
            }
        }
    }
    public override bool GetCharacter(ref CharacterAttacker character)
    {
        if (currentCharacter == 1)
        {
            character = this.character[0];
            return true;
        }
        else
        {
            character = this.character[1];
            return false;
        }
    }
    public override bool Execute()
    {
        if (hasCharacter && CharactersIsAlive())
        {
            if (Time.time >= nextLightning)
            {
                Lightning();
                nextLightning = Time.time + timeToLightning;
            }
            for (int i = 0; i < character.Length; i++)
            {
                float distance = (target[i] - character[i].transform.position).sqrMagnitude;
                if (distance > distanceToTarget * distanceToTarget)
                {
                    character[i].CharacterMovement.speedMultiply = 2.0f;
                    character[i].CharacterMovement.MoveTo(target[i]);
                }
                else
                {
                    return true;
                }

            }
        }
        return false;
    }
    public override bool LateExecute()
    {
        if ((hasCharacter && !CharactersIsAlive()) || Time.time >= nextKill)
        {
            hasTarget = false;
            hasCharacter = false;
            for (int i = 0; i < character.Length; i++)
            {
                character[i].Health.Kill();
            }         
            return true;
        }
        return false;
    }

    protected  bool CharactersIsAlive()
    {
        for (int i = 0; i < character.Length; i++)
        {
            if (!character[i].Health.IsAlive)
            {
                return false;
            }
        }
        return true;
    }
    protected void Lightning()
    {
        //Debug.Log("call");
        IPoolDynamicLineRenderer ipdlr = GameInstance.Instance.PoolManager.Pop(dynamicLightning) as IPoolDynamicLineRenderer;
        ipdlr.SetPosition(Vector3.Lerp(this.character[0].transform.position, this.character[1].transform.position, 0.5f));
        ipdlr.SetPoint(0, character[0].gameObject);
        ipdlr.SetPoint(1, character[1].gameObject);

        float distinace = (character[1].transform.position - character[0].transform.position).magnitude + lightningSize.x;
        Vector3 direction = (character[1].transform.position - character[0].transform.position).normalized;
        Vector3 position = character[0].transform.position - direction * lightningSize.x;
        RaycastHit2D[] hits = Physics2D.BoxCastAll(position, lightningSize, 0, direction, distinace, LayerMask.GetMask("Pawn"));
        //Debug.Log(hits.Length);
        for (int i = 0; i < hits.Length; i++)
        {
            IHealth health = hits[i].collider.GetComponent<IHealth>();
            if (health != null && !health.Team.Equals(character[0].Health.Team))
            {
                health.Hurt(new DamageStruct(character[0].gameObject, character[0].Health.Team, damage, Vector3.zero, 0.0f), hits[i]);
            }
        }
    }
    public override void Stop()
    {
        hasTarget = false;
        hasCharacter = false;
        currentCharacter = 0;
        currentTarget = 0;
    }
    public override JSONObject Save( JSONObject jsonObject)
    {
        jsonObject.Add("hasCharacter", new JSONBool(hasCharacter));
        if (hasCharacter)
        {
            JSONArray charactersJArray = new JSONArray();
            {
                for (int i = 0; i < character.Length; i++)
                {
                    JSONObject characterJObject = new JSONObject();
                    SaveSystem.ComponentReferenceSave(characterJObject, "character", character[i]);
                    charactersJArray.Add(characterJObject);
                }              
            }
            jsonObject.Add("characters", charactersJArray);
        }
        jsonObject.Add("hasTarget", new JSONBool(hasTarget));
        if (hasTarget)
        {
            JSONArray targetsJArray = new JSONArray();
            for (int i = 0; i < target.Length; i++)
            {
                JSONArray targetJArray = new JSONArray();
                targetJArray.Add(new JSONNumber(target[i].x));
                targetJArray.Add(new JSONNumber(target[i].y));
                targetJArray.Add(new JSONNumber(target[i].z));
                targetsJArray.Add(targetJArray);
            }
            jsonObject.Add("targets", targetsJArray);
        }
        SaveSystem.TimerSave(jsonObject, "kill", nextKill);
        SaveSystem.TimerSave(jsonObject, "lightning", nextLightning);
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        hasCharacter = jsonObject["hasCharacter"].AsBool;
        if (hasCharacter)
        {
            JSONArray charactersJArray = jsonObject["characters"].AsArray;
            {
                for (int i = 0; i < charactersJArray.Count; i++)
                {
                    JSONObject characterJObject = charactersJArray[i].AsObject;
                    CharacterAttacker character = default;
                    hasCharacter = SaveSystem.ComponentReferenceLoad(characterJObject, "character", ref character);
                    this.character[i] = character;
                }
            }  
        }
        hasTarget = jsonObject["hasTarget"].AsBool;
        if (hasTarget)
        {
            JSONArray targetsJArray = jsonObject["targets"].AsArray;
            for (int i = 0; i < targetsJArray.Count; i++)
            {
                JSONArray targetJArray = targetsJArray[i].AsArray;
                target[i].Set(targetJArray[0].AsFloat, targetJArray[1].AsFloat, targetJArray[2].AsFloat);
            }
        }
        SaveSystem.TimerLoad(jsonObject, "kill",ref nextKill);
        SaveSystem.TimerLoad(jsonObject, "lightning",ref nextLightning);
        return jsonObject;
    }
}
