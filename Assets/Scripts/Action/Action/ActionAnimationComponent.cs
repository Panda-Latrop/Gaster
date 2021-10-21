using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionAnimationComponent : ActionBaseComponent
{
    [SerializeField]
    protected Animator animator;
    [SerializeField]
    private new string animation;

    protected int animationHash;

    public override void OnEnter()
    {
        base.OnEnter();
        animationHash = Animator.StringToHash(animation);
        if (animator.GetCurrentAnimatorStateInfo(0).shortNameHash != animationHash)
        {
            animator.Play(animationHash, 0);
        }
    }
    public override bool OnUpdate()
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f || animator.IsInTransition(0);
    }
    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        SaveSystem.AnimatorSave(jsonObject, "animator", animator);
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        SaveSystem.AnimatorLoad(jsonObject, "animator", animator);
        return jsonObject;
    }
}
