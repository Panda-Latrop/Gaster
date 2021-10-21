using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockAreaObjectComponent : MonoBehaviour, ISaveableComponent
{
    [SerializeField]
    protected new Transform transform;
    [SerializeField]
    protected new SpriteRenderer renderer;
    [SerializeField]
    protected AudioSource source;
    [SerializeField]
    protected AudioClip rubbing, punch;
    protected int currentSound = 0;
    [SerializeField]
    protected bool destroyOnStop;
    [SerializeField]
    protected ParticleSystem destroyParticle, movementParticle;

    public Vector3 Position { get => transform.position; set => transform.position = value; }

    public void OnStart()
    {
        currentSound = 0;
        source.Stop();
        source.clip = rubbing;
        source.time = 0;
        source.loop = true;
        source.Play();
        movementParticle.Play(true);
        renderer.enabled = true;
    }
    public void OnStop()
    {
        movementParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        currentSound = 1;
        source.Stop();
        source.time = 0;
        source.clip = punch;
        source.loop = false;
        source.Play();
        if (destroyOnStop)
        {
            renderer.enabled = false;
            destroyParticle.Play(true);
        }

    }
    public JSONObject Save(JSONObject jsonObject)
    {
        SaveSystem.GameObjectSave(jsonObject, gameObject);
        jsonObject.Add("spriteEnabled", new JSONBool(renderer.enabled));
        jsonObject.Add("currentSound", new JSONNumber(currentSound));
        SaveSystem.AudioSourceSave(jsonObject, "main", source);
        SaveSystem.ParticleSystemSave(jsonObject, "movement", movementParticle);
        if (destroyOnStop)
            SaveSystem.ParticleSystemSave(jsonObject, "destroy", destroyParticle);
        return jsonObject;
    }
    public JSONObject Load(JSONObject jsonObject)
    {
        SaveSystem.GameObjectLoad(jsonObject, gameObject);
        renderer.enabled = jsonObject["spriteEnabled"].AsBool;
        currentSound = jsonObject["currentSound"].AsInt;
        if (currentSound == 0)
            source.clip = rubbing;
        else
            source.clip = punch;
        SaveSystem.AudioSourceLoad(jsonObject, "main", source);
        SaveSystem.ParticleSystemLoad(jsonObject, "movement", movementParticle);
        if (destroyOnStop)
            SaveSystem.ParticleSystemLoad(jsonObject, "destroy", destroyParticle);
        return jsonObject;
    }
}
