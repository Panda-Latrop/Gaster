using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaAnimatedObstacle : ArenaObstacle
{
    [SerializeField]
    protected Animator animator;
    protected int showHash, hideHash;
    public void Start()
    {
        showHash = Animator.StringToHash("Show");
        hideHash = Animator.StringToHash("Hide");
    }
    public override void Show()
    {
        base.Show();
        animator.Play(showHash);
    }
    public override void Hide()
    {
        base.Hide();
        animator.Play(hideHash);
    }
    public override JSONObject Save(JSONObject jsonObject)
    {
        base.Save(jsonObject);
        SaveSystem.ColliderSave(jsonObject, string.Empty, collider);
        SaveSystem.AnimatorSave(jsonObject, string.Empty, animator);
        return jsonObject;
    }
    public override JSONObject Load(JSONObject jsonObject)
    {
        base.Load(jsonObject);
        SaveSystem.ColliderLoad(jsonObject, string.Empty, collider);
        SaveSystem.AnimatorLoad(jsonObject, string.Empty, animator);
        return jsonObject;
    }
}
