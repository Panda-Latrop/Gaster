using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicExecutor : ActionExecutor, IPoolObject
{
    [SerializeField]
    protected string specifier = "base";
    [SerializeField]
    protected float lifeTime = 1.0f;
    protected float nextTime;
    public virtual void OnPop()
    {
        gameObject.SetActive(true);
        nextTime = Time.time + lifeTime;
        Execute();
    }
    public virtual void OnPush()
    {
        Stop();
        gameObject.SetActive(false);
    }
    public override void Stop()
    {
        base.Stop();
        enabled = !enabled;
    }
    protected virtual void LateUpdate()
    {
        if(Time.time >= nextTime)
        {
            Push();
        }
    }

    #region Pool
    protected virtual void Push()
    {
        GameInstance.Instance.PoolManager.Push(this);
    }
    public string PoolTag => this.GetType().Name + specifier;
    public Transform Transform => transform;
    public void SetPosition(Vector3 position) => transform.position = position;
    public void SetRotation(Quaternion rotation) => transform.rotation = rotation;
    public void SetScale(Vector3 scale) => transform.localScale = scale;
    public void SetParent(Transform parent) => transform.SetParent(parent,false);
    #endregion
    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        SaveSystem.TimerSave(jsonObject, "life", nextTime);
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        SaveSystem.TimerLoad(jsonObject, "life",ref nextTime);
        return jsonObject;
    }
}
