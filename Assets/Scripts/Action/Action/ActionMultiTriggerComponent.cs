using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMultiTriggerComponent : ActionBaseComponent
{
    [SerializeField]
    protected bool close;
    [SerializeField]
    protected TriggerExecutor[] triggers;
    public override void OnEnter()
    {
        base.OnEnter();
        if (close)
        {
            for (int i = 0; i < triggers.Length; i++)
            {
                triggers[i].Close();
            }
        }
        else
        {
            for (int i = 0; i < triggers.Length; i++)
            {
                triggers[i].Open();
            }
        }
    }
    public override bool OnUpdate()
    {
        return true;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < triggers.Length; i++)
        {
            if(triggers != null)
            {
                Gizmos.DrawLine(transform.position,triggers[i].transform.position);
            }
        }
    }
}