using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAnimationActor : Actor
{
    [SerializeField]
    protected SimpleAnimationComponent[] animations;

    protected void LateUpdate()
    {
        for (int i = 0; i < animations.Length; i++)
        {
            animations[i].OnLateUpdate();
        }
    }
    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        JSONArray animationsJArray = new JSONArray();
        for (int i = 0; i < animations.Length; i++)
            animationsJArray.Add(animations[i].Save(new JSONObject()));         
        jsonObject.Add("animations", animationsJArray);
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        JSONArray animationsJArray = jsonObject["animations"].AsArray;
        for (int i = 0; i < animationsJArray.Count; i++)
            animations[i].Load(animationsJArray[i].AsObject);
        return jsonObject;
    }
}
