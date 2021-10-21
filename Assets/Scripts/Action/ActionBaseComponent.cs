using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionBaseComponent : MonoBehaviour, ISaveableComponent
{
    public bool waitForEnd;
    protected bool finished;

    public bool Finished => finished;
    public virtual void OnEnter() { finished = false; return; }
    public virtual bool OnUpdate() { return true; }
    public virtual void OnExit() { finished = true; return; }

    public virtual JSONObject Save( JSONObject jsonObject)
    {
        jsonObject.Add("finished", new JSONBool(finished));
        return jsonObject;
    }

    public virtual JSONObject Load( JSONObject jsonObject)
    {
        finished = jsonObject["finished"].AsBool;
        return jsonObject;
    }
}
