using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "Music Holder", menuName = "System/Music/Holder", order = 2)]
public class MusicHolder : ScriptableObject
{
    [SerializeField]
    protected AudioMixerGroup mixerGroup;
    [SerializeField]
    protected AudioClip[] peace, combat;

    public AudioMixerGroup GetMixerGroup()
    {
        return mixerGroup;
    }

    public AudioClip GetPeace(int music)
    {
        return peace[music];
    }
    public AudioClip GetCombat(int music)
    {
        return combat[music];
    }
    public int GetPeaceCount()
    {
        return peace.Length;
    }
    public int GetCombatCount()
    {
        return combat.Length;
    }

}