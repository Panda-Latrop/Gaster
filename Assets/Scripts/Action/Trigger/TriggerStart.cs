using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerStart : TriggerExecutor
{
    protected virtual void Start()
    {
        if(!isClosed)
        Execute();       
    }
    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        enabled = true;
        return jsonObject;
    }
}
