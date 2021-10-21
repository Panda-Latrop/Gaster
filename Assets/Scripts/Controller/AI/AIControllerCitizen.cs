using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControllerCitizen : AIControllerBase
{
    protected CharacterCitizen character;
    protected CharacterMovementComponent characterMovement;
    protected bool isPanic = false;
    [SerializeField]
    protected float timeToChangePath = 1.5f, timeToÑalm = 10.0f;
    [SerializeField]
    protected float distanceToPanicMove = 4.0f;
    [SerializeField]
    protected float distanceToFear = 4.0f;
    protected bool fearToTarget;
    protected bool hasAggressor;
    protected Pawn aggressor;
    protected float nextChange, nextÑalm;
    protected Vector2 randomDirection;
    public override void Possess(Pawn pawn)
    {
        base.Possess(pawn);
        character = pawn as CharacterCitizen;
        characterMovement = character.CharacterMovement;
    }
    protected override void OnHurt(DamageStruct ds, RaycastHit2D raycastHit)
    {
        if (!hasAggressor)
        {
            IPerceptionTarget ipt;
            if((ipt = ds.causer.GetComponent<IPerceptionTarget>()) != null)
            {
                aggressor = ipt.GetPawn();
                hasAggressor = true;
            }
        }
        character.Dialog.Close();
        isPanic = true;
        nextÑalm = Time.time + timeToÑalm;
    }
    protected override void OnMorale(DamageStruct ds, RaycastHit2D raycastHit)
    {
        controlledPawn.Health.Kill();
    }
    protected override void OnDeath(DamageStruct ds, RaycastHit2D raycastHit)
    {
    }
    protected void Update()
    {
        if (isPanic)
        {
            if (!fearToTarget)
            {
                if (Time.time >= nextChange)
                {
                    nextChange = Time.time + timeToChangePath;
                    randomDirection = Random.insideUnitCircle.normalized;
                }
                character.CharacterMovement.Move(randomDirection);
            }
            if (Time.time >= nextÑalm)
            {
                isPanic = false;
            }
        }
        if (hasAggressor)
        {
            Vector3 target = this.aggressor.transform.position;
            float distance = (target - character.transform.position).sqrMagnitude;
            if (distance <= distanceToFear * distanceToFear*2.0f)
            {
                isPanic = true;
                nextÑalm = Time.time + timeToÑalm;
            }
            if (fearToTarget)
            {
                if (distance >= distanceToFear * distanceToFear * 1.5f)
                    fearToTarget = false;
            }
            else
            {
                if (distance <= distanceToFear * distanceToFear)
                    fearToTarget = true;
            }

           
            if (fearToTarget)
            {
                Vector3 direction = (target - character.transform.position).normalized;
                character.CharacterMovement.Move(-direction);
            }
        }

    }
    protected void LateUpdate()
    {
        if (hasAggressor && !aggressor.Health.IsAlive)
        {
            aggressor = null;
            hasAggressor = false;
            isPanic = false;
            character.Dialog.Open();
        }
    }
    public override JSONObject Save(JSONObject jsonObject)
    {
        base.Save(jsonObject);
        SaveSystem.TimerSave(jsonObject, "change", nextChange);
        return jsonObject;
    }
    public override JSONObject Load(JSONObject jsonObject)
    {
        base.Load(jsonObject);
        SaveSystem.TimerLoad(jsonObject, "change", ref nextChange);
        return jsonObject;
    }
}
