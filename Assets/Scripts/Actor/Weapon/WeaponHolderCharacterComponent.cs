using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolderCharacterComponent : WeaponHolderBaseComponent
{
    [SerializeField]
    protected bool useAnimation = true;
    [SerializeField]
    protected AnimationCharacterComponent animationCharacter;
    [SerializeField]
    protected AudioSource audioSource;
    protected bool hasAudio;
    [SerializeField]
    protected AudioClip changeSound;

    protected override void Start()
    {
        base.Start();
        hasAudio = audioSource != null;
    }

    public override ShootState Shoot(Vector3 position, Vector3 direction)
    {
        ShootState state = base.Shoot(position, direction);
        switch (state)
        {
            case ShootState.initiated:
                if(useAnimation)
                animationCharacter.PlayShoot();
                break;
            default:
                break;
        }
        return state;
    }
    public override int ChangeSlot(int toSlot)
    {
        switch (base.ChangeSlot(toSlot))
        {
            case 0:               
                return 0;
            case 1:
                if (hasAudio)
                {
                    audioSource.clip = changeSound;
                    audioSource.time = 0.0f;
                    audioSource.Play();
                }
                return 1;
            case 2:
                return 2;
            default:
                return 0;
        }
    }
}