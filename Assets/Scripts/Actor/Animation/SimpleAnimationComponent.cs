using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAnimationComponent : MonoBehaviour, ISaveableComponent
{
    [SerializeField]
    protected SpriteRenderer[] renderers;
    [SerializeField]
    protected Sprite[] sprites;
    [SerializeField]
    protected float time;
    protected float nextTime;
    protected int current;

    public void OnLateUpdate()
    {
        if (Time.time >= nextTime)
        {
            nextTime = Time.time + time;
            if (++current >= sprites.Length)
                current = 0;
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].sprite = sprites[current];
            }
        }
    }
    public virtual JSONObject Save( JSONObject jsonObject)
    {
        SaveSystem.TimerSave(jsonObject, "frame", nextTime);
        jsonObject.Add("frame", new JSONNumber(current));
        return jsonObject;
    }
    public virtual JSONObject Load( JSONObject jsonObject)
    {
        SaveSystem.TimerLoad(jsonObject, "frame", ref nextTime);
        current = jsonObject["frame"].AsInt;
        return jsonObject;
    }
}