using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionGlobalComponent : ActionBaseComponent
{
    [SerializeField]
    protected GameInstance.GlobalStruct global;
    [SerializeField]
    protected bool add;
    public override void OnEnter()
    {
        base.OnEnter();
        if (add)
        {
            GameInstance.Instance.GlobalAdd(global);
        }
        else
        {
            GameInstance.Instance.GlobalSet(global);
        }
    }
    public override bool OnUpdate()
    {
        return true;
    }
}
