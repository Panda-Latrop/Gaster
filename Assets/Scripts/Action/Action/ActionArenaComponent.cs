using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionArenaComponent : ActionBaseComponent
{
    [SerializeField]
    protected ArenaMaster arena;
    public override void OnEnter()
    {
        base.OnEnter();
        arena.Execute();
    }
    public override bool OnUpdate()
    {
        return true;
    }
}
