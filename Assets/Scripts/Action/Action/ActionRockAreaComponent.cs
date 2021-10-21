using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionRockAreaComponent : ActionBaseComponent
{
    [SerializeField]
    protected RockAreaMaster rockArea;
    [SerializeField]
    protected bool execute = true;
    public override void OnEnter()
    {
        base.OnEnter();
        if (execute)
            rockArea.Execute();
        else
            rockArea.Stop();
    }
    public override bool OnUpdate()
    {
        return true;
    }
}
