using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionDelayComponent : ActionBaseComponent
{
    [SerializeField]
    protected float time;
    protected float nextTime;
    

    public override void OnEnter()
    {
        base.OnEnter();
        nextTime = time + Time.time;
    }

    public override bool OnUpdate()
    {
        if (Time.time >= nextTime)
            Debug.Log("Call");
        return Time.time >= nextTime;
    }
    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        SaveSystem.TimerSave(jsonObject, "next", nextTime);
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        SaveSystem.TimerLoad(jsonObject, "next",ref nextTime);
        return jsonObject;
    }
}
