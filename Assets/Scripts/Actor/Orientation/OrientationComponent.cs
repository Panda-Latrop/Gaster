using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class OrientationComponent : MonoBehaviour, ISaveableComponent
{
    [SerializeField]
    protected Vector2 direction = Vector2.right;
    public Vector2 Direction { get => direction; set => direction = value; }

    public virtual JSONObject Save( JSONObject jsonObject)
    {
        jsonObject.Add("enabled", new JSONBool(enabled));
        JSONArray orientationJArray = new JSONArray();
        {
            orientationJArray.Add(new JSONNumber(direction.x));
            orientationJArray.Add(new JSONNumber(direction.y));
        }
        jsonObject.Add("orientation", orientationJArray);
        return jsonObject;
    }
    public virtual JSONObject Load( JSONObject jsonObject)
    {
        enabled = jsonObject["enabled"].AsBool;
        JSONArray orientationJArray = jsonObject["orientation"].AsArray;
        {
            direction.Set(orientationJArray[0].AsFloat, orientationJArray.AsFloat);
        }
        return jsonObject;
    }
}
