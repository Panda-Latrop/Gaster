using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControllerSummoner : AIControllerAttacker
{
    [SerializeField]
    protected float distanceToAttack = 20.0f;
    [SerializeField]
    protected float distanceToFear = 5.0f;
    [SerializeField]
    protected float distanceToClose = 10.0f;
    [SerializeField]
    protected float distanceToFar= 15.0f;
    protected bool closeToTarget,fearToTarget;
    [SerializeField]
    protected int maxSummon;
    protected List<ProjectilePawnBase> summoneds = new List<ProjectilePawnBase>();


    public override void Possess(Pawn pawn)
    {
        base.Possess(pawn);
        WeaponSummoner summoner = character.WeaponHolderComponent.GetWeapon() as WeaponSummoner;
        summoner.BindOnSummoned(OnSummoned);
    }
    public override void Unpossess()
    {
        for (int i = 0; i < summoneds.Count; i++)
        {
            summoneds[i].Health.Kill();
        }
        base.Unpossess();
    }

    protected override void Update()
    {
        base.Update();
        if (hasTarget)
        {
            Vector3 target = this.target.transform.position;
            float distance = (target - character.transform.position).sqrMagnitude;
            Vector3 direction = (target - character.transform.position).normalized;
            if (distance <= distanceToAttack * distanceToAttack && summoneds.Count < maxSummon)
            {
                character.WeaponHolderComponent.SetFire(true);
                character.WeaponHolderComponent.Shoot(character.WeaponHolderComponent.GetShootPoint(), direction);
            }
            else
            {
                character.WeaponHolderComponent.SetFire(false);
            }

            if (distance <= distanceToFear * distanceToFear && !fearToTarget)
                fearToTarget = true;
            else
            {
                if (distance > distanceToClose * distanceToClose && fearToTarget)
                    fearToTarget = false;
            }
            if (distance <= distanceToClose * distanceToClose && !closeToTarget)
                closeToTarget = true;
            else
            {
                if (distance > distanceToFar * distanceToFar && closeToTarget)
                    closeToTarget = false;
            }
            if (!closeToTarget || fearToTarget)
            {

                if (fearToTarget)
                    character.CharacterMovement.Move(-direction);
                else
                {
                    if (!closeToTarget)
                    {
                        character.CharacterMovement.FollowTarget(this.target,distance, direction);
                    }
                }
            }
            else
                character.CharacterMovement.Stop();
        }
        for (int i = 0; i < summoneds.Count; i++)
        {
            if (!summoneds[i].Health.IsAlive)
            {
                summoneds.RemoveAt(i);
                i--;
                continue;
            }
            Vector3 direction;
            if (hasTarget)
                direction = (target.transform.position - summoneds[i].transform.position).normalized;
            else
                direction = (controlledPawn.transform.position - summoneds[i].transform.position).normalized;
            summoneds[i].Move(direction);
        }
    }
    protected override void OnDeath(DamageStruct ds, RaycastHit2D raycastHit)
    {
        for (int i = 0; i < summoneds.Count; i++)
        {
           summoneds[i].Health.Kill();
        }
        base.OnDeath(ds, raycastHit);
    }
    protected override void OnMorale(DamageStruct ds, RaycastHit2D raycastHit)
    {
        base.OnMorale(ds, raycastHit);
        for (int i = 0; i < summoneds.Count; i++)
        {
            summoneds[i].Health.Kill();
        }
    }
    protected void OnSummoned(ProjectilePawnBase summoned)
    {   if(character.SpriteRendererHolder.GrayShades > 0)
        {
            summoned.SpriteRendererHolder.OnMorale();
        }
        summoneds.Add(summoned);
    }


    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        jsonObject.Add("closeToTarget", new JSONBool(closeToTarget));
        jsonObject.Add("fearToTarget", new JSONBool(fearToTarget));
        JSONArray summonedsJArray = new JSONArray();
        {
            for (int i = 0; i < summoneds.Count; i++)
            {
                if (summoneds[i].Health.IsAlive)
                {
                    JSONObject summonedJObject = new JSONObject();
                    SaveSystem.ComponentReferenceSave(summonedJObject, "summoned", summoneds[i]);
                    summonedsJArray.Add(summonedJObject);
                }
            }           
        }
        jsonObject.Add("summoneds", summonedsJArray);
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        closeToTarget = jsonObject["closeToTarget"].AsBool;
        fearToTarget = jsonObject["fearToTarget"].AsBool;
        JSONArray summonedsJArray = jsonObject["summoneds"].AsArray;
        {
            for (int i = 0; i < summonedsJArray.Count; i++)
            {
                    JSONObject summonedJObject = summonedsJArray[i].AsObject;
                    ProjectilePawnBase ppb = default;
                    SaveSystem.ComponentReferenceLoad(summonedJObject, "summoned", ref ppb);
                    summoneds.Add(ppb);
            }
        }
        return jsonObject;
    }
    protected void OnDrawGizmosSelected()
    {
        if (hasPawn)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(character.transform.position, distanceToClose);
            Gizmos.DrawWireSphere(character.transform.position, distanceToFar);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(character.transform.position, distanceToAttack);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(character.transform.position, distanceToFear);
        }
        else
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, distanceToClose);
            Gizmos.DrawWireSphere(transform.position, distanceToFar);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, distanceToAttack);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, distanceToFear);
        }
    }
}
