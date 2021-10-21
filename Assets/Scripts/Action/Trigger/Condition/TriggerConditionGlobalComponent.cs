using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerConditionGlobalComponent : TriggerConditionBaseComponent
{
    [SerializeField]
    protected GameInstance.GlobalStruct global;
    public override bool Check()
    {
        return GameInstance.Instance.GlobalCheck(global);
    }
}
