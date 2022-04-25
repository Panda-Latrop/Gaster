using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCharacterComponent : AnimationBaseComponent
{
    [SerializeField]
    protected OrientationComponent orientation;
    [SerializeField]
    protected CharacterMovementComponent characterMovement;

    protected int moveHash, moveDirectionXHash, moveDirectionYHash, moveOrientationHash, lookOrientationXHash, lookOrientationYHash, shootHash, attackHash;

    protected void Start()
    {
        moveHash = Animator.StringToHash("Move");
        moveDirectionXHash = Animator.StringToHash("MoveDirectionX");
        moveDirectionYHash = Animator.StringToHash("MoveDirectionY");
        moveOrientationHash = Animator.StringToHash("MoveOrientation");
        lookOrientationXHash = Animator.StringToHash("LookOrientationX");
        lookOrientationYHash = Animator.StringToHash("LookOrientationY");
        shootHash = Animator.StringToHash("Shoot");
        attackHash = Animator.StringToHash("Attack");
    }

    public override void AnimatorMessage(string message, int value = 0)
    {
        //throw new System.NotImplementedException();
    }
    public virtual void PlayShoot()
    {
        animator.Play(shootHash, 0);
    }
    public virtual void PlayAttack()
    {
        animator.Play(attackHash, 0);
    }
    protected virtual void LateUpdate()
    {
        float sqrm = characterMovement.Rigidbody.velocity.sqrMagnitude;
        animator.SetBool(moveHash, sqrm > 0.0f);
        Vector2 direction = characterMovement.Direction;
        animator.SetFloat(moveDirectionXHash, direction.x);
        animator.SetFloat(moveDirectionYHash, direction.y);
        Vector2 orientation = this.orientation.Direction;
        animator.SetFloat(lookOrientationXHash, orientation.x);
        animator.SetFloat(lookOrientationYHash, orientation.y);
        float sign;
        if ((direction.x >= 0.0f && orientation.x < 0.0f) || (direction.x < 0.0f && orientation.x >= 0.0f))
            sign = -1.0f;
        else
            sign = 1.0f;
        if (sqrm < 1.0f)
        {
            animator.SetFloat(moveOrientationHash, sqrm * sign);
        }
        else
        {
            animator.SetFloat(moveOrientationHash, sign);
        }
    }
}
