using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControllerShooter : AIControllerAttacker
{
    [SerializeField]
    protected float distanceToClose = 2.0f;
    [SerializeField]
    protected float distanceToFar = 2.0f;
    protected bool closeToTarget;
    protected ShootState shootState;

    protected override void Update()
    {
        base.Update();
        if (hasTarget)
        {
            shootState = ShootState.none;

            Vector3 target = this.target.transform.position;
            float distance = (target - character.transform.position).sqrMagnitude;
            Vector3 direction = (target - character.transform.position).normalized;
            if (distance <= distanceToClose * distanceToClose && !closeToTarget)
                closeToTarget = true;
            else
            {
                if (distance > distanceToFar * distanceToFar && closeToTarget)
                    closeToTarget = false;
            }
            if (closeToTarget)
            {

                character.WeaponHolderComponent.SetFire(true);
                character.WeaponHolderComponent.SetTarget(this.target.transform);
                shootState = character.WeaponHolderComponent.Shoot(character.WeaponHolderComponent.GetShootPoint(), direction);
            }
            if (!shootState.Equals(ShootState.initiated) && !shootState.Equals(ShootState.process) && !closeToTarget)
            {
                character.WeaponHolderComponent.SetFire(false);
                character.CharacterMovement.FollowTarget(this.target, distance, direction);
            }
            else
            {
                character.CharacterMovement.Stop();
            }
        }
    }
    protected void OnDrawGizmosSelected()
    {
        if (hasPawn)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(character.transform.position, distanceToClose);
            Gizmos.DrawWireSphere(character.transform.position, distanceToFar);
        }
        else
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, distanceToClose);
            Gizmos.DrawWireSphere(transform.position, distanceToFar);
        }
    }
    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        jsonObject.Add("closeToTarget", new JSONBool(closeToTarget));     
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        closeToTarget = jsonObject["closeToTarget"].AsBool;
        return jsonObject;
    }

}
