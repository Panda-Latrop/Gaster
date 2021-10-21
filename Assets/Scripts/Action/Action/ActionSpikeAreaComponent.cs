using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSpikeAreaComponent : ActionBaseComponent
{
    [SerializeField]
    protected SpikeAreaMaster spikeArea;
    [SerializeField]
    protected bool execute = true;
    public override void OnEnter()
    {
        base.OnEnter();
        if (execute)
            spikeArea.Execute();
        else
            spikeArea.Stop();
    }
    public override bool OnUpdate()
    {
        return true ;
    }
}
