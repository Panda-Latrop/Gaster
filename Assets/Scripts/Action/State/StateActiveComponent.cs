using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateActiveComponent : StateBaseComponent
{
    [SerializeField]
    protected GameObject[] gameObjects;
    public override JSONObject Save( JSONObject jsonObject)
    {
        JSONArray objectsJArray = new JSONArray();
        for (int i = 0; i < gameObjects.Length; i++)
            objectsJArray.Add(SaveSystem.GameObjectSave(new JSONObject(), gameObjects[i]));
        jsonObject.Add("objects", objectsJArray);
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        JSONArray objectsJArray = jsonObject["objects"].AsArray;
        for (int i = 0; i < objectsJArray.Count; i++)
            SaveSystem.GameObjectLoad(objectsJArray[i].AsObject, gameObjects[i]);
        return jsonObject;
    }
}
