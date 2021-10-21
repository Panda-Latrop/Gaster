using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponTrajectoryBaseComponent : MonoBehaviour, ISaveableComponent
{
    protected int count = 0;
    [SerializeField]
    protected Vector2 size = Vector2.one;
    public float scale = 1;
    protected float shift;
    [SerializeField]
    protected float shiftSpeed;
    [SerializeField]
    [Range(0, 2)]
    protected float fill = 1.0f;
    protected float step;
    public int GetCount() => count;
    public abstract void SetCount(int count);
    public abstract Vector3 Evaluate(int position);
    public abstract void OnUpdate();
    public virtual void RandomShift()
    {
        shift = Random.Range(0.0f,1.0f);
    }
    public void ResetShift()
    {
        shift = 0.0f;
    }

    public virtual JSONObject Save( JSONObject jsonObject)
    {
        jsonObject.Add("count", new JSONNumber(count));
        jsonObject.Add("scale", new JSONNumber(scale));
        jsonObject.Add("shift", new JSONNumber(shift));
        jsonObject.Add("step", new JSONNumber(step));
        return jsonObject;
    }
    public virtual JSONObject Load( JSONObject jsonObject)
    {
        count = jsonObject["count"].AsInt;
        scale = jsonObject["scale"].AsFloat;
        shift = jsonObject["shift"].AsFloat;
        step = jsonObject["step"].AsFloat;
        return jsonObject;
    }
}