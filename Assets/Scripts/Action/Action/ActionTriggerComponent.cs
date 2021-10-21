using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTriggerComponent : ActionBaseComponent
{
    [SerializeField]
    protected bool close;
    [SerializeField]
    protected TriggerExecutor trigger;
    public override void OnEnter()
    {
        base.OnEnter();
        if (close)
            trigger.Close();
        else
            trigger.Open();
    }
    public override bool OnUpdate()
    {
        return true;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
            if (trigger != null)
            {
                Gizmos.DrawLine(transform.position, trigger.transform.position);
            }
        
    }
}