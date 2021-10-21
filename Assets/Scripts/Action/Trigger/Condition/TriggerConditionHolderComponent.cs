using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerConditionHolderComponent : MonoBehaviour
{
    protected List<TriggerConditionBaseComponent> conditions = new List<TriggerConditionBaseComponent>();
    public bool Check()
    {
        bool action = true;
        for (int i = 0; i < conditions.Count; i++)
        {
            if(!conditions[i].Check())
            {
                action = false;
                break;
            }    
        }
        return action;
    }
}
