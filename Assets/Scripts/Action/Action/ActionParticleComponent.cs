using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionParticleComponent : ActionBaseComponent
{
    [SerializeField]
    protected new ParticleSystem particleSystem;
    [SerializeField]
    protected List<ParticleSystem> particleSystemChildren = new List<ParticleSystem>();

    public override void OnEnter()
    {
        base.OnEnter();
        particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particleSystem.time = 0.0f;
        particleSystem.Play(true);
    }

    public override bool OnUpdate()
    {
        return !particleSystem.isPlaying;
    }

    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        SaveSystem.ParticleSystemSave(jsonObject, "main", particleSystem, particleSystemChildren);
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        SaveSystem.ParticleSystemLoad(jsonObject, "main", particleSystem, particleSystemChildren);
        return jsonObject;
    }
}