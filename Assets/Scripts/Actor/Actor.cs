using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour, ISaveable
{
    [SerializeField]
    protected PrefabHolder prefab;
    [SerializeField]
    protected string saveTag;

    public PrefabHolder Prefab => prefab;
    public GameObject GameObject => gameObject;

    public string SaveTag { get => saveTag; set => saveTag = value; }
    public virtual JSONObject Save( JSONObject jsonObject)
    {
        jsonObject.Add("name", new JSONString(gameObject.name));
        jsonObject.Add("prefab", new JSONString(prefab.Path));
        jsonObject.Add("enabled", new JSONBool(enabled));
        jsonObject.Add("saveTag", new JSONString(saveTag));
        SaveSystem.GameObjectSave(jsonObject, gameObject);
        return jsonObject;
    }
    public virtual JSONObject Load( JSONObject jsonObject)
    {
        gameObject.name = jsonObject["name"];
        enabled = jsonObject["enabled"].AsBool;
        saveTag = jsonObject["saveTag"];
        SaveSystem.GameObjectLoad(jsonObject, gameObject);
        return jsonObject;
    }
}
