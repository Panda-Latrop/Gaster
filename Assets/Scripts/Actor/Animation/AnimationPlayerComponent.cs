using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AnimationPlayerComponent : AnimationCharacterComponent
{
    [SerializeField]
    protected Animator weaponAnimator;
    [SerializeField]
    protected Transform bodyTransform, weaponSocketTransform;
    [SerializeField]
    protected SpriteRenderer[] handSprites;
    protected bool isFlipped;
    [SerializeField]
    protected bool hasWeapon, twoHandedWeapon;

    public bool GetHasWeapon() => hasWeapon;
    public void SetHasWeapon(bool has, bool twoHandedWeapon)
    {
        this.twoHandedWeapon = twoHandedWeapon;
        hasWeapon = has;
        handSprites[0].enabled = !has || !twoHandedWeapon; 
        handSprites[1].enabled = !has ;
        handSprites[2].enabled = has; 
        handSprites[3].enabled = has && twoHandedWeapon;
    }

    public override void PlayShoot()
    {
        weaponAnimator.Play(shootHash, 0);
    }

    protected void Flip(bool flip)
    {
        isFlipped = flip;
        
        if (!flip)
        {
            bodyTransform.localRotation = Quaternion.identity;
            weaponSocketTransform.localRotation = Quaternion.identity;
            
        }
        else
        {
            bodyTransform.localRotation = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);
            weaponSocketTransform.localRotation = new Quaternion(1.0f, 0.0f, 0.0f, 0.0f);           
        }
        Vector2 vector = weaponSocketTransform.localPosition;
        vector.y *= -1.0f;
        weaponSocketTransform.localPosition = vector;
    }
    protected override void LateUpdate()
    {
        base.LateUpdate();
        //weaponAnimator.SetBool(moveHash, characterMovement.Rigidbody.velocity.sqrMagnitude > 0.0f);
        Vector2 orientation = this.orientation.Direction;
        if (orientation.x >= 0 && isFlipped)
            Flip(false);
        else
        {
            if (orientation.x < 0 && !isFlipped)
                Flip(true);
        }
    }

    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        jsonObject.Add("hasWeapon", new JSONBool(hasWeapon));
        jsonObject.Add("twoHandedWeapon", new JSONBool(twoHandedWeapon));
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        SetHasWeapon(jsonObject["hasWeapon"].AsBool, jsonObject["twoHandedWeapon"].AsBool);
        return jsonObject;
    }
}
