using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemaParticleComponent : CinemaBaseComponent
{
    public enum CinemaParticleType
    {
        play,
        stop,
    }
    [SerializeField]
    protected ParticleSystem particle;
    [SerializeField]
    protected CinemaParticleType type;
    public override void OnPlay()
    {
        base.OnPlay();
        switch (type)
        {
            case CinemaParticleType.play:
                particle.Play(true);
                break;
            case CinemaParticleType.stop:
                particle.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
                break;
            default:
                break;
        }
    }
    public override bool OnUpdate()
    {
        return particle.isPlaying;
    }
    public override void OnSkip()
    {
        base.OnSkip();
        particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
