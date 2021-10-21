using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionAddToPlayerWeaponComponent : ActionBaseComponent
{
    [SerializeField]
    protected WeaponBase weapon;
    public override void OnEnter()
    {
        base.OnEnter();
        (GameInstance.Instance.PlayerController.ControlledPawn as CharacterPlayer).WeaponHolderComponent.AddWeapon(weapon,true);
    }
    public override bool OnUpdate()
    {
        return true;
    }
}
