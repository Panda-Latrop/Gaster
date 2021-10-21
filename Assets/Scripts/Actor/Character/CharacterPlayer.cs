using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPlayer : Character
{
    [SerializeField]
    protected AnimationPlayerComponent animationPlayerComponent;
    [SerializeField]
    protected WeaponHolderPlayerComponent weaponHolderComponent;
    [SerializeField]
    protected PlayerInputRotationComponent inputRotationComponent;
    [SerializeField]
    protected SpriteRendererHolderComponent spriteRendererHolder;
    public WeaponHolderPlayerComponent WeaponHolderComponent => weaponHolderComponent;
    public PlayerInputRotationComponent InputRotationComponent => inputRotationComponent;
    public SpriteRendererHolderComponent SpriteRendererHolder => spriteRendererHolder;
    protected override void OnHurt(DamageStruct ds, RaycastHit2D raycastHit)
    {
        spriteRendererHolder.OnHurt();
        characterMovement.Push(-raycastHit.normal, ds.power);
        base.OnHurt(ds, raycastHit);
    }
    protected override void OnDeath(DamageStruct ds, RaycastHit2D raycastHit)
    {
        animationPlayerComponent.enabled = false;
        weaponHolderComponent.enabled = false;
        inputRotationComponent.enabled = false;
        base.OnDeath(ds, raycastHit);
    }
    protected override void OnResurrect()
    {
        animationPlayerComponent.enabled = true;
        weaponHolderComponent.enabled = true;
        inputRotationComponent.enabled = true;
        base.OnResurrect();
    }
    public override JSONObject Save(JSONObject jsonObject)
    {
        base.Save(jsonObject);
        jsonObject.Add("animation", animationPlayerComponent.Save(new JSONObject()));
        jsonObject.Add("weaponHolder", weaponHolderComponent.Save(new JSONObject()));
        jsonObject.Add("spriteHolder", spriteRendererHolder.Save(new JSONObject()));
        return jsonObject;
    }
    public override JSONObject Load(JSONObject jsonObject)
    {
        base.Load(jsonObject);
        animationPlayerComponent.Load(jsonObject["animation"].AsObject);
        weaponHolderComponent.Load(jsonObject["weaponHolder"].AsObject);
        spriteRendererHolder.Load(jsonObject["spriteHolder"].AsObject);
        return jsonObject;
    }
}