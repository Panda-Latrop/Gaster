using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CharacterOrientationBaseComponent : MonoBehaviour, ISaveableComponent
{
    [SerializeField]
    protected Vector2 orientation = Vector2.right;
    public Vector2 Orientation { get => orientation; set => orientation = value; }

    public virtual JSONObject Save( JSONObject jsonObject)
    {
        jsonObject.Add("enabled", new JSONBool(enabled));
        JSONArray orientationJArray = new JSONArray();
        {
            orientationJArray.Add(new JSONNumber(orientation.x));
            orientationJArray.Add(new JSONNumber(orientation.y));
        }
        jsonObject.Add("orientation", orientationJArray);
        return jsonObject;
    }
    public virtual JSONObject Load( JSONObject jsonObject)
    {
        enabled = jsonObject["enabled"].AsBool;
        JSONArray orientationJArray = jsonObject["orientation"].AsArray;
        {
            orientation.Set(orientationJArray[0].AsFloat, orientationJArray.AsFloat);
        }
        return jsonObject;
    }
}
