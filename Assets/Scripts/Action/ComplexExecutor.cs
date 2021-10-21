using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexExecutor : GateExecutor
{
    [SerializeField]
    protected StateHolderComponent stateHolder;
    public override JSONObject Save(JSONObject jsonObject)
    {
        jsonObject.Add("states", stateHolder.Save(new JSONObject()));
        base.Save(jsonObject);
        return jsonObject;
    }
    public override JSONObject Load(JSONObject jsonObject)
    {
        stateHolder.Load(jsonObject["states"].AsObject);
        base.Load(jsonObject);
        return jsonObject;
    }
}
