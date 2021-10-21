using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ActionHolderComponent : MonoBehaviour, ISaveableComponent
{
    [SerializeField]
    protected List<ActionBaseComponent> actions = new List<ActionBaseComponent>();
    protected int currentAction = 0;

    public int CurrentAction { get => currentAction; set => currentAction = value; }
    protected ActionBaseComponent action => actions[currentAction];

    
#if UNITY_EDITOR
    [ContextMenu("Auto Set")]
    public void UNITY_EDITOR_AutoSet()
    {
        actions = new List<ActionBaseComponent>(gameObject.GetComponentsInChildren<ActionBaseComponent>());
        EditorUtility.SetDirty(this);
    }
#endif
    
    protected bool Check()
    {
        if (currentAction < actions.Count)
        {

            action.OnEnter();
            if (action.waitForEnd)
            {
                return true;
            }
                
            if (Change())
            {
                return Check();
            }
                
        }
        return false;
    }

    protected bool Change()
    {
        return (++currentAction < actions.Count);
    }
    public bool OnStart()
    {
        currentAction = 0;
        if (actions.Count <= 0)
            return false;
        if (!Check())
        {
            return false;
        }
        return true;
    }
    public bool OnUpdate()
    {
        
        for (int i = 0; i < currentAction; i++)
        {
            if (!actions[i].Finished)
            {
                if (actions[i].OnUpdate())
                {
                    actions[i].OnExit();
                }
            }
        }
        if (!action.waitForEnd)
        {
            if (!Check())
            {
                return false;
            }
                
        }
        if ((action.waitForEnd && action.OnUpdate()))
        {
            action.OnExit();
            Change();
            if (!Check())
            {
                return false;
            }
        }
        return true;
    }

    public virtual JSONObject Save( JSONObject jsonObject)
    {
        jsonObject.Add("currentAction", new JSONNumber(currentAction));   
        JSONArray actionsJArray = new JSONArray();
        for (int i = 0; i < actions.Count; i++)
            actionsJArray.Add(actions[i].Save(new JSONObject()));
        jsonObject.Add("actions", actionsJArray);        
        return jsonObject;
    }
    public virtual JSONObject Load( JSONObject jsonObject)
    {
        currentAction = jsonObject["currentAction"].AsInt;
        JSONArray actionsJArray = jsonObject["actions"].AsArray;
        for (int i = 0; i < actionsJArray.Count; i++)
            actions[i].Load(actionsJArray[i].AsObject);
        return jsonObject;
    }
}
