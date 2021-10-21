using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WeaponHolderPlayerComponent : WeaponHolderBaseComponent
{
    [SerializeField]
    protected WeaponDominator dominator;
    [SerializeField]
    protected AnimationPlayerComponent animationPlayer;
    [SerializeField]
    protected AudioSource audioSource;
    [SerializeField]
    protected AudioClip changeSound;


    public WeaponDominator Dominator => dominator;

    protected override void Start()
    {
        base.Start();
        if (currentSlot >= 0 && slotEquips[currentSlot])
        {
            animationPlayer.SetHasWeapon(true, slots[currentSlot].TwoHanded);
        }
        dominator.SetOwner(owner);
        
    }
    public void SetFireDominator(bool fire)
    {
        dominator.SetFire(fire);
    }
    public ShootState ShootDominator(Vector3 position, Vector3 direction)
    {
      return dominator.Shoot(position, direction);
    }
    public override ShootState Shoot(Vector3 position, Vector3 direction)
    {
        ShootState state = base.Shoot(position, direction);
        switch (state)
        {
            case ShootState.initiated:
                animationPlayer.PlayShoot();
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
                animationPlayer.SetHasWeapon(false, true);
                return 0;
            case 1:
                audioSource.clip = changeSound;
                audioSource.Play();
                animationPlayer.SetHasWeapon(true, slots[currentSlot].TwoHanded);
                return 1;
            case 2:               
                return 2;
            default:
                return 0;
        }      
    }

    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        jsonObject.Add("dominator", dominator.Save(new JSONObject()));
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        dominator.Load(jsonObject["dominator"].AsObject);
        return jsonObject;
    }


}