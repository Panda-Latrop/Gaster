using System.Collections;
using UnityEngine;

public class CinemaAnimationComponent : CinemaBaseComponent
{
    [SerializeField]
    protected Animator animator;
    [SerializeField]
    protected new string animation;
    [SerializeField]
    protected bool crossFade;
    [SerializeField]
    protected float transitionDuration;

    protected int animationHash;

    public void Start()
    {
        animationHash = Animator.StringToHash(animation);
    }
    public override void OnPlay()
    {
        base.OnPlay();
        if (animator.GetCurrentAnimatorStateInfo(0).shortNameHash != animationHash)
        {
            if (crossFade)
            {
                animator.CrossFade(animationHash, transitionDuration, 0, 0.0f, 0.0f);
            }
            else
            {
                animator.Play(animationHash, 0);
            }
        }
    }
    public override bool OnUpdate()
    {
        if (waitForEnd)
        {
            var stete = animator.GetCurrentAnimatorStateInfo(0);
            return stete.shortNameHash != animationHash && (stete.normalizedTime >= 1.0f || animator.IsInTransition(0));
        }
        else
            return true;
    }
    public override void OnSkip()
    {
        animator.Play(animationHash, 0,1.0f);
        base.OnSkip();
    }
}