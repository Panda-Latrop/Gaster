using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemaActor : Actor
{
    [SerializeField]
    protected new Rigidbody2D rigidbody;
    [SerializeField]
    protected AnimationBaseComponent animationComponent;
    [SerializeField]
    protected CharacterMovementComponent characterMovement;

    public override JSONObject Save(JSONObject jsonObject)
    {
        base.Save(jsonObject);
        SaveSystem.RigidbodySave(jsonObject, rigidbody);
        jsonObject.Add("animation", animationComponent.Save(new JSONObject()));
        jsonObject.Add("movement", characterMovement.Save(new JSONObject()));
        return jsonObject;
    }
    public override JSONObject Load(JSONObject jsonObject)
    {
        base.Load(jsonObject);
        SaveSystem.RigidbodyLoad(jsonObject, rigidbody);
        animationComponent.Load(jsonObject["animation"].AsObject);
        characterMovement.Load(jsonObject["movement"].AsObject);
        return jsonObject;
    }
}
