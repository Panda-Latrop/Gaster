using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemaAudioComponent : CinemaBaseComponent
{
    public enum CinemaAudioType
    {
        play,
        pause,
        resume,
        stop,
    }
    [SerializeField]
    protected CinemaAudioType type = CinemaAudioType.play;
    [SerializeField]
    protected AudioSource source;
    [SerializeField]
    protected AudioClip sound;

    public override void OnPlay()
    {
        base.OnPlay();
        switch (type)
        {
            case CinemaAudioType.play:
                source.Stop();
                source.clip = sound;
                source.time = 0;
                source.Play();
                break;
            case CinemaAudioType.pause:
                source.Pause();
                break;
            case CinemaAudioType.resume:
                source.UnPause();
                break;
            case CinemaAudioType.stop:
                source.Stop();
                break;
            default:
                break;
        }
    }
    public override bool OnUpdate()
    {
        return !source.isPlaying;
    }
    public override void OnSkip()
    {
        base.OnSkip();
        source.Stop();
    }
}
