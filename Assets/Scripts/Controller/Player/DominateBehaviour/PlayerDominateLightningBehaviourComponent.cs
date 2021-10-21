using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDominateLightningBehaviourComponent : PlayerDominateBehaviourBaseComponent
{
    protected CharacterAttacker[] character = new CharacterAttacker[2];
    protected Vector3[] target = new Vector3[2];
    [SerializeField]
    protected float damage = 100.0f;
    [SerializeField]
    protected float distanceToTarget = 2.0f;
    [SerializeField]
    protected Vector2 lightningSize;
    protected int characterCount;
    [SerializeField]
    protected DynamicExecutor dynamicLightning;
    [SerializeField]
    protected float timeToLightning, timeToKill = 5.0f;
    protected float nextLightning, nextKill;


    public override bool Ready()
    {
        return controller.TargetCount >= 2 && !hasCharacter && !hasTarget;
    }
    public override void Execute(int action)
    {
        if (action == 0 && Ready())
        {
            characterCount = 0;
            if (controller.ClosestCharacter(ref character[0], controller.GetMousePosition()) && character[0].HasController)
            {
                characterCount = 1;
                target[0] = controller.GetMousePosition();
            }
            else
            {
                characterCount = 0;
                hasCharacter = false;
                hasTarget = false;
            }

            return;
        }
        if (action == 1 && Ready())
        {
            if (characterCount == 1 && 
                character[0].gameObject.activeSelf && 
                controller.ClosestCharacter(ref character[1], character[0], controller.GetMousePosition()) && character[1].HasController && character[0].HasController)
            {

                controller.RemoveTarget(character[0]);
                character[0].Controller.Unpossess();
                character[0].Health.FullResistance = 0.75f;
                character[0].SpriteRendererHolder.OnLightning();
                character[0].CharacterMovement.Stop();

                controller.RemoveTarget(character[1]);
                character[1].Controller.Unpossess();
                character[1].Health.FullResistance = 0.75f;
                character[1].SpriteRendererHolder.OnLightning();          
                character[1].CharacterMovement.Stop();

                nextKill = Time.time + timeToKill;
                hasCharacter = true;
                target[1] = controller.GetMousePosition();
                hasTarget = true;
            }
            else
            {
                characterCount = 0;
                hasCharacter = false;
                hasTarget = false;
            }

            return;
        }
        Stop();
    }
    public override bool OnUpdate()
    {
        if (CharactersIsAlive())
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

    public override bool OnLateUpdate()
    {
        if (!CharactersIsAlive() || Time.time >= nextKill)
        {
            hasTarget = false;
            hasCharacter = false;
            for (int i = 0; i < character.Length; i++)
            {
                if (character[i].Health.IsAlive)
                    character[i].Health.Kill();
            }
            return true;
        }
        return false;
    }
    public override void Stop()
    {
        hasTarget = false;
        hasCharacter = false;
        characterCount = 0;
    }

    protected bool CharactersIsAlive()
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
        IPoolDynamicLineRenderer ipdlr = GameInstance.Instance.PoolManager.Pop(dynamicLightning) as IPoolDynamicLineRenderer;
        ipdlr.SetPosition(Vector3.Lerp(this.character[0].transform.position, this.character[1].transform.position, 0.5f));
        ipdlr.SetPoint(0, character[0].gameObject);
        ipdlr.SetPoint(1, character[1].gameObject);
        float distinace = (character[1].transform.position - character[0].transform.position).magnitude + lightningSize.x;
        Vector3 direction = (character[1].transform.position - character[0].transform.position).normalized;
        Vector3 position = character[0].transform.position - direction * lightningSize.x;
        RaycastHit2D[] hits = Physics2D.BoxCastAll(position, lightningSize, 0, direction, distinace, LayerMask.GetMask("Pawn"));
        for (int i = 0; i < hits.Length; i++)
        {
            IHealth health = hits[i].collider.GetComponent<IHealth>();
            if (health != null && !health.Team.Equals(character[0].Health.Team))
            {
                health.Hurt(new DamageStruct(character[0].gameObject, character[0].Health.Team, damage, Vector3.zero, 0.0f), hits[i]);
            }
        }
    }
    public override JSONObject Save(JSONObject jsonObject)
    {
        base.Save(jsonObject);
        jsonObject.Add("hasCharacter", new JSONBool(hasCharacter));
        if (hasCharacter)
        {
            JSONArray charactersJArray = new JSONArray();
            {
                for (int i = 0; i < character.Length; i++)
                {
                    charactersJArray.Add(SaveSystem.ComponentReferenceSave(new JSONObject(), "character", character[i]));
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
        SaveSystem.TimerSave(jsonObject, "lightning", nextLightning);
        SaveSystem.TimerSave(jsonObject, "kill", nextKill);
        return jsonObject;
    }
    public override JSONObject Load(JSONObject jsonObject)
    {
        base.Load(jsonObject);
        hasCharacter = jsonObject["hasCharacter"].AsBool;
        if (hasCharacter)
        {
            JSONArray charactersJArray = jsonObject["characters"].AsArray;
            {
                for (int i = 0; i < charactersJArray.Count; i++)
                {
                    CharacterAttacker character = default;
                    if (hasCharacter = SaveSystem.ComponentReferenceLoad(charactersJArray[i].AsObject, "character", ref character))
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
        SaveSystem.TimerLoad(jsonObject, "lightning", ref nextLightning);
        SaveSystem.TimerLoad(jsonObject, "kill", ref nextKill);
        return jsonObject;
    }
}
