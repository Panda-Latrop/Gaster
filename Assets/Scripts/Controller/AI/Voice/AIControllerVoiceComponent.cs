using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControllerVoiceComponent : MonoBehaviour
{
    protected AudioSource source;
    [SerializeField]
    protected AudioClip[] battleCries, hurtCries;

    public void SetAudioSource(AudioSource source)
    {
        this.source = source;
    }
    public void Play(AudioClip clip)
    {
        source.Stop();
        source.time = 0;
        source.clip = clip;
        source.Play();
    }
    public void PlayBattleCries()
    {
        Play(battleCries[Random.Range(0, battleCries.Length)]);
    }
    public void PlayHurtCries()
    {
        Play(hurtCries[Random.Range(0, hurtCries.Length)]);
    }
}
