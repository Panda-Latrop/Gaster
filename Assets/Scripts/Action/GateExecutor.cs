using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateExecutor : ActionExecutor
{
    [SerializeField]
    protected bool closeOnExecute = true;
    protected bool isClosed;
    public bool IsClosed => isClosed;

    public virtual void Close()
    {
        isClosed = true;
    }
    public virtual void Open()
    {
        isClosed = false;
    }
    public override void Execute()
    {
        if (!isClosed)
        {
            base.Execute();
            if (closeOnExecute)
                Close();
        }
    }
    public override JSONObject Save(JSONObject jsonObject)
    {
        base.Save(jsonObject);
        jsonObject.Add("isClosed", new JSONBool(isClosed));
        return jsonObject;
    }
    public override JSONObject Load(JSONObject jsonObject)
    {
        base.Load(jsonObject);
        isClosed = jsonObject["isClosed"].AsBool;
        return jsonObject;
    }
}
