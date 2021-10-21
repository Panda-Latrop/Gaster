using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerDominateBehaviourBaseComponent : MonoBehaviour, ISaveableComponent
{

    [SerializeField]
    protected PrefabHolder prefab;
    [SerializeField]
    protected int slot;
    protected bool hasCharacter;
    protected bool hasTarget;
    protected PlayerDominateController controller;

    public int Slot => slot;
    public bool HasCharacter => hasCharacter;
    public bool HasTarget => hasTarget;

    public void SetPlayerController(PlayerDominateController controller)
    {
        this.controller = controller;
    }

    public abstract bool Ready();
    public abstract void Execute(int action);
    public virtual bool CanUpdate() { return hasCharacter && hasTarget; }
    public abstract bool OnUpdate();
    public abstract bool OnLateUpdate();
    public abstract void Stop();

    public virtual JSONObject Save(JSONObject jsonObject)
    {
        jsonObject.Add("name", new JSONString(gameObject.name));
        jsonObject.Add("prefab", new JSONString(prefab.Path));
        jsonObject.Add("enabled", new JSONBool(enabled));
        jsonObject.Add("active", new JSONBool(gameObject.activeSelf));       
        return jsonObject;
    }
    public virtual JSONObject Load(JSONObject jsonObject)
    {
        gameObject.name = jsonObject["name"];
        enabled = jsonObject["enabled"].AsBool;
        gameObject.SetActive(jsonObject["active"].AsBool);
        return jsonObject;
    }
}
