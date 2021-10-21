using SimpleJSON;
using System.Collections;
using UnityEngine;

public class ActionSoundComponent : ActionBaseComponent
{
    [SerializeField]
    protected AudioSource audioSource;
    [SerializeField]
    protected bool play = true;
    public override void OnEnter()
    {
        base.OnEnter();
        audioSource.time = 0.0f;
        if(play)
            audioSource.Play();
        else
            audioSource.Stop();
    }
    public override bool OnUpdate()
    {
        return !audioSource.isPlaying;
    }

    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        SaveSystem.AudioSourceSave(jsonObject, "main", audioSource);
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        SaveSystem.AudioSourceLoad(jsonObject, "main", audioSource);      
        return jsonObject;
    }
}