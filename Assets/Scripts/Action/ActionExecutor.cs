using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ActionExecutor : Actor
{
    [SerializeField]
    protected List<ActionHolderComponent> holders = new List<ActionHolderComponent>();
    [SerializeField]
    protected int currentExecutor;
    protected bool isFinished;
    protected ActionHolderComponent holder => holders[currentExecutor];
    public bool IsFinished => isFinished;
    public int CurrentExecutor { get => currentExecutor; set => currentExecutor = value; }

#if UNITY_EDITOR
    [ContextMenu("Auto Set")]
    public void UNITY_EDITOR_AutoSet()
    {
        var raw = gameObject.GetComponentsInChildren<ActionHolderComponent>();
        if (raw == null || (raw != null && raw.Length <= 0))
        {
            var rawExecutor = gameObject.AddComponent<ActionHolderComponent>();
            rawExecutor.UNITY_EDITOR_AutoSet();
            holders = new List<ActionHolderComponent>();
            holders.Add(rawExecutor);
        }
        else
        {
            holders = new List<ActionHolderComponent>(raw);
        }
        EditorUtility.SetDirty(this);

    }
#endif
    public virtual void Execute()
    {
        enabled = true;
        isFinished = true;
        if (!holder.OnStart())
            Stop();
    }
    public virtual void Stop()
    {
        enabled = false;
        isFinished = false;
        holder.CurrentAction = 0;
    }
    protected virtual void Update()
    {
        if (isFinished)
            if (!holder.OnUpdate())
                Stop();
    }
    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        JSONArray executorsJArray = new JSONArray();
        for (int i = 0; i < holders.Count; i++)
            executorsJArray.Add(holders[i].Save(new JSONObject()));
        jsonObject.Add("holders", executorsJArray);
        jsonObject.Add("isFinished", new JSONBool(isFinished));      
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        JSONArray executorsJArray = jsonObject["holders"].AsArray;
        for (int i = 0; i < executorsJArray.Count; i++)
            holders[i].Load(executorsJArray[i].AsObject);
        isFinished = jsonObject["isFinished"].AsBool;
        return jsonObject;
    }
}
