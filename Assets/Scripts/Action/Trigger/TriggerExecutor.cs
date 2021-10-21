using SimpleJSON;
using System.Collections;
using UnityEngine;

public class TriggerExecutor : GateExecutor
{
    [SerializeField]
    protected bool useCondition = false;
    [SerializeField]
    protected TriggerConditionHolderComponent condition;

    public override void Execute()
    {
        if (useCondition)
        {
            if (condition.Check())
                base.Execute();
        }
        else
        {
            base.Execute();
        }        
    }
}