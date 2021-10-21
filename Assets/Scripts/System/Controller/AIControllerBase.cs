using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControllerBase : ControllerBase, IPoolObject
{
    public enum State
    {
        enter,
        process,
        exit,
    }

    [SerializeField]
    protected string specifier = "base";
    public string PoolTag => this.GetType().Name+ specifier;
    public void OnPop()
    {        
        enabled = true;
    }
    public void OnPush()
    {
        enabled = false;
    }
    public override void Unpossess()
    {
        base.Unpossess();
        GameInstance.Instance.PoolManager.Push(this);
    }

    public Transform Transform => transform;
    public void SetPosition(Vector3 position) => transform.position = position;
    public void SetRotation(Quaternion rotation) => transform.rotation = rotation;
    public void SetScale(Vector3 scale) => transform.localScale = scale;
    public void SetParent(Transform parent) => transform.parent = parent;

}
