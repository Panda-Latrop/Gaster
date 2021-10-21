using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemaMusicComponent : CinemaBaseComponent
{
    public enum CinemaMusicType
    {
        change,
        pause,
        resume,     
        clear,
    }
    [SerializeField]
    protected CinemaMusicType type = CinemaMusicType.change;
    [SerializeField]
    protected AudioClip music;

    public override void OnPlay()
    {
        base.OnPlay();
        switch (type)
        {
            case CinemaMusicType.resume:
                GameInstance.Instance.MusicSystem.Resume();
                break;
            case CinemaMusicType.pause:
                GameInstance.Instance.MusicSystem.Pause();
                break;
            case CinemaMusicType.change:
                GameInstance.Instance.MusicSystem.Change(music,false);
                break;
            case CinemaMusicType.clear:
                GameInstance.Instance.MusicSystem.Change();
                break;
            default:
                break;
        }
    }
    public override void OnSkip()
    {
        base.OnSkip();
        GameInstance.Instance.MusicSystem.Stop();
    }
}
