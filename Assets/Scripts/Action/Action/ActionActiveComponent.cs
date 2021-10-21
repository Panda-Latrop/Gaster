using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionActiveComponent : ActionBaseComponent
{
    [SerializeField]
    protected GameObject target;
    [SerializeField]
    protected bool active;
    public override void OnEnter()
    {
        base.OnEnter();
        target.SetActive(active);
    }

    public override bool OnUpdate()
    {
        return target.activeSelf == active; ;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        if (target != null)
        {
            Gizmos.DrawLine(transform.position, target.transform.position);
        }

    }
}
