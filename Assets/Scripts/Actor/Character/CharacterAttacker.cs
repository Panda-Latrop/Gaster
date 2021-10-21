using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttacker : Character , IPoolCharacterAttacker
{
    [SerializeField]
    protected string specifier = "base";
    [SerializeField]
    protected SpriteRendererHolderComponent spriteRendererHolder;
    [SerializeField]
    protected AnimationCharacterComponent animationCharacterComponent;
    [SerializeField]
    protected WeaponHolderBaseComponent weaponHolderComponent;
    [SerializeField]
    protected AIPerceptionComponent perception;
    [SerializeField]
    protected AudioSource voiceSource;
    public SpriteRendererHolderComponent SpriteRendererHolder => spriteRendererHolder;
    public AnimationCharacterComponent AnimationCharacterComponent => animationCharacterComponent;
    public WeaponHolderBaseComponent WeaponHolderComponent => weaponHolderComponent;
    public AIPerceptionComponent Perception => perception;

    public string PoolTag => this.GetType().Name + specifier;

    public Transform Transform => transform;

    public AudioSource VoiceSource => voiceSource;

    public virtual void OnPush()
    {
        health.Kill();
        saveTag = "";
        gameObject.SetActive(false);
    }
    public virtual void OnPop()
    {
        gameObject.SetActive(true);
        health.Resurrect();
    }
    protected override void OnHurt(DamageStruct ds, RaycastHit2D raycastHit)
    {
        spriteRendererHolder.OnHurt();
        characterMovement.Push(ds.direction, ds.power);
        base.OnHurt(ds, raycastHit);
    }

    protected override void OnDeath(DamageStruct ds, RaycastHit2D raycastHit)
    {
        animationCharacterComponent.enabled = false;
        weaponHolderComponent.enabled = false;
        weaponHolderComponent.StopAll();
        perception.enabled = false;
        spriteRendererHolder.Clear();
        characterMovement.speedMultiply = 1.0f;
        base.OnDeath(ds, raycastHit);
        GameInstance.Instance.PoolManager.Push(this);
    }
    protected override void OnMorale(DamageStruct ds, RaycastHit2D raycastHit)
    {
        spriteRendererHolder.OnMorale();
        base.OnMorale(ds, raycastHit);
    }
    protected override void OnResurrect()
    {
        animationCharacterComponent.enabled = true;
        weaponHolderComponent.enabled = true;
        perception.enabled = true;
        base.OnResurrect();
    }
    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
    }

    public void SetScale(Vector3 scale)
    {
        transform.localScale = scale;
    }

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
    }

    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        jsonObject.Add("animation", animationCharacterComponent.Save( new JSONObject()));
        jsonObject.Add("weaponHolder", weaponHolderComponent.Save(new JSONObject()));
        jsonObject.Add("spriteHolder", spriteRendererHolder.Save(new JSONObject()));
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        animationCharacterComponent.Load(jsonObject["animation"].AsObject);
        weaponHolderComponent.Load(jsonObject["weaponHolder"].AsObject);
        spriteRendererHolder.Load(jsonObject["spriteHolder"].AsObject);
        return jsonObject;
    }
}