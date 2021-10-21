using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AIControllerBossFlowey : AIControllerAttacker
{
    protected int attackStage = 0,actionStage = 0; 
    protected State stageState = State.enter;
    protected ShootState shootState;
    [SerializeField]
    protected int shootInStage0, shootInStage1, shootInStage2;
    protected bool toStage;
    protected int shootCount;
    [SerializeField]
    protected float radiusToAppear = 10.0f;
    [SerializeField]
    protected float timeToAppear = 1.0f, timeToWait = 1.0f;
    protected float timeMultiply = 1.0f;
    protected float nextAppear,nextWait;
    protected Vector3 lastTargetPos;
    [SerializeField]
    protected DynamicExecutor groundBreake, groundHole;
    protected int animHideHash, animUnhideHash, animLaughtHash, animDefaultHash;

    public void Start()
    {
        animHideHash = Animator.StringToHash("Hide");
        animUnhideHash = Animator.StringToHash("Unhide");
        animLaughtHash = Animator.StringToHash("ToLaugh");
        animDefaultHash = Animator.StringToHash("ToDefault");
    }

    protected override void OnHurt(DamageStruct ds, RaycastHit2D raycastHit)
    {
    }
    protected override void OnDeath(DamageStruct ds, RaycastHit2D raycastHit)
    {
        base.OnDeath(ds, raycastHit);
    }
    protected override void OnMorale(DamageStruct ds, RaycastHit2D raycastHit)
    {
    }
    protected override void Update()
    {
        base.Update();
        if (hasTarget)
        {
            
           //shootState = ShootState.none;
         
            Vector3 target = this.target.transform.position;
            float distance = (target - character.transform.position).sqrMagnitude;
            Vector3 direction = (target - character.transform.position).normalized;

            switch (attackStage)
            {
                case 0:
                    switch (stageState)
                    {
                        case State.enter:
                            if ((Vector3.zero - controlledPawn.transform.position).sqrMagnitude <= 1.0f)
                                actionStage = 3;
                            stageState = State.process;
                            break;
                        case State.process:

                            switch (actionStage)
                            {
                                case 0:
                                    Hide();
                                    nextAppear = Time.time + timeToAppear;
                                    actionStage = 1;
                                    break;
                                case 1:
                                    if (CanAppear())
                                    {
                                        character.CharacterMovement.Teleport(this.target.transform.position + Vector3.up*4.0f);//path need
                                        Unhide();
                                        nextWait = Time.time + timeToWait;
                                        actionStage = 2;
                                    }
                                    break;
                                case 2:
                                    if (CanWait())
                                        actionStage = 3;
                                    break;
                                case 3:
                                    character.WeaponHolderComponent.SetFire(true);
                                    character.WeaponHolderComponent.SetTarget(this.target.transform);
                                    shootState = character.WeaponHolderComponent.Shoot(character.WeaponHolderComponent.GetShootPoint(), direction);
                                    if (shootState.Equals(ShootState.initiated))
                                        Laught();
                                    if (shootState.Equals(ShootState.ended))
                                    {
                                        StopLaught();
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
                    break;
                case 1:
                    switch (stageState)
                    {
                        case State.enter:                         
                            stageState = State.process;
                            break;
                        case State.process:

                            switch (actionStage)
                            {
                                case 0:
                                    Hide();
                                    nextAppear = Time.time + timeToAppear;
                                    actionStage = 1;
                                    break;
                                case 1:
                                    if (CanAppear())
                                    {
                                        Vector3 rand = Random.insideUnitCircle * radiusToAppear;
                                        if (rand.sqrMagnitude < 2)
                                            rand *= 2;
                                        character.CharacterMovement.Teleport(target + rand);
                                        Unhide();
                                        nextWait = Time.time + timeToWait;
                                        actionStage = 2;
                                    }
                                    break;
                                case 2:
                                    if (CanWait())
                                        actionStage = 3;
                                    break;
                                case 3:
                                    character.WeaponHolderComponent.SetFire(true);
                                    character.WeaponHolderComponent.SetTarget(this.target.transform);
                                    shootState = character.WeaponHolderComponent.Shoot(character.WeaponHolderComponent.GetShootPoint(), direction);
                                    if (shootState.Equals(ShootState.initiated))
                                        Laught();
                                    if (shootState.Equals(ShootState.ended))
                                    {
                                        StopLaught();
                                        shootCount++;
                                    }
                                        
                                    if (shootState.Equals(ShootState.unready))
                                    {
                                        if (shootCount >= shootInStage1)
                                        {
                                            toStage = true;
                                        }
                                        else
                                        {
                                            actionStage = 4;
                                            nextWait = Time.time + timeToWait;
                                        }
                                    }
                                    break;
                                case 4:
                                    if (CanWait())
                                        actionStage = 0;
                                    break;
                                default:
                                    break;
                            }                            
                            break;
                        case State.exit:
                            break;
                    }
                    break;
                case 2:
                    switch (stageState)
                    {
                        case State.enter:
                            stageState = State.process;
                            break;
                        case State.process:
                            switch (actionStage)
                            {
                                case 0:
                                    Hide();
                                    nextAppear = Time.time + timeToAppear;
                                    actionStage = 1;
                                    break;
                                case 1:
                                    if (CanAppear())
                                    {
                                        Vector3 dir = (target - lastTargetPos).normalized;
                                        float dist = (target - lastTargetPos).magnitude;
                                        float velo = dist / Time.deltaTime;
                                        lastTargetPos = target + dir * velo * 0.25f;
                                        nextWait = Time.time + 0.25f;
                                        character.CharacterMovement.Teleport(lastTargetPos);//path need
                                        GameInstance.Instance.PoolManager.Pop(groundBreake).SetPosition(lastTargetPos);
                                        actionStage = 2;
                                    }
                                    else
                                    {
                                        lastTargetPos = target;
                                    }
                                    break;
                                case 2:
                                    if (CanWait())
                                        actionStage = 3;
                                    break;
                                case 3:                                    
                                    Unhide();
                                    GameInstance.Instance.PoolManager.Pop(groundHole).SetPosition(lastTargetPos);
                                    character.WeaponHolderComponent.SetFire(true);
                                    character.WeaponHolderComponent.SetTarget(lastTargetPos);
                                    shootState = character.WeaponHolderComponent.Shoot(character.WeaponHolderComponent.GetShootPoint(), direction);
                                    shootCount++;
                                    actionStage = 4;
                                    nextWait = Time.time + timeToWait;
                                    shootState = ShootState.ended;
                                    break;
                                case 4:
                                    if (CanWait())
                                    {
                                        if (shootCount >= shootInStage2)
                                        {
                                            toStage = true;
                                        }
                                        else
                                        {
                                            actionStage = 0;
                                        }
                                    }                                      
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

            if (toStage && (shootState.Equals(ShootState.unready) || shootState.Equals(ShootState.ended)))
            {
                ChangeAttackStage();
            }
        }
    }
    protected void Laught()
    {
        character.AnimationCharacterComponent.Play(animLaughtHash,1);
    }
    protected void StopLaught()
    {
        character.AnimationCharacterComponent.Play(animDefaultHash, 1);
    }
    protected void Unhide()
    {
        character.Capsule.enabled = true;
        character.AnimationCharacterComponent.Play(animUnhideHash);
    }
    protected void Hide()
    {
        character.Capsule.enabled = false;        
        character.AnimationCharacterComponent.Play(animHideHash);
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

    protected bool CanAppear()
    {
        if (Time.time >= nextAppear)
        {
            nextAppear = Time.time + timeToAppear;
            return true;
        }
        return false;
    }
    protected void ChangeAttackStage()
    {
        var stage = Random.Range(0, 3);
        if (attackStage == stage)
        {
            if(++stage >= 3)
            {
                stage = 0;
            }
        }
        attackStage = stage;
        stageState = State.enter;
        actionStage = 0;
        shootCount = 0;
        toStage = false;
        character.WeaponHolderComponent.ChangeSlot(stage);
    }
    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        jsonObject.Add("attackStage",new JSONNumber(attackStage));
        jsonObject.Add("actionStage", new JSONNumber(actionStage));
        jsonObject.Add("stageState", new JSONNumber((int)stageState));
        jsonObject.Add("shootState", new JSONNumber((int)shootState));
        jsonObject.Add("toStage", new JSONBool(toStage));
        jsonObject.Add("shootCount", new JSONNumber(shootCount));
        jsonObject.Add("timeMultiply", new JSONNumber(timeMultiply));
        SaveSystem.TimerSave(jsonObject, "appear", nextAppear);
        SaveSystem.TimerSave(jsonObject, "wait", nextWait);
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
        attackStage = jsonObject["attackStage"].AsInt;
        actionStage = jsonObject["actionStage"].AsInt;
        stageState = (State)jsonObject["stageState"].AsInt;
        shootState = (ShootState)jsonObject["shootState"].AsInt;
        toStage = jsonObject["toStage"].AsBool;
        shootCount = jsonObject["shootCount"].AsInt;
        timeMultiply = jsonObject["timeMultiply"].AsFloat;
        SaveSystem.TimerLoad(jsonObject, "appear", ref nextAppear);
        SaveSystem.TimerLoad(jsonObject, "wait", ref nextWait);
        JSONArray lastJArray = jsonObject["last"].AsArray;
        lastTargetPos.Set(lastJArray[0].AsFloat, lastJArray[1].AsFloat, lastJArray[2].AsFloat);
        return jsonObject;
    }
}
