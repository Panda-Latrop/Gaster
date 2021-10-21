using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateHolderComponent : MonoBehaviour, ISaveableComponent
{
    [SerializeField]
    protected StateBaseComponent[] states;
    public virtual JSONObject Save(JSONObject jsonObject)
    {
        JSONArray statesJArray = new JSONArray();
        for (int i = 0; i < states.Length; i++)
            statesJArray.Add(states[i].Save(new JSONObject()));
        jsonObject.Add("states", statesJArray);
        return jsonObject;
    }
    public virtual JSONObject Load(JSONObject jsonObject)
    {
        JSONArray statesJArray = jsonObject["states"].AsArray;
        for (int i = 0; i < statesJArray.Count; i++)
            states[i].Load(statesJArray[i].AsObject);
        return jsonObject;
    }
}
