using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControllerBossDummyGuard : AIControllerAttacker
{
    protected int attackType = 0, actionStage = 0;
    protected State attackState = State.enter;
    protected ShootState shootState;
    [SerializeField]
    protected float distanceToClose = 3.0f;
    protected bool closeToTarget;
    [SerializeField]
    protected int shootInStage0, shootInStage1, shootInStage2;
    protected bool toStage;
    protected int shootCount;
    [SerializeField]
    protected float timeToWait = 1.0f,timeToVoice,timeToShield;
    protected float nextWait,nextVoice,nextShield;
    //protected int animAttackHash, animIdleHash;
    protected Vector3 lastTargetPos;
    protected WeaponBase weaponShield;
    [SerializeField]
    protected AIControllerVoiceComponent voice;
    /* public void Start()
     {
         animAttackHash = Animator.StringToHash("Attack");
         animIdleHash = Animator.StringToHash("Idle");
     }*/

    public override void Possess(Pawn pawn)
    {
        base.Possess(pawn);
        weaponShield = character.WeaponHolderComponent.GetWeapon(2);
        voice.SetAudioSource(character.VoiceSource);
        nextVoice = Time.time + timeToVoice * 0.5f;
        nextShield = Time.time + timeToShield*0.5f;
    }
    protected override void OnHurt(DamageStruct ds, RaycastHit2D raycastHit)
    {    }
    protected override void OnDeath(DamageStruct ds, RaycastHit2D raycastHit)
    {        base.OnDeath(ds, raycastHit);    }
    protected override void OnMorale(DamageStruct ds, RaycastHit2D raycastHit)
    {    }
    protected override void Update()
    {
        base.Update();
        if (hasTarget)
        {
            //shootState = ShootState.none;
            Vector3 target = this.target.transform.position;
            float distance = (target - character.transform.position).sqrMagnitude;
            Vector3 direction = (target - character.transform.position).normalized;
            closeToTarget = distance <= distanceToClose * distanceToClose;

            switch (attackType)
            {
                case 0:
                    character.CharacterMovement.speedMultiply = 1.25f;
                    switch (attackState)
                    {
                        case State.enter:
                            //if ((Vector3.zero - controlledPawn.transform.position).sqrMagnitude <= 1.0f)
                            //    actionStage = 3;

                            attackState = State.process;
                            break;
                        case State.process:

                            switch (actionStage)
                            {
                                case 0:
                                    nextWait = Time.time + timeToWait;
                                    actionStage = 1;
                                    break;
                                case 1:
                                    if (CanWait())
                                        actionStage = 2;
                                    break;
                                case 2:
                                    character.WeaponHolderComponent.SetFire(true);
                                    character.WeaponHolderComponent.SetTarget(this.target.transform);
                                    shootState = character.WeaponHolderComponent.Shoot(character.WeaponHolderComponent.GetShootPoint(), direction);
                                    //if (shootState.Equals(ShootState.initiated))
                                    //    Laught();
                                    if (shootState.Equals(ShootState.ended))
                                    {
                                        shootCount++;
                                    }
                                    if (shootCount >= shootInStage0 && shootState.Equals(ShootState.unready))
                                    {
                                        toStage = true;
                                    }
                                    break;
                                default:
                                    break;
                            }

                            break;
                        case State.exit:
                            break;
                    }
                    if (!closeToTarget)
                    {
                        character.CharacterMovement.FollowTarget(this.target, distance, direction);
                    }
                    else
                    {
                        character.CharacterMovement.Stop();
                    }
                    break;
                case 1:
                    switch (attackState)
                    {
                        case State.enter:
                            attackState = State.process;
                            break;
                        case State.process:

                            switch (actionStage)
                            {
                                case 0:
                                    nextWait = Time.time + timeToWait;
                                    actionStage = 1;
                                    break;
                                case 1:
                                    if (CanWait())
                                        actionStage = 2;
                                    if (!closeToTarget)
                                    {
                                        character.CharacterMovement.FollowTarget(this.target, distance, direction);
                                    }
                                    else
                                    {
                                        character.CharacterMovement.Stop();
                                    }
                                    break;
                                case 2:
                                    Vector3 dir = (target - lastTargetPos).normalized;
                                    float dist = (target - lastTargetPos).magnitude;
                                    float velo = dist / Time.deltaTime;
                                    lastTargetPos = target + dir * velo * 0.25f;
                                    character.CharacterMovement.Stop();
                                    character.WeaponHolderComponent.SetFire(true);
                                    character.WeaponHolderComponent.SetTarget(lastTargetPos);
                                    shootState = character.WeaponHolderComponent.Shoot(character.WeaponHolderComponent.GetShootPoint(), direction);
                                    if (shootState.Equals(ShootState.ended))
                                    {
                                        shootCount++;
                                    }
                                    if (shootState.Equals(ShootState.unready))
                                    {
                                        if (shootCount >= shootInStage1)
                                        {
                                            toStage = true;
                                        }
                                    }
                                    lastTargetPos = target;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case State.exit:
                            break;
                    }
                    break;
                default:
                    break;
            }

            if (CanVoice())
            {
                voice.PlayBattleCries();
            }
            if (Time.time >= nextShield)
            {
                weaponShield.SetFire(true);
                ShootState shieldState = weaponShield.Shoot(transform.position, direction);
                if (shieldState.Equals(ShootState.initiated))
                {
                    voice.PlayHurtCries();
                }
                if (shieldState.Equals(ShootState.ended))
                {
                    nextShield = Time.time + timeToShield;
                }
            }
            if (toStage && (shootState.Equals(ShootState.unready) || shootState.Equals(ShootState.ended)))
            {
                ChangeAttackType();
            }
        }
    }
    protected bool CanWait()
    {
        if (Time.time >= nextWait)
        {
            nextWait = Time.time + timeToWait;
            return true;
        }
        return false;
    }
    protected bool CanVoice()
    {
        if (Time.time >= nextVoice)
        {
            nextVoice = Time.time + timeToVoice;
            return true;
        }
        return false;
    }
    protected void ChangeAttackType()
    {
        var stage = Random.Range(0, 2);
        if (attackType == stage)
        {
            if (++stage >= 2)
            {
                stage = 0;
            }
        }
        attackType = stage;
        attackState = State.enter;
        actionStage = 0;
        shootCount = 0;
        toStage = false;
        character.WeaponHolderComponent.ChangeSlot(stage);
    }
    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        jsonObject.Add("attackType", new JSONNumber(attackType));
        jsonObject.Add("actionStage", new JSONNumber(actionStage));
        jsonObject.Add("attackState", new JSONNumber((int)attackState));
        jsonObject.Add("shootState", new JSONNumber((int)shootState));
        jsonObject.Add("toStage", new JSONBool(toStage));
        jsonObject.Add("shootCount", new JSONNumber(shootCount));
        SaveSystem.TimerSave(jsonObject, "wait", nextWait);
        SaveSystem.TimerSave(jsonObject, "voice", nextVoice);
        SaveSystem.TimerSave(jsonObject, "shield", nextShield);
        JSONArray lastJArray = new JSONArray();
        {
            lastJArray.Add(new JSONNumber(lastTargetPos.x));
            lastJArray.Add(new JSONNumber(lastTargetPos.y));
            lastJArray.Add(new JSONNumber(lastTargetPos.z));
        }
        jsonObject.Add("last", lastJArray);
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        attackType = jsonObject["attackType"].AsInt;
        actionStage = jsonObject["actionStage"].AsInt;
        attackState = (State)jsonObject["attackState"].AsInt;
        shootState = (ShootState)jsonObject["shootState"].AsInt;
        toStage = jsonObject["toStage"].AsBool;
        shootCount = jsonObject["shootCount"].AsInt;
        SaveSystem.TimerLoad(jsonObject, "wait", ref nextWait);
        SaveSystem.TimerLoad(jsonObject, "voice", ref nextVoice);
        SaveSystem.TimerLoad(jsonObject, "shield", ref nextShield);
        JSONArray lastJArray = jsonObject["last"].AsArray;
        lastTargetPos.Set(lastJArray[0].AsFloat, lastJArray[1].AsFloat, lastJArray[2].AsFloat);
        return jsonObject;
    }
}
