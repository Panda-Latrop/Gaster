using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterDialogComponent : MonoBehaviour,ISaveableComponent, IInteractiveExecutor
{
    [SerializeField]
    protected string file;
    [SerializeField]
    protected bool closeOnExecute = true;
    [SerializeField]
    protected int currentExecutor;
    [SerializeField]
    protected List<ActionHolderComponent> holders = new List<ActionHolderComponent>();
    protected bool isFinished;
    protected bool isClosed;
    protected int local = 0;
    public bool IsClosed => isClosed;
    protected ActionHolderComponent holder => holders[currentExecutor];
    public bool IsFinished => isFinished;
    public int CurrentExecutor { get => currentExecutor; set => currentExecutor = value; }

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
    public virtual void Close()
    {
        isClosed = true;
    }
    public virtual void Open()
    {
        isClosed = false;
    }
    public virtual void Execute()
    {
        if (!isClosed)
        {
            var dialog = GameInstance.Instance.Dialog;
            dialog.BindOnMessage(OnMessage);
            dialog.BindOnOnEnd(OnEnd);
            dialog.Prepare(file).Next(true,local);
            
            if (closeOnExecute)
                Close();
        }
    }
    protected void OnMessage(int message)
    {
        local = message;
    }
    protected void OnEnd()
    {
        if (!isClosed)
        {
            if (local >= 0 && local < holders.Count)
                currentExecutor = local;

            {
                if (!isClosed)
                {
                    enabled = true;
                    isFinished = true;
                    if (!holder.OnStart())
                        Stop();
                    if (closeOnExecute)
                        Close();
                }
            }
        }
    }
    public virtual JSONObject Save(JSONObject jsonObject)
    {
        JSONArray executorsJArray = new JSONArray();
        for (int i = 0; i < holders.Count; i++)
            executorsJArray.Add(holders[i].Save(new JSONObject()));
        jsonObject.Add("holders", executorsJArray);
        jsonObject.Add("isFinished", new JSONBool(isFinished));
        jsonObject.Add("isClosed", new JSONBool(isClosed));
        jsonObject.Add("local", new JSONNumber(local));
        return jsonObject;
    }
    public virtual JSONObject Load(JSONObject jsonObject)
    {
        JSONArray executorsJArray = jsonObject["holders"].AsArray;
        for (int i = 0; i < executorsJArray.Count; i++)
            holders[i].Load(executorsJArray[i].AsObject);
        isFinished = jsonObject["isFinished"].AsBool;
        isClosed = jsonObject["isClosed"].AsBool;
        local = jsonObject["local"].AsInt;
        return jsonObject;
    }
}
