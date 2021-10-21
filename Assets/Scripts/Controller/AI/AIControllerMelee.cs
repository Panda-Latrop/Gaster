using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControllerMelee : AIControllerAttacker
{
    [SerializeField]
    protected float distanceToAttack = 2.0f;

    protected override void Update()
    {
        base.Update();
        if (hasTarget)
        {
            Vector3 target = this.target.transform.position;
            float distance = (target - character.transform.position).sqrMagnitude;
            Vector3 direction = (target - character.transform.position).normalized;
            if (distance <= distanceToAttack * distanceToAttack)
            {
                character.WeaponHolderComponent.SetFire(true);
                character.WeaponHolderComponent.Shoot(character.WeaponHolderComponent.GetShootPoint(), direction);
            }
            else
            {
                character.WeaponHolderComponent.SetFire(false);
            }
            character.CharacterMovement.FollowTarget(this.target,distance, direction);
        }
    }
    protected void OnDrawGizmosSelected()
    {
        if (hasPawn)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(character.transform.position, distanceToAttack);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, distanceToAttack);
        }
    }
}
